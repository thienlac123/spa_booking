using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnSPA.Models
{
    public class GioHangItem
    {
        public int GioHangItemId { get; set; }

        [ForeignKey(nameof(SanPham))]
        public int SanPhamId { get; set; }
        public SanPham SanPham { get; set; } = null!;

        [ForeignKey(nameof(Customer))]
        public string CustomerId { get; set; } = null!;
        public SpaUser Customer { get; set; } = null!;

        public int SoLuong { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
