using System;
using System.ComponentModel.DataAnnotations;

namespace DoAnSPA.Models
{
    public class LienHe
    {
        public int LienHeId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập chủ đề.")]
        public string ChuDe { get; set; } = string.Empty; // ✅ Thêm thuộc tính ChuDe

        [Required(ErrorMessage = "Vui lòng nhập nội dung liên hệ.")]
        public string NoiDung { get; set; } = string.Empty;

        public DateTime NgayGui { get; set; } = DateTime.Now;
    }
}
