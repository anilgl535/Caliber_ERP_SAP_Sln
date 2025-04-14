

namespace Caliber_Components.Token
{

    /// <summary> Provides methods for token generation and validation. </summary>
    public static class Tokens
    {
        // Gets or sets the token settings.
        public static TokenSettings? TokenOptions { get; set; }

        public static int AuthenticationType { get; set; }

        //Sets the token settings.
        /// <param name="tokenSettings">The token settings to set.</param>
        public static void SetTokenSettings(TokenSettings tokenSettings)
        {
            TokenOptions = tokenSettings;
        }

        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">The user for whom to generate the token.</param>
        /// <returns>The generated JWT token.</returns>
        public static async Task<string> GenerateToken(LoginUserModel? user)
        {
            if (TokenOptions != null && user != null)
            {
                var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenOptions.Key));
                var signingCredentials = new SigningCredentials(
                    issuerSigningKey,
                    SecurityAlgorithms.HmacSha256Signature
                );
                var subject = new List<Claim>
                                    {
                                        new Claim(JwtRegisteredClaimNames.Name, user.UserID??""),
                                        new Claim(JwtRegisteredClaimNames.Email, user.Passsword ?? ""),
                                    };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(subject),
                    Issuer = TokenOptions.Issuer,
                    Audience = TokenOptions.Audience,
                    SigningCredentials = signingCredentials,
                    Expires = DateTime.UtcNow.AddMinutes(TokenOptions.ExpTimes),
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return await Task.Run(() => tokenHandler.WriteToken(token));
            }
            return "";

        }

        /// <summary>
        /// Checks if the specified token is expired.
        /// </summary>
        /// <param name="token">The token to check.</param>
        /// <returns>True if the token is expired; otherwise, false.</returns>
        public static bool IsTokenExpired(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (tokenHandler.CanReadToken(token))
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);

                // Extract the 'exp' claim
                var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);

                if (expClaim != null)
                {
                    var exp = long.Parse(expClaim.Value);
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;

                    // Compare the expiration time with the current time
                    return expirationTime < DateTime.UtcNow;
                }
            }

            return true; // Consider the token expired if it cannot be read or 'exp' claim is missing
        }

        /// <summary>
        /// Gets the user ID from the specified token.
        /// </summary>
        /// <param name="token">The token to extract the user ID from.</param>
        /// <returns>The user ID if found; otherwise, null.</returns>
        public static string? GetTokenUserId(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (tokenHandler.CanReadToken(token))
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);

                // Extract the 'name' claim
                return jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value;
            }

            return null;
        }

        /// <summary>
        /// Validates the specified token asynchronously.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <returns>True if the token is valid; otherwise, false.</returns>
        public static async Task<bool> ValidateTokenAsync(string token)
        {
            if (TokenOptions == null)
            {
                throw new InvalidOperationException("TokenOptions must be set before validating a token.");
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(TokenOptions.Key);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = TokenOptions.Audience,
                    ValidIssuer = TokenOptions.Issuer,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero // Set clock skew to zero so tokens expire exactly at token expiration time
                }, out SecurityToken validatedToken);

                return await Task.Run(() => true);
            }
            catch (Exception)
            {
                return await Task.Run(() => false);
            }
        }
    }
}
