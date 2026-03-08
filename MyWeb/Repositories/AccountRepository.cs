using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyWeb.Models;
using MyWeb.ViewModels;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace MyWeb.Repositories
{
    public class AccountRepository : IAccountRepository
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
            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
            }

            return result;
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
        public async Task GenerateAndSendOTPAsync(Users user)
        {
            var random = new Random();
            var otp = random.Next(100000, 999999).ToString();

            user.OTPCode = otp;
            user.OTPExpiryTime = DateTime.Now.AddMinutes(5);

            await userManager.UpdateAsync(user);

            // 3. Gửi Email (Ở đây mình in ra Console để bạn test trước)
            // Sau này bạn sẽ thay dòng này bằng code gửi mail thật (SMTP)
            Console.WriteLine($"=== OTP CỦA {user.Email} LÀ: {otp} ===");
        }
        public async Task<bool> VerifyOTPAsync(string email, string otpCode)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return false;

            // Kiểm tra 3 điều kiện:
            // 1. Mã OTP trùng khớp
            // 2. Mã chưa hết hạn
            // 3. Mã không được rỗng
            if (user.OTPCode == otpCode && user.OTPExpiryTime > DateTime.Now)
            {
                // Xác thực thành công -> Xóa OTP đi để không dùng lại được
                user.EmailConfirmed = true;
                user.OTPCode = null;
                user.OTPExpiryTime = null;

                await userManager.UpdateAsync(user);
                return true;
            }
            return false;
        }
    }
}