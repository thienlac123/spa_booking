using DoAnSPA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace DoAnSPA.Services
{
    public class VnPayService
    {
        private readonly VnPayOptions _opt;

        public VnPayService(IOptions<VnPayOptions> options)
        {
            _opt = options.Value;
        }

        // Helper để mã hóa hash
        private string HmacSHA512(string key, string data)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(data))
                return "";

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
        }

        public string CreatePaymentUrl(DonHang order, HttpContext httpContext)
        {
            var vnp_TmnCode = _opt.TmnCode;
            var vnp_HashSecret = _opt.HashSecret;
            var vnp_Url = _opt.BaseUrl;
            var vnp_Returnurl = _opt.ReturnUrl;

            // 1. Lấy giờ Việt Nam (quan trọng nếu server đặt nước ngoài)
            TimeZoneInfo timeZoneId = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneId);

            // 2. Tạo mã tham chiếu
            string vnp_TxnRef = $"{order.DonHangId}_{now.Ticks}";
            string vnp_OrderInfo = $"Thanh toan don hang {order.DonHangId}"; // Khuyên dùng không dấu để an toàn nhất

            // 3. Xử lý IP (cần clean IPv6 ::1 nếu chạy localhost)
            string vnp_IpAddr = httpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            if (vnp_IpAddr == "::1") vnp_IpAddr = "127.0.0.1";

            // 4. Tạo danh sách tham số (SortedDictionary tự động sắp xếp theo key)
            var vnp_Params = new SortedDictionary<string, string>(new VnPayCompare())
            {
                {"vnp_Version", "2.1.0"},
                {"vnp_Command", "pay"},
                {"vnp_TmnCode", vnp_TmnCode},
                {"vnp_Amount", ((long)order.TongTien * 100).ToString()},
                {"vnp_CreateDate", now.ToString("yyyyMMddHHmmss")},
                {"vnp_CurrCode", "VND"},
                {"vnp_IpAddr", vnp_IpAddr},
                {"vnp_Locale", "vn"},
                {"vnp_OrderInfo", vnp_OrderInfo},
                {"vnp_OrderType", "other"},
                {"vnp_ReturnUrl", vnp_Returnurl},
                {"vnp_TxnRef", vnp_TxnRef},
                {"vnp_ExpireDate", now.AddMinutes(15).ToString("yyyyMMddHHmmss")} // Thêm thời gian hết hạn (15p)
            };

            // 5. Tạo chuỗi dữ liệu (Data) và chuỗi ký (Sign Data)
            // QUAN TRỌNG: Cả key và value đều phải UrlEncode khi tạo hash string
            var data = new StringBuilder();

            foreach (var kv in vnp_Params)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    // Dùng WebUtility.UrlEncode thay vì HttpUtility (nhẹ hơn trong .NET Core)
                    var encodedKey = WebUtility.UrlEncode(kv.Key);
                    var encodedValue = WebUtility.UrlEncode(kv.Value);

                    data.Append(encodedKey + "=" + encodedValue + "&");
                }
            }

            // Xóa dấu & cuối cùng
            string queryString = data.ToString();
            if (queryString.Length > 0)
            {
                queryString = queryString.Remove(queryString.Length - 1, 1);
            }

            // 6. Tạo mã Hash
            string vnp_SecureHash = HmacSHA512(vnp_HashSecret, queryString);

            // 7. Tạo đường dẫn thanh toán hoàn chỉnh
            string paymentUrl = $"{vnp_Url}?{queryString}&vnp_SecureHash={vnp_SecureHash}";

            // Update lại thông tin vào đơn hàng để lưu DB
            order.VnPayTxnRef = vnp_TxnRef;
            order.VnPayPayUrl = paymentUrl;

            return paymentUrl;
        }

        // Hàm kiểm tra chữ ký khi VNPAY trả về (IPN hoặc Return)
        public bool ValidateSignature(IQueryCollection query)
        {
            string inputHash = query["vnp_SecureHash"];
            if (string.IsNullOrEmpty(inputHash)) return false;

            var vnp_HashSecret = _opt.HashSecret;
            var vnp_Params = new SortedDictionary<string, string>(new VnPayCompare());

            // Lấy tất cả tham số trả về bắt đầu bằng vnp_ (trừ vnp_SecureHash)
            foreach (var key in query.Keys)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnp_Params.Add(key, query[key]);
                }
            }

            // Xóa 2 tham số hệ thống không tham gia ký
            vnp_Params.Remove("vnp_SecureHash");
            vnp_Params.Remove("vnp_SecureHashType");

            // Tạo chuỗi raw data để hash lại
            var data = new StringBuilder();
            foreach (var kv in vnp_Params)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    // Chú ý: Phải encode lại giá trị nhận được để khớp logic hash
                    var encodedKey = WebUtility.UrlEncode(kv.Key);
                    var encodedValue = WebUtility.UrlEncode(kv.Value);
                    data.Append(encodedKey + "=" + encodedValue + "&");
                }
            }

            string rawData = data.ToString();
            if (rawData.Length > 0) rawData = rawData.Remove(rawData.Length - 1, 1);

            string myChecksum = HmacSHA512(vnp_HashSecret, rawData);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    // Class so sánh để đảm bảo sắp xếp đúng theo bảng mã ASCII (yêu cầu VNPAY)
    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpCompare = CompareInfo.GetCompareInfo("en-US");
            return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
        }
    }
}