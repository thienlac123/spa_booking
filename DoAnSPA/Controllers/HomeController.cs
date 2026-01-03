using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DoAnSPA.Models;
using DoAnSPA.Repositories;
using System.Linq;

namespace DoAnSPA.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDichVuRepository _dichVuRepository;
        private readonly INhanVienRepository _nhanVienRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IDichVuRepository dichVuRepository,
            INhanVienRepository nhanVienRepository)
        {
            _logger = logger;
            _dichVuRepository = dichVuRepository;
            _nhanVienRepository = nhanVienRepository;
        }

        [HttpGet("")]
        [HttpGet("index")]
        public IActionResult Index()
        {
            var viewModel = new TrangChuViewModel
            {
                DichVus = _dichVuRepository.GetAll()
                            .Where(dv => dv.HienThiTrangChu)
                            .Take(3)
                            .ToList(),

                NhanViens = _nhanVienRepository.GetAll()
                              .Where(nv => nv.HienThiTrangChu)
                              .Take(3)
                              .ToList()
            };

            return View(viewModel);
        }

        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
