using Microsoft.AspNetCore.Mvc;
using DoAnSPA.Data;
using DoAnSPA.Models;
using System.Linq;

public class NhanVienController : Controller
{
    private readonly SpaDbContext _context;

    public NhanVienController(SpaDbContext context)
    {
        _context = context;
    }

    // ✅ Hiển thị danh sách nhân viên
    public IActionResult Index()
    {
        var nhanViens = _context.NhanViens
            .Where(nv => nv.HienThiTrangChu) // Chỉ lấy nhân viên hiển thị
            .ToList();

        return View(nhanViens);
    }

    // ✅ Hiển thị chi tiết nhân viên (nếu cần)
    public IActionResult Details(int id)
    {
        var nhanVien = _context.NhanViens.FirstOrDefault(nv => nv.Id == id);
        if (nhanVien == null) return NotFound();

        return View(nhanVien);
    }
}
