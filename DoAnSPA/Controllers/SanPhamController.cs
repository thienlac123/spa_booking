using DoAnSPA.Data;
using DoAnSPA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DoAnSPA.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly SpaDbContext _context;

        public SanPhamController(SpaDbContext context)
        {
            _context = context;
        }

        // Trang danh sách sản phẩm (cho mọi người xem)
        [AllowAnonymous]
        public async Task<IActionResult> Index(string search, string category, string sortOrder)
        {
            // base query: chỉ lấy sản phẩm đang active
            var query = _context.SanPhams
                .Where(x => x.IsActive)
                .AsQueryable();

            // 🔍 TÌM KIẾM theo tên / mô tả
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.TenSanPham.Contains(search) ||
                    p.MoTa.Contains(search));
            }

            // 🏷 LỌC THEO LOẠI SẢN PHẨM
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.LoaiSanPham == category);
            }

            // ⬆⬇ SẮP XẾP
            switch (sortOrder)
            {
                case "price_asc":
                    query = query.OrderBy(p => p.Gia);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.Gia);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(p => p.TenSanPham);
                    break;
                case "name_asc":
                default:
                    query = query.OrderBy(p => p.TenSanPham);
                    break;
            }

            // Lấy danh sách loại để fill dropdown
            var categories = await _context.SanPhams
                .Where(x => x.IsActive && !string.IsNullOrEmpty(x.LoaiSanPham))
                .Select(x => x.LoaiSanPham)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.Search = search;
            ViewBag.Category = category;
            ViewBag.SortOrder = sortOrder;

            var list = await query.ToListAsync();
            return View(list);
        }

        // Thêm vào giỏ hàng (cần đăng nhập Customer)
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int id, int soLuong = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null || !sp.IsActive) return NotFound();

            var existing = await _context.GioHangItems
                .FirstOrDefaultAsync(g => g.CustomerId == userId && g.SanPhamId == id);

            if (existing != null)
            {
                existing.SoLuong += soLuong;
            }
            else
            {
                _context.GioHangItems.Add(new GioHangItem
                {
                    SanPhamId = id,
                    CustomerId = userId,
                    SoLuong = soLuong
                });
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã thêm vào giỏ hàng.";
            return RedirectToAction(nameof(Index));
        }
    }
}
