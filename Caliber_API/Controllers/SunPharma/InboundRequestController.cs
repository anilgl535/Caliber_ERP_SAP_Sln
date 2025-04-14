
using System.Data;
using Caliber_Components.Autherization;
using Caliber_Components.DBComponents;
using Caliber_Models.Models.SunPharma;
using Microsoft.AspNetCore.Mvc;

namespace Caliber_API.Controllers.SunPharma
{
    [Route("[controller]")]
    [ApiController]
    [ServiceFilter(typeof(AuthFilter))]
    public class InboundRequestController : ControllerBase
    {
        private readonly IDBAccess dbAccess;
        public InboundRequestController(IDBAccess _dbAccess)
        {
            dbAccess = _dbAccess;
        }

        [HttpPost("LotRequest")]
        [ServiceFilter(typeof(PreInsert))]
        public async Task<IActionResult> SAPTransferLot([FromBody] InboundModel requestModel)
        {
            if (ModelState.IsValid)
            {
                var response = await dbAccess.WriteDataAsync(CommandType.StoredProcedure, "STP_SAP_ADD_LOT", requestModel, new SunparmaInboundResponseModel());


                if (response.OutputParams is SunparmaInboundResponseModel InboundResponseModel && ((SunparmaInboundResponseModel)response.OutputParams).ReturnStatus > 0)
                {
                    return new JsonResult(new { Message = "Transfered Successfully", InspectionLot = requestModel.MemoNumber }) { StatusCode = 202 };
                }
                else if (((SunparmaInboundResponseModel)response.OutputParams).ReturnStatus == 0)
                {
                    return new JsonResult(new { Message = ((SunparmaInboundResponseModel)response.OutputParams).KeyField ?? "Database Issue.", InspectionLot = requestModel.MemoNumber }) { StatusCode = 403 };
                }
                else
                {
                    return new JsonResult(new { Message = ((SunparmaInboundResponseModel)response.OutputParams).KeyField ?? "Database Issue.", InspectionLot = requestModel.MemoNumber }) { StatusCode = 500 };
                }

            }
            else
            {
                return await Task.Run(() => BadRequest());
            }
        }


    }
}
