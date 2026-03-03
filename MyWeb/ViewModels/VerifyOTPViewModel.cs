using System.ComponentModel.DataAnnotations;

namespace MyWeb.ViewModels
{
    public class VerifyOTPViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mã OTP")]
        [StringLength(6, ErrorMessage = "Mã OTP phải có 6 ký tự")]
        public string OTP { get; set; }

        public string? ActionType { get; set; }
    }
}