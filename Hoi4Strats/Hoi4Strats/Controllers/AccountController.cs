using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedProj;
namespace Hoi4Strats.Controllers
{
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Skapa en säker POST-metod för att hantera utloggning
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            // Logga ut användaren
            await _signInManager.SignOutAsync();

            // Om returnUrl är null eller ogiltigt, omdirigera till hem-sidan
            return RedirectToAction("Index", "Home");
        }
    }
}

