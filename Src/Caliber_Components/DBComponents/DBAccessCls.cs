

namespace Caliber_Components.DBComponents
{
    public class DBAccessCls : IDBAccess, IDisposable
    {
        /// <summary>
        /// Gets or sets the connection string information.
        /// </summary>
        private string ConnKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets the exception message if an exception occurred after executing the database access class methods.
        /// </summary>
        public string ExMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets the exception message number if an exception occurred after executing the database access class methods.
        /// </summary>
        public int ExMsgNo { get; set; } = 0;

        /// <summary>
        /// Specifies the maximum duration (in seconds) a database command is allowed to execute before timing out.
        /// </summary>
        private int? CommandTimeOut { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbAccessCls"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string for database connectivity.</param>
        /// <param name="commandTimeOut">The maximum duration (in seconds) a database command is allowed to execute before timing out.</param>
        public DBAccessCls(string connectionString, int? commandTimeOut = 0)
        {
            ConnKey = connectionString;
            CommandTimeOut = commandTimeOut;
        }

        /// <summary>
        /// Executes a database command with provided input parameters, retrieves output parameters if any, and manages database transactions.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        /// <param name="commandType">Specifies the type of command (e.g., stored procedure or text).</param>
        /// <param name="commandText">The SQL query or stored procedure to execute.</param>
        /// <param name="inputParamsList">A list of input parameter objects for the command.</param>
        /// <param name="outputParams">An object with properties designated to hold output parameters.</param>
        /// <returns>Returns a tuple containing output parameters, exception message, and exception number.</returns>
        public async Task<(object OutputParams, string ExceptionMsg, int ExceptionNo)> ExtWriteDataAsync(
            string connectionString,
            CommandType commandType,
            string commandText,
            List<object> inputParamsList,
            object? outputParams)
        {
            string exceptionMsg = string.Empty;
            int exceptionNo = 0;
            var @params = new DynamicParameters();

            // Add input parameters
            if (inputParamsList != null)
            {
                foreach (var inputParams in inputParamsList)
                {
                    if (inputParams is not null)
                    {
                        @params.AddDynamicParams(inputParams);
                    }
                }
            }

            // Add output parameters
            if (outputParams != null)
            {
                foreach (var prop in outputParams.GetType().GetProperties())
                {
                    @params.Add(prop.Name, prop.GetValue(outputParams, null), direction: ParameterDirection.Output);
                }
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            await connection.ExecuteAsync(commandText, @params, transaction, commandType: commandType);
                            await transaction.CommitAsync();
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionNo = 1;
                exceptionMsg = ex.Message; // Avoid using `ex.ToString()` unless needed for debugging purposes.
            }

            // Handle output parameters
            if (outputParams != null && string.IsNullOrEmpty(exceptionMsg))
            {
                foreach (var prop in outputParams.GetType().GetProperties())
                {
                    try
                    {
                        switch (Type.GetTypeCode(prop.PropertyType))
                        {
                            case TypeCode.Int32:
                                prop.SetValue(outputParams, @params.Get<int>(prop.Name), null);
                                break;
                            case TypeCode.Double:
                                prop.SetValue(outputParams, @params.Get<double>(prop.Name));
                                break;
                            case TypeCode.String:
                                prop.SetValue(outputParams, @params.Get<string>(prop.Name));
                                break;
                            case TypeCode.DateTime:
                                prop.SetValue(outputParams, @params.Get<DateTime>(prop.Name));
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        exceptionNo = 2;
                        exceptionMsg += $"\nError processing output parameter {prop.Name}: {ex.Message}";
                    }
                }
            }

            return (outputParams ?? new object(), exceptionMsg ?? string.Empty, exceptionNo);
        }

        /// <summary>
        /// Executes a database command with provided input parameters, retrieves output parameters if any, and manages database transactions.
        /// </summary>
        /// <param name="commandType">Specifies the type of command (e.g., stored procedure or text).</param>
        /// <param name="commandText">The SQL query or stored procedure to execute.</param>
        /// <param name="inputParams">An object containing input parameters for the command.</param>
        /// <param name="outputParams">An object with properties designated to hold output parameters.</param>
        /// <returns>Returns a tuple containing output parameters, exception message, and exception number.</returns>
        public async Task<(object OutputParams, string ExceptionMsg, int ExceptionNo)> WriteDataAsync(
                 CommandType commandType,
                 string commandText,
                 object inputParams,
                 object? outputParams)
        {
            var objs = new List<object> { inputParams };

            var result = await ExtWriteDataAsync(ConnKey ?? string.Empty, commandType, commandText, objs, outputParams);

            return (result.OutputParams ?? new object(), result.ExceptionMsg ?? string.Empty, result.ExceptionNo);
        }

        /// <summary>
        /// Executes a database command with provided input parameters, if any, and manages database transactions.
        /// </summary>
        /// <param name="commandType">Specifies the type of command (e.g., stored procedure or text).</param>
        /// <param name="commandText">The SQL query or stored procedure to execute.</param>
        /// <param name="inputParams">An object containing input parameters for the command.</param>
        /// <returns>Returns the exception message if any.</returns>
        public async Task<string> WriteDataAsync(
            CommandType commandType,
            string commandText,
            object inputParams)
        {
            var objs = new List<object> { inputParams };

            var outObjs = new object();

            var result = await ExtWriteDataAsync(ConnKey, commandType, commandText, objs, outObjs);

            return result.ExceptionMsg;
        }

        /// <summary>
        /// Asynchronously reads data from the database.
        /// </summary>
        /// <typeparam name="T">The type of the data to retrieve.</typeparam>
        /// <param name="connectionString">The connection string to the database.</param>
        /// <param name="commandType">Specifies the type of command (e.g., stored procedure or text).</param>
        /// <param name="commandText">The SQL query or stored procedure to execute.</param>
        /// <param name="param">An object containing parameters for the command.</param>
        /// <returns>Returns the data retrieved from the database.</returns>
        public async Task<T> ExtReadData<T>(
    string connectionString,
    CommandType commandType,
    string commandText,
    object param)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var result = await connection.QueryFirstOrDefaultAsync<T>(
                        commandText,
                        param,
                        commandType: commandType
                    );
                    return result!;
                }
            }
            catch
            {
                throw; // Re-throw the exception to allow higher-level handling
            }
        }

        /// <summary>
        /// Asynchronously retrieves a list of data from the database.
        /// </summary>
        /// <typeparam name="T">The type of objects to return in the list.</typeparam>
        /// <param name="commandText">The SQL query or stored procedure to execute.</param>
        /// <param name="commandType">Specifies whether the command is a text query or a stored procedure.</param>
        /// <param name="param">The parameters to pass to the query or stored procedure.</param>
        /// <returns>A list of type <typeparamref name="T"/> representing the retrieved data.</returns>
        public async Task<List<T>> ReadListData<T>(
            string commandText,
            CommandType commandType,
            object param)
        {
            try
            {
                using (var connection = new SqlConnection(ConnKey))
                {
                    await connection.OpenAsync();

                    var result = await connection.QueryAsync<T>(
                        commandText,
                        param,
                        commandType: commandType
                    );
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the list of data.", ex);
            }
        }

        /// <summary>
        /// Disposes the resources used by the <see cref="DbAccessCls"/> class.
        /// </summary>
        public void Dispose()
        {
            ConnKey = string.Empty;
            CommandTimeOut = null;
        }
    }
}
