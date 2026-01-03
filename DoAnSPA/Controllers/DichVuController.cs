using DoAnSPA.Data;
using DoAnSPA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace DoAnSPA.Controllers
{

    public class DichVuController : Controller
    {
        private readonly SpaDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DichVuController(SpaDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        // Trang trưng bày dịch vụ cho khách
        [HttpGet]
        [Route("DichVu/Index")]
        public IActionResult Index()
        {
            var dichVus = _context.DichVus.ToList();
            return View(dichVus);
        }
    }
}
