using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnSPA.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingInfoToDonHang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiaChiNhan",
                table: "DonHangs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "DonHangs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoDienThoaiNhan",
                table: "DonHangs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenNguoiNhan",
                table: "DonHangs",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiaChiNhan",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "SoDienThoaiNhan",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "TenNguoiNhan",
                table: "DonHangs");
        }
    }
}
