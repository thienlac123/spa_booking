namespace DoAnSPA.Models
{
    public class PhanHoi
    {
       
            public int phanHoiId { get; set; }
            public int khachHangId { get; set; }
            public int dichVuId { get; set; }
            public int diemDanhGia { get; set; } // 1-5 sao
            public string noiDung { get; set; }
            public DateTime ngayDanhGia { get; set; }
        

    }
}
