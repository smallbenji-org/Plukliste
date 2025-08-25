using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using PluckFish.Models;

namespace PluckFish.Controllers
{
    [ApiController]
    [Route("Auth")]
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> IndexAsync([FromBody] LoginDto model)
        {
            Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(model.username, model.password, isPersistent: true, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Unauthorized("Invalid login attempt.");
            }
            return Ok("Logged in");
        }

        [HttpPost("FormLogin")]
        public async Task<IActionResult> FormLogin([FromForm] string username, [FromForm] string password)
        {
            Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(username, password, isPersistent: true, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Unauthorized("Invalid login attempt.");
            }

            return Ok("Logged in");
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = model.username,
                FullName = model.fullname,
                Email = model.username
            };
            IdentityResult result = await userManager.CreateAsync(user, model.password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User registered");
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok("Signed out");
        }

        public class LoginDto
        {
            public string username { get; set; }
            public string password { get; set; }
        }

        public class RegisterDto
        {
            public string username { get; set; }
            public string fullname { get; set; }
            public string password { get; set; }
        }
    }
}
