using System.ComponentModel.DataAnnotations;

namespace DoAnSPA.Models.ViewModels
{
    public class DatLichViewModel
    {
        // Truyền ĐÚNG 1 trong 2:
        public int? DichVuId { get; set; }     // đặt dịch vụ lẻ
        public int? ComboId { get; set; }     // đặt combo

        // ---- Thông tin hiển thị trên form (read-only ở View) ----
        [Display(Name = "Tên gói/dịch vụ")]
        [Required] public string Ten { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }      // để nullable hợp lý hơn

        [Display(Name = "Giá")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá không hợp lệ")]
        public decimal Gia { get; set; }       // 0 => “Liên hệ” khi render

        // Nếu là combo, có thể show danh sách dịch vụ con
        public List<string>? TenDichVus { get; set; }

        // ---- Dữ liệu người dùng nhập ----
        [Required(ErrorMessage = "Vui lòng chọn ngày hẹn")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày hẹn")]
        public DateTime NgayHen { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giờ hẹn")]
        [DataType(DataType.Time)]
        [Display(Name = "Giờ hẹn")]
        public TimeSpan GioHen { get; set; }

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        // tiện dụng cho controller/view
        public bool IsCombo => ComboId.HasValue && ComboId > 0;
    }
}
