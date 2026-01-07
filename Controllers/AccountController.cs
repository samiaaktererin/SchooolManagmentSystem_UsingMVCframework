using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MyMvcApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ================= LOGIN (GET) =================
        [HttpGet]
        public IActionResult Login(string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Role = role; // Admin / Teacher
            return View();
        }

        // ================= LOGIN (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string email,
            string password,
            string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                ViewBag.Error = "Invalid login request";
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ViewBag.Error = "Invalid email or password";
                ViewBag.Role = role;
                return View();
            }

            // 🔒 Role strict check
            if (!await _userManager.IsInRoleAsync(user, role))
            {
                ViewBag.Error = $"You are not allowed to login as {role}";
                ViewBag.Role = role;
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                password,
                false,
                false);

            if (!result.Succeeded)
            {
                ViewBag.Error = "Invalid email or password";
                ViewBag.Role = role;
                return View();
            }

            // ✅ Correct redirect
            if (role == "Admin")
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            if (role == "Teacher")
            {
                return RedirectToAction("MyProfile", "Teachers");
            }

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ================= LOGOUT =================
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
