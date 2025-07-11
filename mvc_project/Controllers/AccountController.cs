using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mvc_project.Models;

namespace mvc_project.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string email,
            string password,
            string fullname,
            string? address
        )
        {
            var user = new AppUser
            {
                UserName = email,
                Email = email,
                FullName = fullname,
                Address = address,
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View();
            }
        }

        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            Console.WriteLine(user == null ? "User is null" : $"User found: {user.UserName}");

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz giriş.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                password,
                false,
                false
            );

            Console.WriteLine(
                result.Succeeded ? "Password sign in succeeded" : "Password sign in failed"
            );

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Console.WriteLine("Roles count: " + roles.Count);
                foreach (var role in roles)
                {
                    Console.WriteLine("Role: " + role);
                }

                if (roles.Contains("Admin"))
                    return RedirectToAction("Index", "Admin");
                else if (roles.Contains("WarehouseManager"))
                {
                    return RedirectToAction("Index", "Warehouse");
                }
                else
                    return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "E-posta veya parola hatalı.");
                return View();
            }
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
