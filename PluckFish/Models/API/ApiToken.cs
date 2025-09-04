namespace PluckFish.Models.API
{
    public class ApiToken
    {
        public string Token { get; }
        public DateTime ExpirationDate { get; }
        public ApiToken(string token, DateTime expirationDate) 
        {
            Token = token;
            ExpirationDate = expirationDate;
        }
    }
}
