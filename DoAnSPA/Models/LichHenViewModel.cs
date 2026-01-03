using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoAnSPA.Models
{
    public class LichHenViewModel
    {
        public int DichVuId { get; set; }
        public int? ComboId { get; set; }


        public int? NhanVienId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày hẹn")]
        public DateTime NgayHen { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giờ hẹn")]
        public TimeSpan GioHen { get; set; }

        public string? GhiChu { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên người đặt")]
        [StringLength(255)]
        public string TenNguoiDat { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20)]
        public string SoDienThoai { get; set; }

        // Dữ liệu hiển thị
        public string? TenDichVu { get; set; }

        public string? TenCombo { get; set; }

        public List<SelectListItem>? NhanViens { get; set; }
        public string? TransactionHash { get; set; }
        public string? BlockchainId { get; set; } // Thêm cái này
        public long? CancelBefore { get; set; }   // Thêm cái này
        public long? GraceUntil { get; set; }     // Thêm cái này
    }
}
