using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MyWeb.Repositories;
using MyWeb.ViewModels;
using MyWeb.Models;

namespace MyWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<Users> _userManager;
        private readonly IStringLocalizer<AccountController> _localizer;

        public AccountController(
            IAccountRepository accountRepository,
            IStringLocalizer<AccountController> localizer,
            UserManager<Users> userManager) 
        {
            _accountRepository = accountRepository;
            _localizer = localizer;
            _userManager = userManager;
        }

        [HttpPost]
        public IActionResult ChangeLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.LoginAsync(model);

                if (result.Succeeded)
                {
                    // 1. Tìm đối tượng User dựa trên Email
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user != null)
                    {
                        // 2. Kiểm tra nếu User có quyền "Admin"
                        if (await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            // Chuyển hướng đến trang quản lý sách cho Admin
                            return RedirectToAction("Index", "Books");
                        }
                    }

                    // 3. Mặc định (User thường hoặc không có Role đặc biệt) về trang chủ
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", _localizer["Error_InvalidLogin"]);
                    return View(model);
                }
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.RegisterAsync(model);
                if (result.Succeeded)
                {
                    var user = await _accountRepository.GetUserByEmailAsync(model.Email);
                    if (user != null)
                    {
                        await _accountRepository.GenerateAndSendOTPAsync(user);
                    }
                    else
                    {
                        ModelState.AddModelError("", _localizer["Error_UserNotFound"]);
                        return View(model);
                    }
                    return RedirectToAction("VerifyOTP", new { email = model.Email, type = "Register" });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(model);
                }
            }
            return View(model);
        }

        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _accountRepository.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", _localizer["Error_EmailNotFound"]);
                    return View(model);
                }
                else
                {
                    await _accountRepository.GenerateAndSendOTPAsync(user);
                    return RedirectToAction("VerifyOTP", "Account", new { email = model.Email, type = "ChangePassword" });
                }
            }
            return View(model);
        }

        public IActionResult VerifyOTP(string email, string type)
        {
            var model = new VerifyOTPViewModel
            {
                Email = email,
                ActionType = type
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTP(VerifyOTPViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.VerifyOTPAsync(model.Email, model.OTP);

                // Logic xử lý chung cho cả Register và ChangePassword
                if (result)
                {
                    if (model.ActionType == "ChangePassword")
                    {
                        return RedirectToAction("ChangePassword", "Account", new { email = model.Email });
                    }
                    else if (model.ActionType == "Register")
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                ModelState.AddModelError("", _localizer["Error_InvalidOTP"]);
                return View(model);
            }
            return View(model);
        }

        public IActionResult ChangePassword()
        {
            if (string.IsNullOrEmpty(Request.Query["email"]))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                Email = Request.Query["email"]
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.ChangePasswordAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", _localizer["Error_Generic"]);
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _accountRepository.Logout();
            return RedirectToAction("Login", "Account");
        }
    }
}