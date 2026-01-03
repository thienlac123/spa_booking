using DoAnSPA.Data;
using DoAnSPA.Models;
using DoAnSPA.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nethereum.Util;
using System.Globalization;
using System.Security.Claims;

public class KhuyenMaiController : Controller
{
    private readonly SpaDbContext _context;

    // tỉ lệ cọc & tỉ giá demo giống bên dịch vụ
    private const decimal DEPOSIT_RATE = 0.5m;          
    private const decimal ETH_RATE_VND = 65_000_000m;    // 1 ETH ≈ 65m VND (demo Hardhat)

    public KhuyenMaiController(SpaDbContext context) => _context = context;
    public class ClientCancelDto
    {
        public int id { get; set; }      // lichHenId
        public string? tx { get; set; }  // hash giao dịch hoàn cọc
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> MarkClientCancel([FromBody] ClientCancelDto body)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (body == null) return BadRequest(new { message = "Payload rỗng." });

        var l = await _context.LichHens.FindAsync(body.id);
        if (l == null) return NotFound(new { message = "Không tìm thấy lịch." });

        // Chỉ chủ lịch mới được hủy
        if (l.customerId != userId)
            return Forbid();

        if (l.trangThai != "DaCoc")
            return BadRequest(new { message = "Trạng thái không cho phép hủy." });

        l.trangThai = "DaHuy";

        if (!string.IsNullOrWhiteSpace(body.tx))
        {
            // nếu bạn có cột CompletedTxHash:
            l.CompletedTxHash = body.tx;
            // hoặc ghi thêm vào ghi chú:
            // l.ghiChu = ((l.ghiChu ?? "") + $" [RefundTx:{body.tx}]").Trim();
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Đã huỷ lịch & ghi nhận hoàn cọc." });
    }


    // ================== DANH SÁCH / CHI TIẾT COMBO ==================

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var combos = await _context.ComboDichVus
            .Include(c => c.DichVus)
            .OrderBy(c => c.TenCombo)
            .ToListAsync();

        return View(combos);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var combo = await _context.ComboDichVus
            .Include(c => c.DichVus)
            .FirstOrDefaultAsync(c => c.ComboDichVuId == id);

        if (combo == null) return NotFound();
        return View(combo);
    }

    // ================== ĐẶT LỊCH COMBO – GET ==================

