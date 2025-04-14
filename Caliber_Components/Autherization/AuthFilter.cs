namespace Caliber_Components.Autherization
{
    public class AuthFilter : Attribute, IAsyncAuthorizationFilter
    {
        private readonly IDBAccess _dbAccess;
        public AuthFilter(IDBAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {

            // You can extract Authorization Details from the request headers.
            var authorization = context.HttpContext.Request.Headers["Authorization"].ToString();

            var request = context.HttpContext.Request;
            var authorizationparts = authorization.Split(' ');
            //For Header Authorization is available or not
            if ((authorization == null || string.IsNullOrEmpty(authorizationparts[0])) && string.IsNullOrEmpty(context.HttpContext.Request.Headers[ApiKey.Apikeys.ApiKey]))
            {
                context.Result = new JsonResult(new { message = "Authorization header is missing" })
                {
                    StatusCode = 401
                };
            }

            // Additional authorization logic can be added here by adding one more else if
            else if (authorizationparts[0] == "Bearer" && !string.IsNullOrEmpty(authorizationparts[0]) && Tokens.AuthenticationType == 1)
            {
                try
                {
                    //For verifying Token is expired or not.
                    if (!Tokens.IsTokenExpired(authorizationparts[1]))
                    {
                        bool isValidToken = await Tokens.ValidateTokenAsync(authorizationparts[1]);

                        if (!isValidToken)
                        {
                            // If the token is invalid, return 401 Unauthorized.
                            context.Result = new JsonResult(new { message = "Invalid Token" })
                            {
                                StatusCode = 401
                            };
                        }
                        else
                        {
                            var User = Tokens.GetTokenUserId(authorizationparts[1]);
                            var Identity = new ClaimsIdentity(new List<Claim>() { new Claim(ClaimTypes.Name, User ?? (authorizationparts[1])) }, "Jwt");
                            context.HttpContext.User = new ClaimsPrincipal(Identity);
                            return;
                        }
                    }
                    else
                    {
                        //if the Token Expired, return 401 Token Expired.
                        context.Result = new JsonResult(new { message = "Token was Expired" })
                        {
                            StatusCode = 401
                        };
                    }
                }
                catch (Exception)
                {
                    context.Result = new JsonResult(new { Message = "Invalid Token" }) { StatusCode = 400 };
                }
            }
            else if (context.HttpContext.Request.Headers.TryGetValue(ApiKey.Apikeys.ApiKey, out var extractedApiKey) && Tokens.AuthenticationType == 0)
            {
                if (!string.Equals(ApiKey.Apikeys.Keyvalue, extractedApiKey, StringComparison.OrdinalIgnoreCase))
                {
                    context.Result = new JsonResult(new { Message = "Invalid Api Key" }) { StatusCode = 401 };
                    return;
                }
                else
                {
                    var identity = new ClaimsIdentity(new List<Claim>
                    {
                                    new Claim(ClaimTypes.Name, ApiKey.Apikeys.ApiKey)
                                }, "Api-key");

                    context.HttpContext.User = new ClaimsPrincipal(identity);
                    return;
                }
            }
            else
            {
                context.Result = new JsonResult(new { Message = "Please Use Bearer Token/Api-Key Authorizations" })
                {
                    StatusCode = 409
                };
            }
            return;
        }
    }
}
