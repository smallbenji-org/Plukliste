namespace PluckFish.Models.API
{
    public class ApiToken
    {
        public string Token { get; }
        DateTime ExpirationDate { get; }
        public ApiToken(string token) 
        {
            Token = token;
            ExpirationDate = DateTime.UtcNow;
        }
    }
}
