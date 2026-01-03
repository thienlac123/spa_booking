namespace DoAnSPA.Models
{
    public class DichVu
    {
        public int DichVuId { get; set; }
        public string TenDichVu { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public decimal Gia { get; set; }
        public string? HinhAnh { get; set; } // Đường dẫn hoặc tên file ảnh

        // ⚠️ Thêm danh sách Combos để tạo quan hệ nhiều-nhiều
        public List<ComboDichVu> Combos { get; set; } = new List<ComboDichVu>();
        public bool HienThiTrangChu { get; set; } = false;

    }
}
