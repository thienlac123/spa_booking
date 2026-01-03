using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnSPA.Models
{
    public class SanPham
    {
        public int SanPhamId { get; set; }

        [Required, StringLength(150)]
        public string TenSanPham { get; set; } = string.Empty;

        [StringLength(500)]
        public string? MoTa { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Gia { get; set; }

        [StringLength(255)]
        public string? HinhAnhUrl { get; set; }

        public bool IsActive { get; set; } = true;
        public string LoaiSanPham { get; set; }

    }

}
