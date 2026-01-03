using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnSPA.Models
{
    public class DonHang
    {
        public int DonHangId { get; set; }
        public string CustomerId { get; set; } = null!;
        public SpaUser Customer { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        // ChoThanhToan, DaThanhToan, ThatBai, Huy, ChoXacNhan
        [StringLength(30)]
        public string TrangThai { get; set; } = "ChoThanhToan";

        // "MOMO", "VNPAY", "TIENMAT", 
        [StringLength(20)]
        public string PhuongThucThanhToan { get; set; } = "TIENMAT";

        // ==== THÔNG TIN GIAO HÀNG ====
        [Required, StringLength(150)]
        public string TenNguoiNhan { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string SoDienThoaiNhan { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string DiaChiNhan { get; set; } = string.Empty;

        [StringLength(500)]
        public string? GhiChu { get; set; }

        public virtual ICollection<DonHangChiTiet> ChiTiets { get; set; }

        public DonHang()
        {
            ChiTiets = new HashSet<DonHangChiTiet>();
        }


        // MoMo
        [StringLength(50)] public string? MoMoOrderId { get; set; }
        [StringLength(50)] public string? MoMoRequestId { get; set; }
        public long? MoMoTransId { get; set; }
        [StringLength(10)] public string? MoMoErrorCode { get; set; }
        [StringLength(500)] public string? MoMoPayUrl { get; set; }

        // VNPAY
        [StringLength(50)] public string? VnPayTxnRef { get; set; }   // mã đơn phía VNPAY
        [StringLength(10)] public string? VnPayResponseCode { get; set; }
        [StringLength(2048)] public string? VnPayPayUrl { get; set; }
    }

}
