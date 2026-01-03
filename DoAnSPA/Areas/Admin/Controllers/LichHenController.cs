using DoAnSPA.Data;
using DoAnSPA.Models;
using DoAnSPA.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnSPA.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SpaOwner")]
    public class LichHenController : Controller
    {
        private readonly SpaDbContext _context;

        public LichHenController(SpaDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. INDEX: danh sách Lịch hẹn (dịch vụ + combo)
        // ============================================================
        public async Task<IActionResult> Index(
     DateTime? ngayBatDau,
     DateTime? ngayKetThuc,
     string? status
 )
        {
            // 1. Query cơ bản: luôn Include cả DichVu & ComboDichVu
            var query = _context.LichHens
                .Include(l => l.DichVu)
                .Include(l => l.ComboDichVu)
                .Include(l => l.NhanVien)
                .AsSplitQuery()
                .AsQueryable();

            // 2. Lọc theo ngày
            if (ngayBatDau.HasValue)
                query = query.Where(l => l.ngayHen >= ngayBatDau.Value.Date);

            if (ngayKetThuc.HasValue)
                query = query.Where(l => l.ngayHen <= ngayKetThuc.Value.Date);

            // 3. Lọc theo trạng thái (nếu có)
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(l => l.trangThai == status);

            // 4. Lấy data, ưu tiên đơn "Đã cọc" lên trên
            var list = await query
                .OrderByDescending(l => l.trangThai == "DaCoc")
                .ThenByDescending(l => l.ngayHen)
                .ThenByDescending(l => l.gioHen)
                .ToListAsync();

            // 4.1. **Backup**: tự gán ComboDichVu nếu vì lý do gì đó Include không bơm dữ liệu
            var comboIds = list
                .Where(l => l.comboId.HasValue)
                .Select(l => l.comboId.Value)
                .Distinct()
                .ToList();

            if (comboIds.Any())
            {
                var comboMap = await _context.ComboDichVus
                    .Where(c => comboIds.Contains(c.ComboDichVuId))
                    .ToDictionaryAsync(c => c.ComboDichVuId);

                foreach (var l in list)
                {
                    if (l.comboId.HasValue &&
                        (l.ComboDichVu == null) &&
                        comboMap.TryGetValue(l.comboId.Value, out var combo))
                    {
                        l.ComboDichVu = combo;
                    }
                }
            }

            // 5. Thống kê theo trạng thái từ dữ liệu đã lọc
            var stats = list
                .GroupBy(l => l.trangThai ?? "")
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );

            int getCount(string key) => stats.ContainsKey(key) ? stats[key] : 0;

            ViewBag.ThongKeTrangThai = new Dictionary<string, int>
    {
        { "ChoXacNhan", getCount("ChoXacNhan") },
        { "DaCoc",       getCount("DaCoc") },
        { "HoanThanh",   getCount("HoanThanh") },
        { "DaHuy",       getCount("DaHuy") },
        { "DaPhat",      getCount("DaPhat") }
    };

            ViewBag.NgayBatDau = ngayBatDau?.ToString("yyyy-MM-dd");
            ViewBag.NgayKetThuc = ngayKetThuc?.ToString("yyyy-MM-dd");
            ViewBag.FilterStatus = status;

            // 6. Map ID -> BlockchainId để dùng cho nút Web3
            var hashes = new Dictionary<int, string>();
            foreach (var l in list)
            {
                if (!string.IsNullOrWhiteSpace(l.BlockchainId))
                {
                    var hex = l.BlockchainId.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                        ? l.BlockchainId
                        : "0x" + l.BlockchainId;

                    hashes[l.lichHenId] = hex;
                }
            }
            ViewBag.BookingHashes = hashes;

            // Map comboId -> info combo cho View
            var comboLookup = await _context.ComboDichVus
      .Select(c => new
      {
          c.ComboDichVuId,
          c.TenCombo,
          c.GiaKhuyenMai
      })
      .ToDictionaryAsync(c => c.ComboDichVuId, c => c);

            ViewBag.ComboLookup = comboLookup;

            // 7. **Quan trọng**: PHẢI truyền list sang view
            return View("~/Views/Admin/IndexLichHen.cshtml", list);
        }


        // ============================================================
        // 2. API nhận kết quả on-chain (Admin bấm nút Web3)
        // ============================================================
        public class OnchainUpdateDto
        {
            public int id { get; set; }          // lichHenId trong SQL
            public string? status { get; set; }  // "Released" | "Claimed" | "Refunded"
            public string? tx { get; set; }      // transaction hash hoàn tất
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> MarkOnchain([FromBody] OnchainUpdateDto body)
        {
            if (body == null)
                return BadRequest(new { message = "Dữ liệu rỗng." });

            var lh = await _context.LichHens.FindAsync(body.id);
            if (lh == null)
                return NotFound(new { message = "Không tìm thấy lịch." });

            // Map trạng thái từ Smart Contract -> Database
            switch (body.status)
            {
                case "Released":     // confirmService -> tiền về ví Spa
                    lh.trangThai = "HoanThanh";
                    break;
                case "Claimed":      // claimNoShow -> phạt khách, tiền về Spa
                    lh.trangThai = "DaPhat";
                    break;
                case "Refunded":     // cancelBySpa -> hoàn tiền cho khách
                    lh.trangThai = "DaHuy";
                    lh.LyDoHuy = "SpaHuy";
                    break;
                default:
                    return BadRequest(new { message = "Trạng thái không hợp lệ." });
            }

            // Lưu Tx hash hoàn tất (nếu có)
            if (!string.IsNullOrWhiteSpace(body.tx))
            {
                lh.CompletedTxHash = body.tx;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã cập nhật trạng thái on-chain." });
        }

        // ============================================================
        // 3. EDIT thủ công (fallback khi cần chỉnh tay)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var lh = await _context.LichHens
                .Include(x => x.DichVu)
                .Include(x => x.ComboDichVu)
                .Include(x => x.NhanVien)
                .FirstOrDefaultAsync(x => x.lichHenId == id);

            if (lh == null) return NotFound();

            var vm = new LichHenEditTrangThaiViewModel
            {
                LichHenId = lh.lichHenId,
                TrangThai = lh.trangThai,
                TenDichVu = lh.DichVu?.TenDichVu ?? lh.ComboDichVu?.TenCombo,
                NgayHen = lh.ngayHen,
                GioHen = lh.gioHen,
                GhiChu = lh.ghiChu,
                TenNhanVien = lh.NhanVien?.HoTen,
                TenNguoiDat = lh.TenNguoiDat,
                SoDienThoai = lh.SoDienThoai
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LichHenEditTrangThaiViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var lh = await _context.LichHens.FindAsync(model.LichHenId);
            if (lh == null) return NotFound();

            lh.trangThai = model.TrangThai;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật trạng thái thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
