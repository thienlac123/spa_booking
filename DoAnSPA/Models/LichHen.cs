using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// (tùy chọn) để dùng [Index]:
// using Microsoft.EntityFrameworkCore;

namespace DoAnSPA.Models
{
    public class LichHen
    {
        public int lichHenId { get; set; }

        // Tương thích code cũ (không dùng nữa trong Identity)
        public int khachHangId { get; set; }

        [Required, StringLength(255)]
        public string TenNguoiDat { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string SoDienThoai { get; set; } = string.Empty;

        // ===== CHỌN 1 TRONG 2: DỊCH VỤ HOẶC COMBO =====

        // Dịch vụ lẻ
        public int? dichVuId { get; set; }
        public virtual DichVu? DichVu { get; set; }


        public int? comboId { get; set; }
        [ForeignKey(nameof(comboId))]
        public virtual ComboDichVu? ComboDichVu { get; set; }
        // Nhân viên
        public int? nhanVienId { get; set; }
        public virtual NhanVien? NhanVien { get; set; }

        [Required]
        public DateTime ngayHen { get; set; }

        [Required]
        public TimeSpan gioHen { get; set; }

        [StringLength(1000)]
        public string? ghiChu { get; set; }

        [Required, StringLength(30)]
        public string trangThai { get; set; } = "ChoXacNhan";

        public string? LyDoHuy { get; set; }

        // ===== Identity (SpaUser) =====
        [ForeignKey(nameof(Customer))]
        public string? customerId { get; set; }      // ⚠️ tên khác quy ước => thêm [ForeignKey]
        public virtual SpaUser? Customer { get; set; }

        // ===== Blockchain (nếu dùng) =====
        [Column(TypeName = "varbinary(32)")]
        public byte[]? bookingHash { get; set; }

        [MaxLength(66)]
        public string? onChainCreateTx { get; set; }

        [MaxLength(66)]
        public string? onChainCompleteTx { get; set; }
        public string? TransactionHash { get; set; }
        [StringLength(100)]
        public string? BlockchainId { get; set; }    // Quan trọng: ID của cái két sắt (0x...)

        public long? CancelBefore { get; set; }      // Mốc thời gian hủy (Unix Timestamp)
        public long? GraceUntil { get; set; }
        public string? CompletedTxHash { get; set; } // Mã giao dịch lúc hoàn thành/lấy tiền
    }
}
