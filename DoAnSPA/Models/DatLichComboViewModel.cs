using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAnSPA.Models.ViewModels
{
    public class DatLichComboViewModel
    {
        public int ComboDichVuId { get; set; }

        // Hiển thị
        public string? TenCombo { get; set; }
        public decimal? GiaKhuyenMai { get; set; }
        public List<string>? TenDichVus { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày hẹn")]
        public DateTime NgayHen { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Giờ hẹn")]
        public TimeSpan GioHen { get; set; }

        [Display(Name = "Nhân viên")]
        public int? NhanVienId { get; set; }
        public List<SelectListItem>? NhanViens { get; set; }

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Display(Name = "Tên người đặt")]
        public string? TenNguoiDat { get; set; }

        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        // ==== NEW: blockchain fields cho combo ====
        public string? BlockchainId { get; set; }     // bytes32 (dưới dạng hex string 0x...)
        public string? TransactionHash { get; set; }  // hash giao dịch đặt cọc
       
        public long? CancelBefore { get; set; }   
        public long? GraceUntil { get; set; }     
    }
}
