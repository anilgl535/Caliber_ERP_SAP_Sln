namespace Caliber_Components.DBComponents
{
    public interface IDBAccess
    {
        /// <summary>
        /// Executes a database command with provided input parameters, 
        /// retrieves output parameters if any, and manages database transactions.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        /// <param name="commandType">Specifies the type of command (e.g., stored procedure or text).</param>
        /// <param name="commandText">The SQL query or stored procedure to execute.</param>
        /// <param name="inputParamsList">A list of input parameter objects for the command.</param>
        /// <param name="outputParams">An object with properties designated to hold output parameters.</param>
        /// <returns>Returns a tuple containing output parameters, exception message, and exception number.</returns>
        Task<(object OutputParams, string ExceptionMsg, int ExceptionNo)> ExtWriteDataAsync(
            string connectionString,
            CommandType commandType,
            string commandText,
            List<object> inputParamsList,
            object? outputParams);

        /// <summary>
        /// Executes a database command with provided input parameters and retrieves output parameters if any.
        /// </summary>
        /// <param name="commandType">Specifies the type of command (e.g., stored procedure or text).</param>
        /// <param name="commandText">The SQL query or stored procedure to execute.</param>
        /// <param name="inputParams">An object containing input parameters for the command.</param>
        /// <param name="outputParams">An object with properties designated to hold output parameters.</param>
        /// <returns>Returns a tuple containing output parameters, exception message, and exception number.</returns>
        Task<(object OutputParams, string ExceptionMsg, int ExceptionNo)> WriteDataAsync(
            CommandType commandType,
            string commandText,
            object inputParams,
            object? outputParams);

        /// <summary>
        /// Executes a database command and retrieves data.
        /// </summary>
        /// <typeparam name="T">The type of the data to retrieve.</typeparam>
        /// <param name="connectionString">The connection string to the database.</param>
        /// <param name="commandType">Specifies the type of command (e.g., stored procedure or text).</param>
        /// <param name="commandText">The SQL query or stored procedure to execute.</param>
        /// <param name="param">An object containing parameters for the command.</param>
        /// <returns>Returns the data retrieved from the database.</returns>
        Task<T> ExtReadData<T>(
            string connectionString,
            CommandType commandType,
            string commandText,
            object param);

        /// <summary>
        /// Executes a database command and retrieves a list of data.
        /// </summary>
        /// <typeparam name="T">The type of the data to retrieve.</typeparam>
        /// <param name="commandText">The SQL query or stored procedure to execute.</param>
        /// <param name="commandType">Specifies the type of command (e.g., stored procedure or text).</param>
        /// <param name="param">An object containing parameters for the command.</param>
        /// <returns>Returns a list of data retrieved from the database.</returns>
        Task<List<T>> ReadListData<T>(
            string commandText,
            CommandType commandType,
            object param);
    }
}
