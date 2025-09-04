using PluckFish.Models.API;

namespace PluckFish.Interfaces.API
{
    public interface IVerificationApi
    {
        public void createToken(string userId);
        public bool Verify(string token);
        public List<ApiToken> GetApiTokens();
        public List<ApiToken> GetApiTokensForUser(string userId);
        public void RemoveApiTokenFromUser(string userId, string token);
    }
}
