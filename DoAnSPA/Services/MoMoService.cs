using DoAnSPA.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace DoAnSPA.Services
{
    public class MoMoService
    {
        private readonly MoMoOptions _opt;
        private readonly HttpClient _http;

        public MoMoService(IOptions<MoMoOptions> options, HttpClient httpClient)
        {
            _opt = options.Value;
            _http = httpClient;
        }

        private string HmacSHA256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(messageBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public async Task<string?> CreatePaymentUrlAsync(DonHang order)
        {
            // 1. Chuẩn bị dữ liệu
            string orderId = order.DonHangId.ToString();
            string requestId = Guid.NewGuid().ToString("N");

            // Đảm bảo số tiền > 0, ép về long (MoMo yêu cầu số nguyên)
            long amountLong = (long)Math.Round(order.TongTien);
            if (amountLong <= 0) throw new Exception("Số tiền đơn hàng không hợp lệ.");

            string amount = amountLong.ToString();
            string orderInfo = $"Thanh toán đơn hàng #{order.DonHangId}";
            string extraData = "";

            // 2. Tạo chuỗi rawHash đúng thứ tự tham số
            string rawHash =
                $"accessKey={_opt.AccessKey}" +
                $"&amount={amount}" +
                $"&extraData={extraData}" +
                $"&ipnUrl={_opt.IpnUrl}" +
                $"&orderId={orderId}" +
                $"&orderInfo={orderInfo}" +
                $"&partnerCode={_opt.PartnerCode}" +
                $"&redirectUrl={_opt.RedirectUrl}" +
                $"&requestId={requestId}" +
                $"&requestType=captureWallet";

            string signature = HmacSHA256(rawHash, _opt.SecretKey);

            var body = new
            {
                partnerCode = _opt.PartnerCode,
                partnerName = "SpaBooking",
                storeId = "SpaStore01",
                requestId = requestId,
                amount = amount,
                orderId = orderId,
                orderInfo = orderInfo,
                redirectUrl = _opt.RedirectUrl,
                ipnUrl = _opt.IpnUrl,
                lang = "vi",
                extraData = extraData,
                requestType = "captureWallet",
                signature = signature
            };

            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync(_opt.Endpoint, content);
            var respStr = await response.Content.ReadAsStringAsync();

            // 3. Parse response
            dynamic resp = JsonConvert.DeserializeObject(respStr)!;

            int resultCode = resp.resultCode;
            string message = resp.message;

            // 👉 log tạm ra console / Debug để bạn đọc
            System.Diagnostics.Debug.WriteLine("MoMo resp: " + respStr);

            if (resultCode == 0)
            {
                order.MoMoOrderId = orderId;
                order.MoMoRequestId = requestId;
                order.MoMoErrorCode = "0";
                return (string)resp.payUrl;
            }
            // === ĐOẠN GIẢ LẬP CHO ĐỒ ÁN ===
            // Nếu là đồ án / môi trường không có tài khoản M4B, tạo 1 link giả để demo
            var fakeUrl = $"https://example.com/momo-fake?orderId={order.DonHangId}";
            order.MoMoErrorCode = resultCode.ToString(); // lưu lại để biết là giả lập
            return fakeUrl;

           
        }

    }
}
