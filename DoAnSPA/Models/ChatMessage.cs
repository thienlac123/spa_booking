using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Bắt buộc có dòng này

namespace DoAnSPA.Models
{
    [Table("ChatMessage")]
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }
        public string SessionId { get; set; }

        // Dữ liệu thực tế lưu trong DB
        public string Sender { get; set; }
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? UserId { get; set; }

        // --- PHẦN THÊM VÀO ĐỂ SỬA LỖI ---

        // 1. Tạo thuộc tính 'Content' trỏ về 'Message'
        [NotMapped] // [NotMapped] nghĩa là: Chỉ dùng trong code C#, không tạo cột trong SQL
        public string Content
        {
            get { return Message; }
            set { Message = value; }
        }

        // 2. Tạo thuộc tính 'IsBot' tự động kiểm tra xem Sender có phải là bot không
        [NotMapped]
        public bool IsBot
        {
            get { return Sender == "bot"; }
        }
    }
}