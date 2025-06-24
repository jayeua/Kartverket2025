using kartverket2025.Models.DomainModels;
using kartverket2025.Models.ViewModels;
using kartverket2025.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace kartverket2025.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        // --- Profile Page ---
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ProfilePage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var roles = await _userManager.GetRolesAsync(user);
            var model = new ProfilePageViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = roles.Count > 0 ? roles[0] : ""
            };
            return View(model); // Views/Account/Profile.cshtml
        }

        // --- Settings Page (GET) ---
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var model = new SettingsViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
            return View(model); // Views/Account/Settings.cshtml
        }

        // --- Settings Page (POST) ---
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(SettingsViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            user.Email = model.Email;
            user.UserName = model.Email; // If you are using email as username
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);

            ViewBag.Message = "Settings updated!";
            return View(model);
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

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Message"] = "User not found.";
                return RedirectToAction("Login");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Message"] = "Your password has been changed successfully.";
                return RedirectToAction("Index", "Home");
            }
            AddErrors(result);
            return View(model);
        }

        // --- FORGOT PASSWORD ---

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("ForgotPasswordConfirmation");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { email = user.Email, token = token }, protocol: HttpContext.Request.Scheme);

            await _emailSender.SendEmailAsync(
                user.Email,
                "Reset Password",
                $"Reset your password by clicking <a href='{callbackUrl}'>here</a>."
            );

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // --- RESET PASSWORD ---

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (email == null || token == null)
                return RedirectToAction("Index", "Home");

            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("ResetPasswordConfirmation");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
                return RedirectToAction("ResetPasswordConfirmation");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}