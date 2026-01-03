using DoAnSPA.Data;
using DoAnSPA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DoAnSPA.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatAiController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;
        private readonly SpaDbContext _db;

        public ChatAiController(
            IConfiguration config,
            IHttpClientFactory httpClientFactory,
            SpaDbContext db)
        {
            _config = config;
            _http = httpClientFactory.CreateClient();
            _db = db;
        }

        /// <summary>
        /// POST /api/ChatAi/ask
        /// Body: { "message": "..." }
        /// </summary>
        [HttpPost("ask")]
        public async Task<ActionResult<ChatAiResponse>> Ask([FromBody] ChatAiRequest req)
        {
            // 1. Kiểm tra input
            if (req == null || string.IsNullOrWhiteSpace(req.Message))
            {
                return BadRequest(new ChatAiResponse
                {
                    Success = false,
                    Error = "Nội dung câu hỏi trống."
                });
            }

            // 2. Lấy cấu hình Gemini
            var apiKey = _config["Gemini:ApiKey"];
            var modelName = _config["Gemini:Model"];

            if (string.IsNullOrWhiteSpace(modelName))
            {
                // Mặc định dùng model mới
                modelName = "gemini-2.5-flash";
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, new ChatAiResponse
                {
                    Success = false,
                    Error = "Chưa cấu hình Gemini ApiKey (Gemini:ApiKey trong appsettings.json)."
                });
            }

            // 3. RULE: user nói chung "khuyến mãi" => trả trực tiếp từ ComboDichVu, KHÔNG gọi Gemini
            var rawText = req.Message.Trim();
            var lower = rawText.ToLower();

            if (lower.Contains("khuyến mãi") || lower.Contains("khuyen mai"))
            {
                var combos = await _db.ComboDichVus
                    .Select(c => new
                    {
                        c.ComboDichVuId,   // ĐÚNG tên ID trong model của bạn
                        c.TenCombo,        // Tên combo
                        c.GiaKhuyenMai     // Giá khuyến mãi (sửa nếu tên khác, ví dụ GiaCombo)
                    })
                    .ToListAsync();

                if (combos.Any())
                {
                    var sbReply = new StringBuilder();
                    sbReply.AppendLine("Hiện tại spa đang có một số combo khuyến mãi như sau:");

                    foreach (var c in combos)
                    {
                        sbReply.AppendLine($"• {c.TenCombo} (khoảng {c.GiaKhuyenMai:N0} VND)");
                    }

                    sbReply.AppendLine();
                    sbReply.Append("Bạn thích combo nào, hoặc bạn có thể bấm vào mục Khuyến mãi để xem chi tiết rồi đặt lịch nhé!");

                    return Ok(new ChatAiResponse
                    {
                        Success = true,
                        Reply = sbReply.ToString()
                    });
                }
                else
                {
                    // DB không có combo thì mới nói không có khuyến mãi
                    return Ok(new ChatAiResponse
                    {
                        Success = true,
                        Reply = "Hiện tại spa chưa có chương trình khuyến mãi nào được mở. " +
                                "Bạn vẫn có thể đặt các dịch vụ lẻ, mình sẽ hỗ trợ tư vấn lịch trống cho bạn nhé."
                    });
                }
            }

            // 4. Lấy DỊCH VỤ & COMBO từ DB để đưa vào prompt cho Gemini
            var dichVus = await _db.DichVus
                .Select(d => new
                {
                    d.DichVuId,      // Sửa nếu ID tên khác
                    d.TenDichVu,     // Tên dịch vụ
                    d.MoTa,          // Mô tả
                    d.Gia            // Giá (nếu bạn dùng DonGia thì đổi lại)
                })
                .ToListAsync();

            var combosAll = await _db.ComboDichVus
                .Select(c => new
                {
                    c.ComboDichVuId,
                    c.TenCombo,
                  
                    c.GiaKhuyenMai  // hoặc GiaCombo nếu bạn đặt tên khác
                })
                .ToListAsync();

            var sbInfo = new StringBuilder();
            if (dichVus.Any())
            {
                sbInfo.AppendLine("• Dịch vụ lẻ hiện có tại spa:");
                foreach (var d in dichVus)
                {
                    sbInfo.AppendLine($"  - {d.TenDichVu}: {d.MoTa} (khoảng {d.Gia:N0} VND)");
                }
            }

            if (combosAll.Any())
            {
                sbInfo.AppendLine();
                sbInfo.AppendLine("• Các combo khuyến mãi:");
                foreach (var c in combosAll)
                {
                    sbInfo.AppendLine($"  - {c.TenCombo}: (khoảng {c.GiaKhuyenMai:N0} VND)");
                }
            }

            var internalSpaInfo = sbInfo.ToString();

            // 5. Gọi Gemini
            var url = $"https://generativelanguage.googleapis.com/v1/models/{modelName}:generateContent?key={apiKey}";

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text =
                                    BuildSystemPrompt() + "\n\n" +
                                    "Thông tin nội bộ về dịch vụ của spa (chỉ dùng để tư vấn, không cần lặp lại y nguyên):\n" +
                                    internalSpaInfo + "\n\n" +
                                    "Người dùng: " + req.Message
                            }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _http.PostAsync(url, httpContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(500, new ChatAiResponse
                    {
                        Success = false,
                        Error = $"Gemini lỗi {response.StatusCode}: {responseBody}"
                    });
                }

                string reply = "Không đọc được phản hồi từ Gemini.";

                using (var doc = JsonDocument.Parse(responseBody))
                {
                    var root = doc.RootElement;

                    if (root.TryGetProperty("candidates", out var candidates) &&
                        candidates.ValueKind == JsonValueKind.Array &&
                        candidates.GetArrayLength() > 0)
                    {
                        var first = candidates[0];

                        if (first.TryGetProperty("content", out var contentElement) &&
                            contentElement.TryGetProperty("parts", out var partsElement) &&
                            partsElement.ValueKind == JsonValueKind.Array &&
                            partsElement.GetArrayLength() > 0)
                        {
                            var part0 = partsElement[0];

                            if (part0.TryGetProperty("text", out var textElement))
                            {
                                reply = textElement.GetString() ?? reply;
                            }
                        }
                    }
                }

                return Ok(new ChatAiResponse
                {
                    Success = true,
                    Reply = reply
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ChatAiResponse
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Dùng để nhận diện dịch vụ / combo từ câu người dùng để tạo link đặt lịch
        /// POST /api/ChatAi/resolve
        /// Body: { "message": "..." }
        /// </summary>
        [HttpPost("resolve")]
        public async Task<ActionResult<ResolveServiceResponse>> Resolve([FromBody] ChatAiRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Message))
            {
                return Ok(new ResolveServiceResponse { Found = false });
            }

            var textNorm = Normalize(req.Message);

            var stopwords = new HashSet<string>
            {
                "muon","muốn","dat","đặt","dich","dịch","vu","vụ",
                "lich","lịch","khuyen","khuyến","mai","mãi","combo",
                "goi","gói","toi","tôi","cho","minh","mình","lam","làm"
            };

            var userWords = SplitWords(textNorm)
                .Where(w => !stopwords.Contains(w))
                .ToHashSet();

            if (userWords.Count == 0)
                return Ok(new ResolveServiceResponse { Found = false });

            // 1) Thử match DỊCH VỤ
            var dichVus = await _db.DichVus
                .Select(d => new { d.DichVuId, d.TenDichVu })
                .ToListAsync();

            foreach (var d in dichVus)
            {
                if (IsMatch(userWords, Normalize(d.TenDichVu), stopwords))
                {
                    return Ok(new ResolveServiceResponse
                    {
                        Found = true,
                        Type = "dichvu",
                        Id = d.DichVuId,
                        Name = d.TenDichVu
                    });
                }
            }

            // 2) Thử match COMBO
            var combos = await _db.ComboDichVus
                .Select(c => new { c.ComboDichVuId, c.TenCombo })
                .ToListAsync();

            foreach (var c in combos)
            {
                if (IsMatch(userWords, Normalize(c.TenCombo), stopwords))
                {
                    return Ok(new ResolveServiceResponse
                    {
                        Found = true,
                        Type = "combo",
                        Id = c.ComboDichVuId,
                        Name = c.TenCombo
                    });
                }
            }

            return Ok(new ResolveServiceResponse { Found = false });
        }

        // PROMPT hệ thống: trợ lý của spa, trả lời ngắn
        private string BuildSystemPrompt()
        {
            return "Bạn là trợ lý của spa, hỗ trợ tư vấn và đặt lịch cho khách hàng. " +
                   "Trả lời bằng tiếng Việt, giọng điệu thân thiện, tự nhiên như đang chat. " +
                   "LUÔN TRẢ LỜI NGẮN GỌN: tối đa 2–3 câu hoặc dưới 60–80 từ, tránh lặp lại ý. " +
                   "Khi khách hỏi về dịch vụ, hãy nói ngắn: mô tả nhanh, giá tham khảo, và hỏi 1 câu " +
                   "để chốt lịch (ngày, giờ, tên, số điện thoại). " +
                   "Các gói combo trong thông tin nội bộ CHÍNH LÀ chương trình khuyến mãi hiện tại của spa, " +
                   "vì vậy nếu đã có combo thì KHÔNG ĐƯỢC nói là spa không có khuyến mãi. " +
                   "Nếu câu hỏi không liên quan đến spa, có thể trả lời ngắn nhưng khéo léo hướng khách quay lại chủ đề spa.";
        }

        // ====== Helper cho Resolve ======

        private static string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            input = input.ToLower().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var ch in input)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private static IEnumerable<string> SplitWords(string input)
        {
            var current = new StringBuilder();
            foreach (var ch in input)
            {
                if (char.IsLetterOrDigit(ch))
                {
                    current.Append(ch);
                }
                else if (current.Length > 0)
                {
                    yield return current.ToString();
                    current.Clear();
                }
            }

            if (current.Length > 0)
                yield return current.ToString();
        }

        private static bool IsMatch(HashSet<string> userWords, string serviceNameNorm, HashSet<string> stopwords)
        {
            var nameWords = SplitWords(serviceNameNorm)
                .Where(w => !stopwords.Contains(w))
                .ToHashSet();

            if (nameWords.Count == 0) return false;

            // Điều kiện đơn giản: tất cả từ "quan trọng" của user phải nằm trong tên dịch vụ
            return userWords.All(w => nameWords.Contains(w));
        }
    }

    /// <summary>
    /// Kết quả nhận diện dịch vụ/combo để tạo link đặt lịch
    /// </summary>
    public class ResolveServiceResponse
    {
        public bool Found { get; set; }
        /// <summary>"dichvu" hoặc "combo"</summary>
        public string Type { get; set; } = "";
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}
