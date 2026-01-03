using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnSPA.Migrations
{
    /// <inheritdoc />
    public partial class updonhang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MoMoPayUrl",
                table: "DonHangs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhuongThucThanhToan",
                table: "DonHangs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VnPayPayUrl",
                table: "DonHangs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VnPayResponseCode",
                table: "DonHangs",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VnPayTxnRef",
                table: "DonHangs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoMoPayUrl",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "PhuongThucThanhToan",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "VnPayPayUrl",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "VnPayResponseCode",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "VnPayTxnRef",
                table: "DonHangs");
        }
    }
}
