using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyWeb.Models;
using MyWeb.ViewModels;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace MyWeb.Repositories
{
    public class AccountRepository: IAccountRepository
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;

        public AccountRepository(SignInManager<Users> signInManager, UserManager<Users> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                false
            );
        }
        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            var user = new Users
            {
                FullName = model.Name,
                Email = model.Email,
                UserName = model.Email
            };
           return await userManager.CreateAsync(user, model.Password);
        }
        public async Task<Users?> GetUserByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Trả về lỗi nếu không tìm thấy user
                return IdentityResult.Failed(new IdentityError { Description = "Email not found." });
            }
            var removeResult = await userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                return removeResult;
            }

            var addResult = await userManager.AddPasswordAsync(user, model.NewPassword);
            return addResult;
        }
        public async Task Logout()
        {
            await signInManager.SignOutAsync();
        }
    }
}
