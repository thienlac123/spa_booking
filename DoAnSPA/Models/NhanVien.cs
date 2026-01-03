using System.ComponentModel.DataAnnotations;

namespace DoAnSPA.Models
{
    public class NhanVien
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string HoTen { get; set; }

        [StringLength(200)]
        public string QueQuan { get; set; }

        [Required, StringLength(15)]
        public string SoDienThoai { get; set; }

        [StringLength(100)]
        public string ViTri { get; set; }

        [StringLength(200)]
        public string Facebook { get; set; }

        [StringLength(200)]
        public string Twitter { get; set; }

        [StringLength(200)]
        public string LinkedIn { get; set; }

        [StringLength(200)]
        public string Instagram { get; set; }

        [StringLength(255)]
        public string AnhNhanVien { get; set; } // Ảnh nhân viên (đường dẫn ảnh)
        public bool HienThiTrangChu { get; set; } // 🔹 Thuộc tính mới để hiển thị trên trang chủ

    }
}
