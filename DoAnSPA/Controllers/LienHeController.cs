using DoAnSPA.Data;
using DoAnSPA.Helpers;
using DoAnSPA.Models;
using DoAnSPA.Services; // ✅ Thêm EmailService
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace DoAnSPA.Controllers
{
    public class LienHeController : Controller
    {
        private readonly SpaDbContext _context;
        private readonly EmailService _emailService; // ✅ Inject EmailService
        private readonly EmailSettings _emailSettings; // ✅ Inject EmailSettings

        public LienHeController(SpaDbContext context, EmailService emailService, IOptions<EmailSettings> emailSettings)
        {
            _context = context;
            _emailService = emailService;
            _emailSettings = emailSettings.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LienHe model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // ✅ Lưu vào database
            _context.LienHes.Add(model);
            await _context.SaveChangesAsync();

            // ✅ Gửi email cho admin
            await SendEmailToAdmin(model);

            ViewBag.Message = "Cảm ơn bạn đã liên hệ. Chúng tôi sẽ phản hồi sớm nhất!";
            return View();
        }

        private async Task SendEmailToAdmin(LienHe model)
{
    try
    {
        string adminEmail = _emailSettings.SenderEmail;
        string subject = $"Liên hệ mới từ {model.HoTen}";
        string body = $"<p><b>Tên:</b> {model.HoTen}</p>" +
                      $"<p><b>Email:</b> {model.Email}</p>" +
                      $"<p><b>SĐT:</b> {model.SoDienThoai}</p>" +
                      $"<p><b>Nội dung:</b> {model.NoiDung}</p>";

        await _emailService.SendEmailAsync(adminEmail, subject, body);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Lỗi gửi email: " + ex.Message);
        // vẫn cho user thấy "Cảm ơn bạn đã liên hệ..."
    }
}

    }
}
