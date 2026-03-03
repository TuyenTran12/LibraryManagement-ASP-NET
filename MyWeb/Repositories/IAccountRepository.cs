using Microsoft.AspNetCore.Identity;
using MyWeb.Models;
using MyWeb.ViewModels;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace MyWeb.Repositories
{
    public interface IAccountRepository
    {
        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task<IdentityResult> RegisterAsync(RegisterViewModel model);

        Task<Users?> GetUserByEmailAsync(string email);

        Task<IdentityResult> ChangePasswordAsync(ChangePasswordViewModel model);

        Task Logout();

        Task GenerateAndSendOTPAsync(Users user);

        Task<bool> VerifyOTPAsync(string email, string otpCode);
    }
}
