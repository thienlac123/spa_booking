using DoAnSPA.Data;
using DoAnSPA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnSPA.Services;
using QRCoder;
using System.Security.Claims;
using System.Linq;


namespace DoAnSPA.Controllers
{
    [Authorize(Roles = "Customer")]
    public class GioHangController : Controller
    {
        private readonly SpaDbContext _context;
        private readonly MoMoService _momo;
        private readonly VnPayService _vnPay;

        public GioHangController(SpaDbContext context, MoMoService momo, VnPayService vnPay)
        {
            _context = context;
            _momo = momo;
            _vnPay = vnPay;
        }

        // Hiển thị giỏ hàng
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var items = await _context.GioHangItems
                .Include(g => g.SanPham)
                .Where(g => g.CustomerId == userId)
                .ToListAsync();

            return View(items);
        }

        // Xóa 1 item khỏi giỏ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = await _context.GioHangItems.FindAsync(id);
            if (item == null || item.CustomerId != userId) return NotFound();

            _context.GioHangItems.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DonHangCuaToi()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var orders = await _context.DonHangs
                .Include(d => d.Customer)
                .Include(d => d.ChiTiets)           
                    .ThenInclude(ct => ct.SanPham)
                .Where(d => d.CustomerId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return View(orders); // Views/GioHang/DonHangCuaToi.cshtml
        }

        // Checkout: tạo đơn + điều hướng theo phương thức thanh toán
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(
               string paymentMethod,
                string tenNguoiNhan,
                string soDienThoaiNhan,
                string diaChiNhan,
                string? ghiChu )


        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // Lấy giỏ hàng hiện tại
            var items = await _context.GioHangItems
                .Include(g => g.SanPham)
                .Where(g => g.CustomerId == userId)
                .ToListAsync();

            if (!items.Any())
            {
                TempData["Error"] = "Giỏ hàng trống.";
                return RedirectToAction(nameof(Index));
            }

            // Tính tổng tiền
            var tongTien = items.Sum(i => i.SanPham.Gia * i.SoLuong);

            // Tạo đơn hàng
            var order = new DonHang
            {
                CustomerId = userId,
                TongTien = tongTien,
                PhuongThucThanhToan = paymentMethod,

                TenNguoiNhan = tenNguoiNhan,
                SoDienThoaiNhan = soDienThoaiNhan,
                DiaChiNhan = diaChiNhan,
                GhiChu = ghiChu,

                // Trạng thái mặc định: Spa đang chuẩn bị hàng
                TrangThai = "ChuanBi",
                CreatedAt = DateTime.UtcNow
            };

            _context.DonHangs.Add(order);
            await _context.SaveChangesAsync(); // để có DonHangId

            // Tạo chi tiết đơn hàng
            foreach (var i in items)
            {
                _context.DonHangChiTiets.Add(new DonHangChiTiet
                {
                    DonHangId = order.DonHangId,
                    SanPhamId = i.SanPhamId,
                    SoLuong = i.SoLuong,
                    DonGia = i.SanPham.Gia,
                    ThanhTien = i.SanPham.Gia * i.SoLuong
                });
            }

            await _context.SaveChangesAsync();

