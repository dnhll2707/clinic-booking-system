using Microsoft.AspNetCore.Identity;

namespace DaoNuHoangLyLy_2123110414.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}