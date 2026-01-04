# DoAnSPA – Spa Booking & Management System (ASP.NET Core MVC)

Hệ thống quản lý & đặt lịch Spa (đồ án chuyên ngành) giúp khách hàng đặt lịch dịch vụ, đặt cọc/ thanh toán online và nhận tư vấn tự động qua AI Chatbot.

## Tech Stack
- **Backend/Web:** ASP.NET Core **MVC** (.NET 8)
- **ORM:** Entity Framework Core (EF Core)
- **Database:** SQL Server
- **Payment:** MoMo (Sandbox), VNPAY (Sandbox)
- **AI Chatbot:** Google **Gemini API**
- **Blockchain (Deposit):** Smart Contract (Hardhat + MetaMask)
- **Tools:** Visual Studio 2022, Postman, Git/GitHub
- **Authentication/Authorization:** ASP.NET Core Identity (Role-based)
  

## Main Features
- Xem danh sách dịch vụ / sản phẩm, thông tin spa
- Đặt lịch hẹn theo ngày/giờ, quản lý lịch hẹn
- Giỏ hàng và thanh toán/đặt cọc online (MoMo/VNPAY – sandbox)
- AI Chatbot tư vấn dịch vụ tự động (Gemini)
- Deposit minh bạch: ghi nhận giao dịch đặt cọc qua Smart Contract (txHash phục vụ kiểm chứng/đối soát)
- Khu vực quản trị (tuỳ theo cấu hình/role)

## Project Structure
- `DoAnSPA/` – Source code chính (ASP.NET Core MVC)
- `Blockchain/` – Smart contract (Hardhat) phục vụ tính năng Deposit
- `DoAnSPA.sln` – Solution file

---

## Getting Started (Local)

### 1) Requirements
- .NET SDK 8
- Visual Studio 2022 (hoặc VS Code + C# extension)
- SQL Server
- (Tuỳ chọn) Node.js nếu chạy phần `Blockchain/`

### 2) Clone & Restore
```bash
git clone https://github.com/thienlac123/spa_booking.git
cd spa_booking
