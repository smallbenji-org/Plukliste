using PluckFish.Interfaces.API;
using PluckFish.Models.API;

namespace PluckFish.Components.PostgresRepositories.API
{
    public class PostgresApiTokenRepository : IVerificationApi
    {
        public bool Verify(string token)
        {
            // Getting all API Tokens
            List<ApiToken> tokens = GetApiTokens();
            if (tokens == null || tokens.Count == 0) { return false; }

            if (tokens.Any(x => x.Token == token))
            {
                return true; // Acces granted | Token exists
            }

            return false; // Fallback to false
        }

        public List<ApiToken> GetApiTokens()
        {
            List<ApiToken> tokens = new List<ApiToken>();

            ApiToken testToken = new ApiToken("123"); // Mit test data
            tokens.Add(testToken);

            return tokens;
        }
    }
}
