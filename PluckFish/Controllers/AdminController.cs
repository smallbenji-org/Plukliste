using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PluckFish.Interfaces.API;
using PluckFish.Models;
using PluckFish.Models.API;

namespace PluckFish.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IVerificationApi apiRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public AdminController(IVerificationApi apiRepository, UserManager<ApplicationUser> userManager)
        {
            this.apiRepository = apiRepository;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Tokenhåndtering";
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            AdminViewModel model = new AdminViewModel(apiRepository.GetApiTokensForUser(user.Id));
            return View(model);
        }

        public async Task<IActionResult> createToken()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            
            apiRepository.createToken(user.Id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> removeToken(string token)
        {
            if (!string.IsNullOrWhiteSpace(token)) 
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                apiRepository.RemoveApiTokenFromUser(user.Id, token);
            }
            return RedirectToAction(nameof(Index));
        }

        public class AdminViewModel
        {
            public List<ApiToken> Tokens { get; }
            public AdminViewModel(List<ApiToken> tokens) 
            {
                Tokens = tokens;
            }
        }
    }
}
