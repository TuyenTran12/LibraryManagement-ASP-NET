using Microsoft.AspNetCore.Identity;

namespace MyWeb.Models
{
    public class Users: IdentityUser
    {
        public string FullName { get; set; }
        public string? OTPCode { get; set; }
        public DateTime? OTPExpiryTime { get; set; }
    }
}