            // Tùy phương thức thanh toán
            switch (paymentMethod)
            {
                case "MOMO":
                    // Bước 2: sẽ implement thật sự
                   
                    return await ThanhToanMomo(order);

                case "VNPAY":
                    // Bước 3: sẽ implement thật sự

                    return ThanhToanVnPay(order);

                case "TIENMAT":
                    // Đơn hàng thanh toán tại Spa, status vẫn là "ChuanBi"
                    _context.GioHangItems.RemoveRange(items);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = $"Đã tạo đơn #{order.DonHangId}. Spa đang chuẩn bị hàng.";
                    return RedirectToAction(nameof(Index));

                default:
                    TempData["Error"] = "Phương thức thanh toán không hợp lệ.";
                    return RedirectToAction(nameof(Index));
            }
        }

        // ====== STUB: sẽ làm ở bước sau ======

        private async Task<IActionResult> ThanhToanMomo(DonHang order)
        {
            try
            {
                var payUrl = await _momo.CreatePaymentUrlAsync(order);

                if (string.IsNullOrEmpty(payUrl))
                {
                    TempData["Error"] = "Không tạo được link thanh toán MoMo (resultCode != 0). Kiểm tra PartnerCode / AccessKey / SecretKey.";
                    return RedirectToAction(nameof(Index));
                }

                order.MoMoPayUrl = payUrl;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ThanhToanMoMo), new { id = order.DonHangId });
            }
            catch (Exception ex)
            {
                // hiển thị lỗi ra để biết lý do (dev)
                TempData["Error"] = "Lỗi MoMo: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> ThanhToanMoMo(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.DonHangs
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.DonHangId == id && o.CustomerId == userId);

            if (order == null) return NotFound();

            return View(order); // Views/GioHang/ThanhToanMoMo.cshtml
        }


        // Trả ảnh QR từ MoMoPayUrl
        [AllowAnonymous]
        public async Task<IActionResult> MoMoQr(int id)
        {
            var order = await _context.DonHangs.FindAsync(id);
            if (order == null || string.IsNullOrEmpty(order.MoMoPayUrl))
                return NotFound();

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(order.MoMoPayUrl, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrBytes = qrCode.GetGraphic(20);

            return File(qrBytes, "image/png");
        }
       
        [HttpGet]
        [AllowAnonymous]
        // tra
        public async Task<IActionResult> MoMoReturn(
    string orderId, string requestId, string errorCode,
    string amount, string orderInfo, string signature, string extraData)
        {
           

            if (!int.TryParse(orderId, out var donHangId))
                return BadRequest("orderId không hợp lệ");

            var order = await _context.DonHangs.FirstOrDefaultAsync(o => o.DonHangId == donHangId);
            if (order == null) return NotFound();

            if (errorCode == "0")
            {
                order.TrangThai = "DaThanhToan";
            }
            else
            {
                order.TrangThai = "ThatBai";
                order.MoMoErrorCode = errorCode;
            }

            await _context.SaveChangesAsync();

            // Quay lại trang QR để hiện tick nếu thành công
            return RedirectToAction(nameof(ThanhToanMoMo), new { id = order.DonHangId });
        }



        private IActionResult ThanhToanVnPay(DonHang order)
        {
            // Tạo URL thanh toán
            var payUrl = _vnPay.CreatePaymentUrl(order, HttpContext);

            // Lưu lại VnPayTxnRef, VnPayPayUrl
            _context.DonHangs.Update(order);
            _context.SaveChanges();

            // Cách đơn giản nhất: redirect thẳng sang trang thanh toán của VNPAY
            return Redirect(payUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VnPayReturn()
        {
            var query = Request.Query;

            // 1. Kiểm tra chữ ký
            bool validSignature = _vnPay.ValidateSignature(query);
            if (!validSignature)
            {
                ViewBag.Message = "Chữ ký không hợp lệ!";
                return View("VnPayResult");
            }

            string vnp_TxnRef = query["vnp_TxnRef"];
            string vnp_ResponseCode = query["vnp_ResponseCode"];

            // 2. Tìm đơn hàng theo TxnRef đã lưu
            var order = await _context.DonHangs
                .FirstOrDefaultAsync(o => o.VnPayTxnRef == vnp_TxnRef);

            if (order == null)
            {
                ViewBag.Message = "Không tìm thấy đơn hàng!";
                return View("VnPayResult");
            }

            order.VnPayResponseCode = vnp_ResponseCode;

            if (vnp_ResponseCode == "00")
            {
                // Thanh toán thành công
                order.TrangThai = "DaThanhToan";

                // Xoá giỏ hàng của user (nếu thích)
                var cartItems = _context.GioHangItems
                    .Where(x => x.CustomerId == order.CustomerId);
                _context.GioHangItems.RemoveRange(cartItems);

                ViewBag.Message = $"Thanh toán thành công cho đơn #{order.DonHangId}.";
            }
            else
            {
                ViewBag.Message = $"Thanh toán thất bại. Mã lỗi: {vnp_ResponseCode}";
            }

            await _context.SaveChangesAsync();

            return View("VnPayResult", order);
        }




        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FakeMoMoPaid(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var order = await _context.DonHangs
                .FirstOrDefaultAsync(o => o.DonHangId == id && o.CustomerId == userId);

            if (order == null) return NotFound();

            // 👉 Giả lập kết quả thanh toán thành công
            order.TrangThai = "DaThanhToan";

            // Xoá giỏ hàng của user (nếu muốn)
            var cartItems = _context.GioHangItems.Where(x => x.CustomerId == userId);
            _context.GioHangItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            // Quay lại trang QR để hiển thị dấu tick
            return RedirectToAction(nameof(ThanhToanMoMo), new { id = order.DonHangId });
        }

    }
}
