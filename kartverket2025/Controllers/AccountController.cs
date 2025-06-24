using kartverket2025.Models.DomainModels;
using kartverket2025.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace kartverket2025.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // --- Register Map User ---
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                TempData["Message"] = "You are already logged in. Please log out to register a new account.";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                TempData["Message"] = "You are already logged in. Please log out to register a new account.";
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
                return View(registerViewModel);

            var user = new ApplicationUser
            {
                UserName = registerViewModel.Email,
                Email = registerViewModel.Email,
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName
            };

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return View(registerViewModel);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Map User");
            if (!roleResult.Succeeded)
            {
                AddErrors(roleResult);
                return View(registerViewModel);
            }

            TempData["Message"] = "Registration successful! You can now log in.";
            return RedirectToAction("Login");
        }

        // --- Register Case Handler ---
        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegisterCaseHandler()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                TempData["Message"] = "You are already logged in. Please log out to register a new account.";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCaseHandler(RegisterViewModel registerViewModel)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                TempData["Message"] = "You are already logged in. Please log out to register a new account.";
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
                return View(registerViewModel);

            var user = new ApplicationUser
            {
                UserName = registerViewModel.Email,
                Email = registerViewModel.Email,
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName
            };

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return View(registerViewModel);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Case Handler");
            if (!roleResult.Succeeded)
            {
                AddErrors(roleResult);
                return View(registerViewModel);
            }

            TempData["Message"] = "Registration successful! You can now log in.";
            return RedirectToAction("Login");
        }

        // --- Login ---
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                TempData["Message"] = "You are already logged in.";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                TempData["Message"] = "You are already logged in.";
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
                return View(loginViewModel);

            var logInResult = await _signInManager.PasswordSignInAsync(
                loginViewModel.Email,
                loginViewModel.Password,
                loginViewModel.RememberMe,
                lockoutOnFailure: false);

            if (logInResult.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt (email or password incorrect)");
                return View(loginViewModel);
            }
        }

        // --- Logout ---
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            TempData["Message"] = "You have been logged out.";
            return RedirectToAction("Index", "Home");
        }

        // --- Helper for error display ---
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}