    [HttpGet]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> DatLichKhuyenMai(int id)
    {
        var combo = await _context.ComboDichVus
            .Include(c => c.DichVus)
            .FirstOrDefaultAsync(c => c.ComboDichVuId == id);
        if (combo == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var profile = userId == null
            ? null
            : await _context.CustomerProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

        // ----- TIỀN CỌC 50% -----
        var fullPrice = combo.GiaKhuyenMai ;
        var depositVnd = Math.Round(fullPrice * 0.5m, 0);
        // Tỉ giá ETH giả sử:
        const decimal rateVndPerEth = 25_000_000m;   // thay bằng tỉ giá của bạn
        var depositEth = depositVnd / rateVndPerEth;

        ViewBag.PriceVnd = fullPrice;
        ViewBag.PriceEth = (fullPrice / rateVndPerEth).ToString("0.######",
                                System.Globalization.CultureInfo.InvariantCulture);
        ViewBag.DepositVnd = depositVnd.ToString("#,0",
                                new System.Globalization.CultureInfo("vi-VN"));
        ViewBag.DepositEth = depositEth.ToString("0.######",
                                System.Globalization.CultureInfo.InvariantCulture);

        // ----- TÍNH CANCEL / GRACE (ví dụ: được hủy trước 2h, grace 1h) -----
        var defaultDate = DateTime.Today.AddDays(1).Date;
        var defaultTime = new TimeSpan(10, 0, 0);
        var start = defaultDate.Add(defaultTime);

        var cancelBefore = start.AddHours(-2);
        var graceUntil = start.AddHours(1);

        var cancelUnix = new DateTimeOffset(cancelBefore).ToUnixTimeSeconds();
        var graceUnix = new DateTimeOffset(graceUntil).ToUnixTimeSeconds();

        ViewBag.CancelBefore = cancelUnix;
        ViewBag.GraceUntil = graceUnix;
        ViewBag.CancelBeforeHuman = cancelBefore.ToString("dd/MM/yyyy HH:mm");
        ViewBag.GraceUntilHuman = graceUntil.ToString("dd/MM/yyyy HH:mm");

        // ----- Tạo BlockchainId (bytes32 hex) -----
        var seed = $"COMBO-{combo.ComboDichVuId}-{Guid.NewGuid()}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
        var sha3 = new Nethereum.Util.Sha3Keccack();
        var idNoPrefix = sha3.CalculateHash(seed);      // 64 hex chars
        ViewBag.BlockchainId = "0x" + idNoPrefix;

        // Ví Spa = Account #0 hardhat
        ViewBag.SpaAddress = "0xf39Fd6e51aad88F6F4ce6aB8827279cffFb92266";

        var vm = new DatLichComboViewModel
        {
            ComboDichVuId = combo.ComboDichVuId,
            TenCombo = combo.TenCombo,
            GiaKhuyenMai = combo.GiaKhuyenMai,
            TenDichVus = combo.DichVus.Select(d => d.TenDichVu).ToList(),
            NgayHen = defaultDate,
            GioHen = defaultTime,
            NhanViens = await _context.NhanViens
                            .Select(nv => new SelectListItem { Value = nv.Id.ToString(), Text = nv.HoTen })
                            .ToListAsync(),
            TenNguoiDat = profile?.FullName ?? "",
            SoDienThoai = profile?.Phone ?? ""
        };
        PrepareWeb3ForCombo(vm, combo);

        return View(vm);
    }



    // ================== HÀM CHUẨN BỊ VIEWBAG WEB3 (GIỐNG BÊN DỊCH VỤ) ==================

    private void PrepareWeb3ForCombo(DatLichComboViewModel vm, ComboDichVu combo)
    {
        decimal giaCombo = combo.GiaKhuyenMai ;
        var depositVnd = decimal.Round(giaCombo , 0, MidpointRounding.AwayFromZero);
        var depositEth = depositVnd / ETH_RATE_VND;

        ViewBag.PriceVnd = depositVnd;
        ViewBag.PriceEth = depositEth.ToString("0.########", CultureInfo.InvariantCulture);

        // Tạo ID booking 0x... (bytes32) nếu chưa có
        if (string.IsNullOrEmpty(vm.BlockchainId))
        {
            var raw = $"COMBO-{Guid.NewGuid():N}";
            var sha3 = new Sha3Keccack();
            var hexNoPrefix = sha3.CalculateHash(raw);
            vm.BlockchainId = "0x" + hexNoPrefix;
        }
        ViewBag.BlockchainId = vm.BlockchainId;

        // tính mốc hủy / grace (2h trước & 1h sau giờ hẹn)
        var targetLocal = vm.NgayHen.Date + vm.GioHen;
        var target = DateTime.SpecifyKind(targetLocal, DateTimeKind.Local);
        var targetUnix = new DateTimeOffset(target).ToUnixTimeSeconds();

        long cancelBefore = targetUnix - 2 * 3600; // 2 tiếng trước giờ hẹn
        long graceUntil = targetUnix + 1 * 3600; // 1 tiếng sau giờ hẹn

        ViewBag.CancelBefore = cancelBefore;
        ViewBag.GraceUntil = graceUntil;

        // Ví Spa (Account #0 Hardhat)
        ViewBag.SpaAddress = "0xf39Fd6e51aad88F6F4ce6aB8827279cffFb92266";
    }

    // ================== ĐẶT LỊCH COMBO – POST (sau khi đã cọc) ==================

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> XacNhanDatLichKhuyenMai(DatLichComboViewModel model)
    {
        // bỏ validate các field chỉ hiển thị
        ModelState.Remove(nameof(model.TenCombo));
        ModelState.Remove(nameof(model.TenDichVus));
        ModelState.Remove(nameof(model.GiaKhuyenMai));

        var combo = await _context.ComboDichVus
            .Include(c => c.DichVus)
            .FirstOrDefaultAsync(c => c.ComboDichVuId == model.ComboDichVuId);

        if (combo == null)
        {
            ModelState.AddModelError(string.Empty, "Combo không tồn tại.");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Forbid();

        if (model.NgayHen.Date < DateTime.Today)
            ModelState.AddModelError(nameof(model.NgayHen), "Ngày hẹn phải từ hôm nay trở đi.");

        // 1) KH có lịch trùng slot không?
        if (ModelState.IsValid)
        {
            bool userSlotTaken = await _context.LichHens.AnyAsync(x =>
                x.customerId == userId &&
                x.ngayHen == model.NgayHen &&
                x.gioHen == model.GioHen &&
                x.trangThai != "DaHuy");

            if (userSlotTaken)
                ModelState.AddModelError(string.Empty, "Bạn đã có lịch ở khung giờ này.");
        }

        // 2) Nhân viên trùng lịch?
        if (ModelState.IsValid && model.NhanVienId.HasValue)
        {
            bool staffBusy = await _context.LichHens.AnyAsync(x =>
                x.nhanVienId == model.NhanVienId &&
                x.ngayHen == model.NgayHen &&
                x.gioHen == model.GioHen &&
                x.trangThai != "DaHuy");

            if (staffBusy)
                ModelState.AddModelError(string.Empty, "Nhân viên đã có lịch ở khung giờ này.");
        }

        if (!ModelState.IsValid || combo == null)
        {
            // nạp lại data & Web3
            model.NhanViens = await _context.NhanViens
                .Select(nv => new SelectListItem
                {
                    Value = nv.Id.ToString(),
                    Text = nv.HoTen
                }).ToListAsync();

            if (combo != null)
            {
                model.TenCombo = combo.TenCombo;
                model.GiaKhuyenMai = combo.GiaKhuyenMai;
                model.TenDichVus = combo.DichVus.Select(d => d.TenDichVu).ToList();
            }

            PrepareWeb3ForCombo(model, combo!);
            return View("DatLichKhuyenMai", model);
        }

        // ======= Đến đây là OK, tạo lịch =======

        var profile = await _context.CustomerProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        var lich = new LichHen
        {
            dichVuId = null,
            comboId = combo.ComboDichVuId,
            nhanVienId = model.NhanVienId,

            ngayHen = model.NgayHen,
            gioHen = model.GioHen,
            ghiChu = model.GhiChu,

            trangThai = "DaCoc",       // đã cọc Web3
            khachHangId = 0,
            customerId = userId,

            TenNguoiDat = model.TenNguoiDat ?? profile?.FullName,
            SoDienThoai = model.SoDienThoai ?? profile?.Phone,

            // blockchain
            BlockchainId = model.BlockchainId,
            TransactionHash = model.TransactionHash,
        };

        // Tính & lưu CancelBefore / GraceUntil giống bên dịch vụ
        var targetLocal = model.NgayHen.Date + model.GioHen;
        var target = DateTime.SpecifyKind(targetLocal, DateTimeKind.Local);
        var targetUnix = new DateTimeOffset(target).ToUnixTimeSeconds();
        lich.CancelBefore = targetUnix - 2 * 3600; // 2h trước
        lich.GraceUntil = targetUnix + 1 * 3600; // 1h sau

        _context.LichHens.Add(lich);
        await _context.SaveChangesAsync();

        TempData["AlertMsg"] = "Đặt lịch khuyến mãi & đặt cọc thành công!";
        TempData["AlertLevel"] = "success";

        return RedirectToAction(nameof(Success), new { id = lich.lichHenId });
    }

    // ================== SUCCESS ==================

    [HttpGet]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Success(int id)
    {
        var lich = await _context.LichHens
            .Include(l => l.ComboDichVu)
            .Include(l => l.DichVu)
            .Include(l => l.NhanVien)
            .FirstOrDefaultAsync(l => l.lichHenId == id);

        if (lich == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (lich.customerId != userId &&
            !User.IsInRole("Admin") &&
            !User.IsInRole("SpaOwner"))
            return Forbid();

        return View(lich);
    }
}
