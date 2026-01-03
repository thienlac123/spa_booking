using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnSPA.Models
{
    public class ComboDichVu
    {
        [Key]
        public int ComboDichVuId { get; set; }

        [Required, StringLength(150)]
        public string TenCombo { get; set; } = string.Empty;

        // Many-to-many
        public List<DichVu> DichVus { get; set; } = new();

        // Giá KM lưu DB
        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaKhuyenMai { get; set; }

        // ===== Thuộc tính tính toán (không map DB) =====
        [NotMapped]
        public decimal GiaGoc => DichVus?.Sum(d => d.Gia) ?? 0m;

        [NotMapped]
        public decimal PhanTramGiam
        {
            get
            {
                if (GiaGoc <= 0) return 0m;
                var pct = (1 - (GiaKhuyenMai / GiaGoc)) * 100m;
                if (pct < 0) pct = 0;
                if (pct > 100) pct = 100;
                return decimal.Round(pct, 2);
            }
        }
    }
}
