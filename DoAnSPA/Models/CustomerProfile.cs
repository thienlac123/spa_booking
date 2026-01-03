// DoAnSPA/Models/CustomerProfile.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnSPA.Models
{
    public class CustomerProfile
    {
        [Key, ForeignKey(nameof(User))]              // PK cũng là FK sang SpaUser
        public string UserId { get; set; } = null!;  // = SpaUser.Id (AspNetUsers.Id)

        [Required, MaxLength(200)]
        public string FullName { get; set; } = null!;

        [Required, MaxLength(20)]
        public string Phone { get; set; } = null!;

        [MaxLength(255)]
        public string? Address { get; set; }

        public DateTime? DOB { get; set; }           // thay vì Age string

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public SpaUser User { get; set; } = null!;
    }
}
