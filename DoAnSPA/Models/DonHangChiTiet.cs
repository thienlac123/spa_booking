using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnSPA.Models
{
    public class DonHangChiTiet
    {
        public int DonHangChiTietId { get; set; }

        [ForeignKey(nameof(DonHang))]
        public int DonHangId { get; set; }
        public DonHang DonHang { get; set; } = null!;

        [ForeignKey(nameof(SanPham))]
        public int SanPhamId { get; set; }
        public SanPham SanPham { get; set; } = null!;

        public int SoLuong { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ThanhTien { get; set; }
    }
}
