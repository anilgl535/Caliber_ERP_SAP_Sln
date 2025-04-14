
namespace Caliber_API
{
    /// <summary>
    /// Represents a filter that logs action execution details.
    /// </summary>
    public class PreInsert : Attribute, IActionFilter
    {
        private readonly IDBAccess _dbAccess;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreInsert"/> class.
        /// </summary>
        /// <param name="dbAccess">The database access service.</param>
        public PreInsert(IDBAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        /// <summary>
        /// Called before an action executes.
        /// </summary>
        /// <param name="context">The context for the action.</param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.Count > 0)
            {
                var content = JsonConvert.SerializeObject(context.ActionArguments.FirstOrDefault().Value);
                var json = JObject.Parse(content);
                var inspectionLot = json.ContainsKey("InspectionLot")
                    ? json["InspectionLot"]?.ToString()
                    : json["MemoNumber"]?.ToString();

                _ = _dbAccess.WriteDataAsync(CommandType.StoredProcedure, "STP_ERP_LOT_REQ_JSON",
                                                new
                                                {
                                                    ReqJson = content,
                                                    InspectionLotNum = inspectionLot,
                                                    TransType = 1,
                                                    DownloadedOn = DateTime.UtcNow,
                                                    ErrorMsg = ""
                                                }
                                                , null);
                context.HttpContext.Items.Add("InspectionLot", inspectionLot);
            }
        }

        /// <summary>
        /// Called after an action executes.
        /// </summary>
        /// <param name="context">The context for the action.</param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is JsonResult objResult && objResult.StatusCode == 202)
            {
                var inspectionLot = context.HttpContext.Items["InspectionLot"]?.ToString() ?? string.Empty;
                _ = _dbAccess.WriteDataAsync(CommandType.StoredProcedure, "STP_ERP_LOT_REQ_JSON", new
                {
                    ReqJson = "",
                    InspectionLotNum = inspectionLot,
                    TransType = 3,
                    DownloadedOn = DateTime.UtcNow,
                    ErrorMsg = ""
                }, null);
            }
            else if (context.Result is JsonResult objResults && objResults.StatusCode == 403)
            {
                var inspectionLot = context.HttpContext.Items["InspectionLot"]?.ToString() ?? string.Empty;
                _ = _dbAccess.WriteDataAsync(CommandType.StoredProcedure, "STP_ERP_LOT_REQ_JSON", new
                {
                    ReqJson = "",
                    InspectionLotNum = inspectionLot,
                    TransType = 2,
                    DownloadedOn = DateTime.UtcNow,
                    ErrorMsg = ""
                }, null);
            }
            else if (context.Result is JsonResult objRes && objRes.StatusCode == 500)
            {
                var inspectionLot = context.HttpContext.Items["InspectionLot"]?.ToString() ?? string.Empty;
                string Message = JObject.Parse(JsonConvert.SerializeObject(objRes.Value))["Message"].ToString();
                _ = _dbAccess.WriteDataAsync(CommandType.StoredProcedure, "STP_ERP_LOT_REQ_JSON", new
                {
                    ReqJson = "",
                    InspectionLotNum = inspectionLot,
                    TransType = 2,
                    DownloadedOn = DateTime.UtcNow,
                    ErrorMsg = Message
                }, null);
            }
        }
    }
}
