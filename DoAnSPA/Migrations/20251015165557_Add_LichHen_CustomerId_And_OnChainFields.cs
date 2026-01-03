using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnSPA.Migrations
{
    /// <inheritdoc />
    public partial class Add_LichHen_CustomerId_And_OnChainFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KhachHangs");

            migrationBuilder.AddColumn<byte[]>(
                name: "bookingHash",
                table: "LichHens",
                type: "varbinary(32)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "customerId",
                table: "LichHens",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "onChainCompleteTx",
                table: "LichHens",
                type: "nvarchar(66)",
                maxLength: 66,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "onChainCreateTx",
                table: "LichHens",
                type: "nvarchar(66)",
                maxLength: 66,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerProfile",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerProfile", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_CustomerProfile_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_Customer_Time",
                table: "LichHens",
                columns: new[] { "customerId", "ngayHen", "gioHen" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProfile_Phone",
                table: "CustomerProfile",
                column: "Phone");

            migrationBuilder.AddForeignKey(
                name: "FK_LichHens_AspNetUsers_customerId",
                table: "LichHens",
                column: "customerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichHens_AspNetUsers_customerId",
                table: "LichHens");

            migrationBuilder.DropTable(
                name: "CustomerProfile");

            migrationBuilder.DropIndex(
                name: "IX_LichHen_Customer_Time",
                table: "LichHens");

            migrationBuilder.DropColumn(
                name: "bookingHash",
                table: "LichHens");

            migrationBuilder.DropColumn(
                name: "customerId",
                table: "LichHens");

            migrationBuilder.DropColumn(
                name: "onChainCompleteTx",
                table: "LichHens");

            migrationBuilder.DropColumn(
                name: "onChainCreateTx",
                table: "LichHens");

            migrationBuilder.CreateTable(
                name: "KhachHangs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHangs", x => x.Id);
                });
        }
    }
}
