namespace Caliber_Components.Authentication.Token
{
    //Represents the settings for token generation and validation
    public class TokenSettings
    {
        // Gets or sets the issuer of the token.
        public string Issuer { get; set; } = string.Empty;

        //Gets or sets the audience of the token.
        public string Audience { get; set; } = string.Empty;

        // Gets or sets the key used for token encryption.
        public string Key { get; set; } = string.Empty;

        // Gets or sets the expiration time of the token in minutes.
        public int ExpTimes { get; set; }
    }

}
