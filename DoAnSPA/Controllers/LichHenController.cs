using DoAnSPA.Data;
using DoAnSPA.Models;
using DoAnSPA.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Globalization; // Để format số tiền
using Nethereum.Util;       // Cần cài NuGet: Nethereum.Web3

namespace DoAnSPA.Controllers
{
    [Authorize(Roles = "Customer")]
    public class LichHenController : Controller
    {
        private readonly SpaDbContext _context;
        public LichHenController(SpaDbContext context) => _context = context;

        // =========================================================================
        // 1. HÀM PHỤ TRỢ (Giúp code gọn gàng, không bị lặp lại)
        // =========================================================================
        private void ReloadViewBags(LichHenViewModel vm)
        {
            // Nạp lại danh sách nhân viên
            vm.NhanViens = _context.NhanViens
                .Select(nv => new SelectListItem { Value = nv.Id.ToString(), Text = nv.HoTen })
                .ToList();

            // Nạp lại tên dịch vụ nếu bị mất
            if (string.IsNullOrEmpty(vm.TenDichVu))
            {
                vm.TenDichVu = _context.DichVus
                    .Where(d => d.DichVuId == vm.DichVuId)
                    .Select(d => d.TenDichVu)
                    .FirstOrDefault() ?? "";
            }
        }

