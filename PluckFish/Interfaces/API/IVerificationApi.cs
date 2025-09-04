using PluckFish.Models.API;

namespace PluckFish.Interfaces.API
{
    public interface IVerificationApi
    {
        public bool Verify(string token);
        public List<ApiToken> GetApiTokens();
    }
}
