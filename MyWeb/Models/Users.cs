using Microsoft.AspNetCore.Identity;

namespace MyWeb.Models
{
    public class Users: IdentityUser
    {
        public string FullName { get; set; }
    }
}
