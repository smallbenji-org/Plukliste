namespace PluckFish.Models
{
    public class ApplicationUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
    }

    public class ApplicationRole
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name {  set; get; }
        public string Description { get; set; }
        public string NormalizedName { get; set; }
    }
}
