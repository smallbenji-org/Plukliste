using PluckFish.Models;

namespace PluckFish.Interfaces
{
    public interface IAuthRepository
    {
        List<ApplicationUser> GetAll();
    }
}