        // =========================================================================
        // 2. GET: Hiển thị Form (Tính toán giá & Hash ID cho Blockchain)
        // =========================================================================
        [HttpGet]
        public IActionResult Create(int dichVuId)
        {
            var dv = _context.DichVus.AsNoTracking().FirstOrDefault(d => d.DichVuId == dichVuId);
            if (dv == null) return NotFound("Không tìm thấy dịch vụ.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = string.IsNullOrEmpty(userId) ? null : _context.CustomerProfiles.AsNoTracking().FirstOrDefault(p => p.UserId == userId);

            var vm = new LichHenViewModel
            {
                DichVuId = dv.DichVuId,
                TenDichVu = dv.TenDichVu,
                NgayHen = DateTime.Today.AddDays(1),
                GioHen = new TimeSpan(10, 0, 0),
                TenNguoiDat = profile?.FullName ?? "",
                SoDienThoai = profile?.Phone ?? ""
            };
            // ... (Phần lấy dv và userId giữ nguyên) ...

            // --- [LOGIC BLOCKCHAIN BẮT ĐẦU] ---

            // 1. TÍNH GIÁ CỌC THEO 50% GIÁ DỊCH VỤ

            decimal giaGoc = dv.Gia; // Lấy giá gốc, nếu null thì bằng 0
            decimal priceVnd = giaGoc * 0.5m;               // Nhân 0.5 để lấy 50%

            // Tỷ giá giả định: 1 ETH = 65 triệu VND
            decimal exchangeRate = 65000000;
            decimal priceEth = priceVnd / exchangeRate;

            // Format số ETH thành chuỗi chuẩn (dấu chấm)
            string priceEthStr = priceEth.ToString("0.00000000", CultureInfo.InvariantCulture);

            // 2. Tạo ID duy nhất cho giao dịch
            string tempOrderCode = "BOOK-" + DateTime.Now.Ticks;

            // 3. Băm ID sang dạng Hex (bytes32)
            var sha3 = new Sha3Keccack();
            string blockchainId = sha3.CalculateHash(tempOrderCode);

            // 4. Truyền dữ liệu sang View
          
            ViewBag.PriceVnd = priceVnd.ToString("N0");
            ViewBag.PriceEth = priceEthStr;
            ViewBag.BlockchainId = blockchainId;

            // Ví chủ Spa (Account #0 Hardhat)
            ViewBag.SpaAddress = "0xf39Fd6e51aad88F6F4ce6aB8827279cffFb92266";

            // 5. Tính thời gian
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            ViewBag.CancelBefore = now + 7200;
            ViewBag.GraceUntil = now + (3 * 3600);

            // --- [LOGIC BLOCKCHAIN KẾT THÚC] ---

            ReloadViewBags(vm);
            return View(vm);

            // =========================================================================
            // 3. POST: Xử lý lưu Database (Nhận TransactionHash từ View)
            // =========================================================================
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LichHenViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Forbid();

            // Chuẩn hóa giờ hẹn
            model.GioHen = new TimeSpan(model.GioHen.Hours, model.GioHen.Minutes, 0);

            // Validate dữ liệu
            if (model.NgayHen.Date < DateTime.Today)
                ModelState.AddModelError(nameof(model.NgayHen), "Ngày hẹn phải từ hôm nay trở đi.");

            if (!ModelState.IsValid)
            {
                ReloadViewBags(model);
                TempData["AlertMsg"] = "Vui lòng kiểm tra lại thông tin đặt lịch.";
                TempData["AlertLevel"] = "warning";
                return View(model);
            }

            // 1. Check trùng lịch của Khách
            bool userSlotTaken = await _context.LichHens.AnyAsync(x =>
                x.customerId == userId &&
                x.ngayHen == model.NgayHen &&
                x.gioHen == model.GioHen &&
                x.trangThai != "DaHuy");

            if (userSlotTaken)
            {
                ReloadViewBags(model);
                TempData["AlertMsg"] = "Bạn đã có lịch hẹn vào khung giờ này.";
                TempData["AlertLevel"] = "danger";
                return View(model);
            }

            // 2. Check trùng lịch Nhân viên (nếu có chọn)
            if (model.NhanVienId.HasValue)
            {
                bool staffBusy = await _context.LichHens.AnyAsync(x =>
                    x.nhanVienId == model.NhanVienId &&
                    x.ngayHen == model.NgayHen &&
                    x.gioHen == model.GioHen &&
                    x.trangThai != "DaHuy");

                if (staffBusy)
                {
                    ReloadViewBags(model);
                    TempData["AlertMsg"] = "Khung giờ này nhân viên đã bận. Vui lòng chọn giờ khác.";
                    TempData["AlertLevel"] = "danger";
                    return View(model);
                }
            }

            // 3. Tạo Entity để lưu (BỔ SUNG CÁC CỘT BLOCKCHAIN Ở ĐÂY)
            var lichHen = new LichHen
            {
                dichVuId = model.DichVuId,
                nhanVienId = model.NhanVienId,
                ngayHen = model.NgayHen,
                gioHen = model.GioHen,
                ghiChu = model.GhiChu,
                khachHangId = 0,
                customerId = userId,
                TenNguoiDat = model.TenNguoiDat,
                SoDienThoai = model.SoDienThoai,

                // === [QUAN TRỌNG: LƯU THÔNG TIN BLOCKCHAIN] ===
                TransactionHash = model.TransactionHash, // Mã giao dịch
                BlockchainId = model.BlockchainId,    // ID của cái két sắt (0x...)
                CancelBefore = model.CancelBefore,    // Mốc thời gian hủy
                GraceUntil = model.GraceUntil       // Mốc thời gian phạt
            };

            // --- [XỬ LÝ TRẠNG THÁI] ---
            if (!string.IsNullOrEmpty(model.TransactionHash))
            {
                // Có hash nghĩa là đã trả tiền qua MetaMask
                lichHen.trangThai = "DaCoc";
                TempData["AlertMsg"] = "Đặt lịch và Thanh toán cọc thành công!";
            }
            else
            {
                // Chưa trả tiền
                lichHen.trangThai = "ChoXacNhan";
                TempData["AlertMsg"] = "Đặt lịch thành công (Chờ xác nhận)!";
            }

            // 4. Lưu xuống Database (Try-Catch an toàn)
            try
            {
                _context.LichHens.Add(lichHen);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ReloadViewBags(model);
                TempData["AlertMsg"] = "Lỗi hệ thống: Không thể lưu lịch hẹn. Vui lòng thử lại.";
                TempData["AlertLevel"] = "danger";
                return View(model);
            }

            TempData["AlertLevel"] = "success";
            return RedirectToAction(nameof(Success), new { id = lichHen.lichHenId });
        }

        // =========================================================================
        // 4. Các Action khác (Giữ nguyên)
        // =========================================================================
        public async Task<IActionResult> Success(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lichHen = await _context.LichHens
                .Include(l => l.DichVu)
                .Include(l => l.NhanVien)
                .FirstOrDefaultAsync(l => l.lichHenId == id);

            if (lichHen == null) return NotFound();
            if (lichHen.customerId != userId && !User.IsInRole("Admin") && !User.IsInRole("SpaOwner"))
                return Forbid();

           
            var thoiGianHen = lichHen.ngayHen.Date.Add(lichHen.gioHen);

            // Nếu gioHen là DateTime full thì dùng: var thoiGianHen = lichHen.gioHen;

            var cancelDeadline = thoiGianHen.AddHours(-2);

            // Truyền sang View bằng ViewBag
            ViewBag.CancelDeadline = cancelDeadline;

            return View(lichHen);
        }


        public IActionResult DaDat()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Forbid();

            var lichHens = _context.LichHens
                .Include(l => l.DichVu)
                .Include(l => l.ComboDichVu)
                .Include(l => l.NhanVien)
                .Where(l => l.customerId == userId)
                .OrderByDescending(l => l.ngayHen)
                .ThenByDescending(l => l.gioHen)
                .ToList();

            return View(lichHens);
        }
        // DTO dùng chung cho cả Admin & Client
        public class OnchainUpdateDto
        {
            public int id { get; set; }
            public string? status { get; set; }  // Released / Claimed / Refunded
            public string? tx { get; set; }
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> MarkClientCancel([FromBody] OnchainUpdateDto body)
        {
            if (body == null) return BadRequest(new { message = "Payload rỗng" });

            var lich = await _context.LichHens.FindAsync(body.id);
            if (lich == null) return NotFound(new { message = "Không tìm thấy lịch" });

            if (body.status != "Refunded")
                return BadRequest(new { message = "Trạng thái không hợp lệ" });

            lich.trangThai = "DaHuy";
            lich.LyDoHuy = "KhachHuy";
            if (!string.IsNullOrWhiteSpace(body.tx))
                lich.CompletedTxHash = body.tx;

            await _context.SaveChangesAsync();
            // Để hiển thị alert trên DaDat / Success
            TempData["AlertMsg"] = "Hủy lịch & hoàn cọc thành công. Vui lòng kiểm tra số dư ví MetaMask.";
            TempData["AlertLevel"] = "success";
            return Ok(new { message = "Đã cập nhật trạng thái hủy." });
        }

      


    }
}