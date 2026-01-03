using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnSPA.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComboDichVus",
                columns: table => new
                {
                    ComboDichVuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenCombo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaKhuyenMai = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboDichVus", x => x.ComboDichVuId);
                });

            migrationBuilder.CreateTable(
                name: "DichVus",
                columns: table => new
                {
                    DichVuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDichVu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HienThiTrangChu = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVus", x => x.DichVuId);
                });

            migrationBuilder.CreateTable(
                name: "KhachHangs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHangs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LichHens",
                columns: table => new
                {
                    lichHenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    khachHangId = table.Column<int>(type: "int", nullable: false),
                    dichVuId = table.Column<int>(type: "int", nullable: false),
                    nhanVienId = table.Column<int>(type: "int", nullable: true),
                    ngayHen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    gioHen = table.Column<TimeSpan>(type: "time", nullable: false),
                    ghiChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    trangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichHens", x => x.lichHenId);
                });

            migrationBuilder.CreateTable(
                name: "LienHes",
                columns: table => new
                {
                    LienHeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChuDe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LienHes", x => x.LienHeId);
                });

            migrationBuilder.CreateTable(
                name: "NhanViens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    QueQuan = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ViTri = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Facebook = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Twitter = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LinkedIn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Instagram = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AnhNhanVien = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HienThiTrangChu = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanViens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhanHois",
                columns: table => new
                {
                    phanHoiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    khachHangId = table.Column<int>(type: "int", nullable: false),
                    dichVuId = table.Column<int>(type: "int", nullable: false),
                    diemDanhGia = table.Column<int>(type: "int", nullable: false),
                    noiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ngayDanhGia = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanHois", x => x.phanHoiId);
                });

            migrationBuilder.CreateTable(
                name: "ComboDichVuDichVu",
                columns: table => new
                {
                    ComboDichVuId = table.Column<int>(type: "int", nullable: false),
                    DichVuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboDichVuDichVu", x => new { x.ComboDichVuId, x.DichVuId });
                    table.ForeignKey(
                        name: "FK_ComboDichVuDichVu_ComboDichVus_ComboDichVuId",
                        column: x => x.ComboDichVuId,
                        principalTable: "ComboDichVus",
                        principalColumn: "ComboDichVuId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComboDichVuDichVu_DichVus_DichVuId",
                        column: x => x.DichVuId,
                        principalTable: "DichVus",
                        principalColumn: "DichVuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComboDichVuDichVu_DichVuId",
                table: "ComboDichVuDichVu",
                column: "DichVuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComboDichVuDichVu");

            migrationBuilder.DropTable(
                name: "KhachHangs");

            migrationBuilder.DropTable(
                name: "LichHens");

            migrationBuilder.DropTable(
                name: "LienHes");

            migrationBuilder.DropTable(
                name: "NhanViens");

            migrationBuilder.DropTable(
                name: "PhanHois");

            migrationBuilder.DropTable(
                name: "ComboDichVus");

            migrationBuilder.DropTable(
                name: "DichVus");
        }
    }
}
