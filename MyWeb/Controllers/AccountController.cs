using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyWeb.Models;
using MyWeb.Repositories;
using MyWeb.ViewModels;

namespace MyWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
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
                    return RedirectToAction("Index", "Books");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
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
                    ModelState.AddModelError("", "Email not found.");
                    return View(model);
                }
                else
                {
                    return RedirectToAction("ChangePassword", "Account", new { email = model.Email });
                }
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
                ModelState.AddModelError("", "Something went wrong. Try again.");
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
