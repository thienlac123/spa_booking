// DoAnSPA/Models/SpaUser.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DoAnSPA.Models
{
    public class SpaUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string? Age { get; set; }

        // 👇 Thêm dòng này để hỗ trợ mapping 1–1
        public CustomerProfile? CustomerProfile { get; set; }
    }
}
