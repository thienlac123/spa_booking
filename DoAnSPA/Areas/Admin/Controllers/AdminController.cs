using DoAnSPA.Data;
using DoAnSPA.Models;
using DoAnSPA.Models.ViewModels;
using DoAnSPA.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnSPA.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminController : Controller
    {
        private readonly SpaDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(SpaDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Khi truy cập /admin thì redirect sang /admin/trangchu
        [HttpGet]
        [Route("admin")]
        public IActionResult Index()
        {
            return RedirectToAction("TrangChu");
        }

        // ------------------ QUẢN LÝ TRANG CHỦ --------------------

        [HttpGet]
        [Route("admin/trangchu")]
        public IActionResult TrangChu()
        {
            var viewModel = new TrangChuViewModel
            {
                DichVus = _context.DichVus.ToList(),      // Lấy tất cả dịch vụ
                NhanViens = _context.NhanViens.ToList()  // Lấy tất cả nhân viên
            };

            return View(viewModel);
        }


        [HttpPost]
        [Route("admin/trangchu/capnhathienthidichvu")]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatHienThiDichVu(int id, bool hienThi)
        {
            var dichVu = _context.DichVus.Find(id);
            if (dichVu != null)
            {
                dichVu.HienThiTrangChu = hienThi;
                _context.SaveChanges();
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [Route("admin/trangchu/capnhathienthinhanvien")]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatHienThiNhanVien(int id, bool hienThi)
        {
            var nhanVien = _context.NhanViens.Find(id);
            if (nhanVien != null)
            {
                nhanVien.HienThiTrangChu = hienThi;
                _context.SaveChanges();
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }


        // Danh sách dịch vụ
        [HttpGet]
        [Route("admin/service")]
        public IActionResult Service()
        {
            var dichVus = _context.DichVus.ToList();
            return View("IndexDichVu", dichVus);
        }

        // Thêm dịch vụ - GET
        [HttpGet]
        [Route("admin/service/create")]
        public IActionResult ServiceCreate()
        {
            return View("CreateDichVu");
        }

        // Thêm dịch vụ - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/service/create")]
        public IActionResult ServiceCreate(DichVu dichVu, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    ImageFile.CopyTo(fileStream);

                    dichVu.HinhAnh = "/uploads/" + uniqueFileName;
                }

                _context.DichVus.Add(dichVu);
                _context.SaveChanges();
                return RedirectToAction(nameof(Service));
            }
            return View("CreateDichVu", dichVu);
        }

        // Sửa dịch vụ - GET
        [HttpGet]
        [Route("admin/service/edit/{id}")]
        public IActionResult ServiceEdit(int id)
        {
            var dichVu = _context.DichVus.Find(id);
            if (dichVu == null) return NotFound();
            return View("EditDichVu", dichVu);
        }

        // Sửa dịch vụ - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/service/edit/{id}")]
        public IActionResult ServiceEdit(int id, DichVu dichVu, IFormFile? ImageFile)
        {
            if (id != dichVu.DichVuId) return BadRequest();

            if (ModelState.IsValid)
            {
                var existingDichVu = _context.DichVus.Find(id);
                if (existingDichVu == null) return NotFound();

                existingDichVu.TenDichVu = dichVu.TenDichVu;
                existingDichVu.MoTa = dichVu.MoTa;
                existingDichVu.Gia = dichVu.Gia;

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    ImageFile.CopyTo(fileStream);

                    existingDichVu.HinhAnh = "/uploads/" + uniqueFileName;
                }

                _context.DichVus.Update(existingDichVu);
                _context.SaveChanges();
                return RedirectToAction(nameof(Service));
            }
            return View("EditDichVu", dichVu);
        }

        // Xóa dịch vụ - GET
        [HttpGet]
        [Route("admin/service/delete/{id}")]
        public IActionResult ServiceDelete(int id)
        {
            var dichVu = _context.DichVus.Find(id);
            if (dichVu == null) return NotFound();
            return View("DeleteDichVu", dichVu);
        }

        // Xóa dịch vụ - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/service/deleteconfirmed/{id}")]
        public IActionResult ServiceDeleteConfirmed(int id)
        {
            var dichVu = _context.DichVus.Find(id);
            if (dichVu == null) return NotFound();

            _context.DichVus.Remove(dichVu);
            _context.SaveChanges();

            return RedirectToAction(nameof(Service));
        }
        // -------------------- COMBO ----------------------------

        [HttpGet]
        [Route("admin/combo")]
        public IActionResult Combo()
        {
            var combos = _context.ComboDichVus
                .Include(c => c.DichVus)
                .ToList();

            var dichVus = _context.DichVus.ToList();
            ViewBag.AvailableDichVus = dichVus;

            return View("KhuyenMai", combos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/combo/add")]
        public IActionResult AddCombo(string TenCombo, decimal GiaKhuyenMai, List<int> SelectedDichVuIds)
        {
            if (string.IsNullOrEmpty(TenCombo) || GiaKhuyenMai <= 0 || SelectedDichVuIds == null || !SelectedDichVuIds.Any())
            {
                ModelState.AddModelError("", "Vui lòng nhập đủ thông tin và chọn ít nhất một dịch vụ.");
                return RedirectToAction(nameof(Combo));
            }

            var selectedDichVus = _context.DichVus.Where(dv => SelectedDichVuIds.Contains(dv.DichVuId)).ToList();

            if (!selectedDichVus.Any())
            {
                ModelState.AddModelError("", "Dịch vụ không hợp lệ.");
                return RedirectToAction(nameof(Combo));
            }

            var newCombo = new ComboDichVu
            {
                TenCombo = TenCombo,
                GiaKhuyenMai = GiaKhuyenMai,
                DichVus = selectedDichVus
            };

            _context.ComboDichVus.Add(newCombo);
            _context.SaveChanges();

            return RedirectToAction(nameof(Combo));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/combo/edit")]
        public IActionResult EditCombo(int ComboDichVuId, string TenCombo, decimal GiaKhuyenMai, List<int> SelectedDichVuIds)
        {
            var combo = _context.ComboDichVus
                .Include(c => c.DichVus)
                .FirstOrDefault(c => c.ComboDichVuId == ComboDichVuId);

            if (combo == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(TenCombo) || GiaKhuyenMai <= 0 || SelectedDichVuIds == null || !SelectedDichVuIds.Any())
            {
                ModelState.AddModelError("", "Vui lòng nhập đủ thông tin và chọn ít nhất một dịch vụ.");
                return RedirectToAction(nameof(Combo));
            }

            combo.TenCombo = TenCombo;
            combo.GiaKhuyenMai = GiaKhuyenMai;

            combo.DichVus.Clear();
            combo.DichVus = _context.DichVus.Where(dv => SelectedDichVuIds.Contains(dv.DichVuId)).ToList();

            _context.SaveChanges();

            return RedirectToAction(nameof(Combo));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/combo/delete")]
        public IActionResult DeleteCombo(int ComboDichVuId)
        {
            var combo = _context.ComboDichVus
                .Include(c => c.DichVus)
                .FirstOrDefault(c => c.ComboDichVuId == ComboDichVuId);

            if (combo == null)
            {
                return NotFound();
            }

            combo.DichVus.Clear();
            _context.SaveChanges();

            _context.ComboDichVus.Remove(combo);
            _context.SaveChanges();

            return RedirectToAction(nameof(Combo));
        }
        // -------------------- NHÂN VIÊN ----------------------------

        [HttpGet]
        [Route("admin/staff")]
        public IActionResult Staff()
        {
            var nhanViens = _context.NhanViens.ToList();
            return View("NhanVien", nhanViens); // View NhanVien.cshtml trong Areas/Admin/Views/Admin/
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/staff/add")]
        public IActionResult AddStaff(NhanVien nhanVien, IFormFile anhFile)
        {
            if (anhFile != null && anhFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/nhanvien");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(anhFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    anhFile.CopyTo(fileStream);
                }

                nhanVien.AnhNhanVien = "/images/nhanvien/" + uniqueFileName;
            }

            _context.NhanViens.Add(nhanVien);
            _context.SaveChanges();
            return RedirectToAction(nameof(Staff));
        }

        [HttpGet]
        [Route("admin/staff/edit/{id}")]
        public IActionResult EditStaff(int id)
        {
            var nhanVien = _context.NhanViens.Find(id);
            if (nhanVien == null) return NotFound();
            return View("EditNhanVien", nhanVien); // View EditNhanVien.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/staff/edit/{id}")]
        public IActionResult EditStaff(int id, NhanVien nhanVien, IFormFile anhFile)
        {
            if (id != nhanVien.Id) return BadRequest();

            var existingNhanVien = _context.NhanViens.Find(id);
            if (existingNhanVien == null) return NotFound();

            existingNhanVien.HoTen = nhanVien.HoTen;
            existingNhanVien.QueQuan = nhanVien.QueQuan;
            existingNhanVien.SoDienThoai = nhanVien.SoDienThoai;
            existingNhanVien.ViTri = nhanVien.ViTri;
            existingNhanVien.Facebook = nhanVien.Facebook;
            existingNhanVien.Twitter = nhanVien.Twitter;
            existingNhanVien.LinkedIn = nhanVien.LinkedIn;
            existingNhanVien.Instagram = nhanVien.Instagram;

            if (anhFile != null && anhFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/nhanvien");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(anhFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    anhFile.CopyTo(fileStream);
                }

                existingNhanVien.AnhNhanVien = "/images/nhanvien/" + uniqueFileName;
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Staff));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/staff/delete/{id}")]
        public IActionResult DeleteStaff(int id)
        {
            var nhanVien = _context.NhanViens.Find(id);
            if (nhanVien == null) return NotFound();

            _context.NhanViens.Remove(nhanVien);
            _context.SaveChanges();
            return RedirectToAction(nameof(Staff));
        }
        // =================== LỊCH HẸN ===================

        [HttpGet]
        [Route("admin/lichhen")]
        public async Task<IActionResult> LichHen(DateTime? ngayBatDau, DateTime? ngayKetThuc)
        {
            DateTime startDate = ngayBatDau ?? DateTime.Today;
            DateTime endDate = ngayKetThuc ?? DateTime.Today;

            if (endDate < startDate)
            {
                var temp = startDate;
                startDate = endDate;
                endDate = temp;
            }

            var lichHens = await _context.LichHens
                .Include(l => l.DichVu)
                .Include(l => l.NhanVien)
                .Where(l => l.ngayHen.Date >= startDate.Date && l.ngayHen.Date <= endDate.Date)
                .OrderBy(l => l.ngayHen)
                .ThenBy(l => l.gioHen)
                .ToListAsync();

            ViewBag.NgayBatDau = startDate.ToString("yyyy-MM-dd");
            ViewBag.NgayKetThuc = endDate.ToString("yyyy-MM-dd");

            var thongKeTrangThai = lichHens
                .GroupBy(l => l.trangThai)
                .Select(g => new { TrangThai = g.Key, Count = g.Count() })
                .ToDictionary(x => x.TrangThai, x => x.Count);

            ViewBag.ThongKeTrangThai = thongKeTrangThai;

            return View("IndexLichHen", lichHens);
        }


        // =================== CHỈNH SỬA LỊCH HẸN ===================

        [HttpGet]
        [Route("admin/lichhen/edit/{id}")]
        public async Task<IActionResult> EditLichHen(int id)
        {
            var lichHen = await _context.LichHens
                .Include(l => l.DichVu)
                .Include(l => l.NhanVien)
                .FirstOrDefaultAsync(l => l.lichHenId == id);

            if (lichHen == null)
                return NotFound();

            var viewModel = new LichHenEditTrangThaiViewModel
            {
                LichHenId = lichHen.lichHenId,
                TrangThai = lichHen.trangThai,
                TenDichVu = lichHen.DichVu?.TenDichVu,
                NgayHen = lichHen.ngayHen,
                GioHen = lichHen.gioHen,
                GhiChu = lichHen.ghiChu,
                TenNhanVien = lichHen.NhanVien?.HoTen
            };

            return View("EditLichHen", viewModel);
        }

        [HttpPost]
        [Route("admin/lichhen/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLichHen(LichHenEditTrangThaiViewModel model)
        {
            if (ModelState.IsValid)
            {
                var lichHen = await _context.LichHens.FindAsync(model.LichHenId);
                if (lichHen == null)
                    return NotFound();

                lichHen.trangThai = model.TrangThai;

                try
                {
                    _context.Update(lichHen);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";

                    // ✅ Lấy lại ngày lọc từ Form để quay về đúng phạm vi
                    var ngayBatDau = Request.Form["ngayBatDau"].ToString();
                    var ngayKetThuc = Request.Form["ngayKetThuc"].ToString();

                    return RedirectToAction("LichHen", new
                    {
                        ngayBatDau = ngayBatDau,
                        ngayKetThuc = ngayKetThuc
                    });
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi cập nhật trạng thái.");
                }
            }

            return View("EditLichHen", model);
        }


        //bảng Lịch hẹn theo giờ 
        [HttpGet]
        [Route("admin/lichhen/lichtheogio")]
        public IActionResult LichHenTheoGio(int weekOffset = 0)
        {
            DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1 + 7 * weekOffset);
            DateTime endOfWeek = startOfWeek.AddDays(6);

            var lichHens = _context.LichHens
                .Include(l => l.DichVu)
                .Include(l => l.NhanVien)
                .Where(l => l.ngayHen.Date >= startOfWeek && l.ngayHen.Date <= endOfWeek)
                .OrderBy(l => l.ngayHen)
                .ThenBy(l => l.gioHen)
                .ToList();

            ViewBag.StartOfWeek = startOfWeek;
            ViewBag.EndOfWeek = endOfWeek;
            ViewBag.WeekOffset = weekOffset;

            return View("LichHenTheoGio", lichHens);
        }
        // =================== SẢN PHẨM BÁN LẺ ===================

        [HttpGet]
        [Route("admin/sanpham")]
        public async Task<IActionResult> SanPham(string? search, string? category)
        {
            var query = _context.SanPhams.AsQueryable();

            // 🔍 TÌM KIẾM theo tên / mô tả
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.TenSanPham.Contains(search) ||
                    p.MoTa.Contains(search));
            }

            // 🏷 LỌC THEO LOẠI SẢN PHẨM (LoaiSanPham)
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.LoaiSanPham == category);
            }

            // Danh sách loại để fill dropdown trên view admin
            var categories = await _context.SanPhams
                .Where(x => !string.IsNullOrEmpty(x.LoaiSanPham))
                .Select(x => x.LoaiSanPham)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.Search = search;
            ViewBag.Category = category;

            var list = await query
                .OrderByDescending(x => x.SanPhamId)
                .ToListAsync();

            return View("IndexSanPham", list);
        }

        // GET: admin/sanpham/create
        [HttpGet]
        [Route("admin/sanpham/create")]
        public IActionResult SanPhamCreate()
        {
            return View("CreateSanPham");
        }

        // POST: admin/sanpham/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/sanpham/create")]
        public async Task<IActionResult> SanPhamCreate(SanPham model, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
                return View("CreateSanPham", model);

            if (ImageFile != null && ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/sanpham");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(fileStream);
                }

                model.HinhAnhUrl = "/images/sanpham/" + uniqueFileName;
            }

            _context.SanPhams.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã thêm sản phẩm mới.";
            return RedirectToAction(nameof(SanPham));
        }

        // GET: admin/sanpham/edit/
        [HttpGet]
        public async Task<IActionResult> SanPhamEdit(int id)
        {
            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null) return NotFound();

            return View("EditSanPham", sp);
        }

        // POST: Admin/SanPhamEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SanPhamEdit(int id, SanPham model, IFormFile? ImageFile)
        {
            if (id != model.SanPhamId) return BadRequest();

            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null) return NotFound();

            if (!ModelState.IsValid)
            {
                // nếu có lỗi validate thì quay lại view để thấy message
                return View("EditSanPham", model);
            }

            // cập nhật field text
            sp.TenSanPham = model.TenSanPham;
            sp.MoTa = model.MoTa;
            sp.Gia = model.Gia;
            sp.IsActive = model.IsActive;
            sp.LoaiSanPham = model.LoaiSanPham;

            // xử lý upload ảnh mới (nếu có)
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "sanpham");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var ext = Path.GetExtension(ImageFile.FileName);
                var uniqueFileName = Guid.NewGuid() + ext;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await ImageFile.CopyToAsync(fileStream);

                sp.HinhAnhUrl = "/images/sanpham/" + uniqueFileName;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã cập nhật sản phẩm.";
            return RedirectToAction(nameof(SanPham));   // action liệt kê danh sách sản phẩm admin
        }


        // GET: admin/sanpham/delete/
        [HttpGet]
        [Route("admin/sanpham/delete/{id}")]
        public async Task<IActionResult> SanPhamDelete(int id)
        {
            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null) return NotFound();

            return View("DeleteSanPham", sp);
        }

        // POST: admin/sanpham/deleteconfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/sanpham/deleteconfirmed/{id}")]
        public async Task<IActionResult> SanPhamDeleteConfirmed(int id)
        {
            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null) return NotFound();

            _context.SanPhams.Remove(sp);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xoá sản phẩm.";
            return RedirectToAction(nameof(SanPham));
        }

        // Bật / tắt đang bán
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/sanpham/toggle/{id}")]
        public async Task<IActionResult> SanPhamToggle(int id)
        {
            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null) return NotFound();

            sp.IsActive = !sp.IsActive;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(SanPham));
        }
        // =================== ĐƠN HÀNG SẢN PHẨM ===================

        [HttpGet]
        [Route("admin/donhang")]
        public async Task<IActionResult> DonHang()
        {
            var list = await _context.DonHangs
                .Include(d => d.Customer)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return View("IndexDonHang", list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/donhang/capnhat")]
        public async Task<IActionResult> CapNhatTrangThaiDonHang(int id, string trangThai)
        {
            var order = await _context.DonHangs.FindAsync(id);
            if (order == null) return NotFound();

            order.TrangThai = trangThai;   // "ChuanBi" / "DangGiao" / "DaGiao" / "Huy"
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã cập nhật trạng thái đơn hàng.";
            return RedirectToAction(nameof(DonHang));
        }





    }
}
