using System;
using System.ComponentModel.DataAnnotations;

namespace DoAnSPA.Models.ViewModels
{
    public class LichHenEditTrangThaiViewModel
    {
        [Required]
        public int LichHenId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        public string TrangThai { get; set; }

        public string? TenNguoiDat { get; set; }
        public string? SoDienThoai { get; set; }

        // Thêm các thuộc tính bạn muốn hiển thị
        public string? TenDichVu { get; set; }
        public DateTime NgayHen { get; set; }
        public TimeSpan GioHen { get; set; }
        public string? GhiChu { get; set; }
        public string? TenNhanVien { get; set; } // Có thể null
        // Bạn có thể thêm các thông tin khác nếu cần
    }
}