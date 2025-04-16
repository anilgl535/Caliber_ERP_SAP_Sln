using Caliber_Components.Authentication.Token;
using Caliber_Components.DBComponents;
using Caliber_Components.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Caliber_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IDBAccess dbAccess;
        public TokenController(IDBAccess _dbAccess)
        {
            dbAccess = _dbAccess;
        }
        [HttpPost("GenerateToken")]
        public async Task<IActionResult> GenerateToken(LoginUserModel user)
        {
            if (ModelState.IsValid && user != null)
            {

                var matchedUser = (await GetUsers()).FirstOrDefault(u => u.UserID == user.UserID && u.Passsword == user.Passsword);

                if (matchedUser != null)
                {
                    var result = await Tokens.GenerateToken(user);
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Invalid user credentials.");
                }
            }
            else
            {
                return BadRequest("Invalid model state or user is null.");
            }
        }

        [NonAction]
        public async Task<List<LoginUserModel>> GetUsers()
        {
            return await dbAccess.ReadListData<LoginUserModel>("STP_SAP_GET_USERS", System.Data.CommandType.StoredProcedure, new());
            //return responce.ToList<LoginUserModel>();
        }

    }
}
