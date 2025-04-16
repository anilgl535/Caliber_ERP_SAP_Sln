
using Caliber_Models.Models.BioPharma;

namespace Caliber_API.Controllers.BioPharma
{
    [Route("api/[controller]")]
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
                var response = await dbAccess.WriteDataAsync(CommandType.StoredProcedure, "STP_SAP_ADD_LOT", requestModel, new BiopharmaInboundResponseModel());


                if (response.OutputParams is BiopharmaInboundResponseModel InBoundResponseModel && ((BiopharmaInboundResponseModel)response.OutputParams).ReturnStatus > 0)
                {
                    return new JsonResult(new { Message = "Transfered Successfully", requestModel.InspectionLot }) { StatusCode = 202 };
                }
                else if (((BiopharmaInboundResponseModel)response.OutputParams).ReturnStatus == 0)
                {
                    return new JsonResult(new { Message = ((BiopharmaInboundResponseModel)response.OutputParams).KeyField, requestModel.InspectionLot }) { StatusCode = 403 };
                }
                else
                {
                    return new JsonResult(new { Message = ((BiopharmaInboundResponseModel)response.OutputParams).KeyField, requestModel.InspectionLot }) { StatusCode = 500 };
                }

            }
            else
            {
                return await Task.Run(() => BadRequest());
            }
        }

    }
}
