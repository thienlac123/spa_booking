using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnSPA.Migrations
{
    /// <inheritdoc />
    public partial class FixBookingIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LichHen_Customer_Time",
                table: "LichHens");

            migrationBuilder.DropIndex(
                name: "IX_LichHens_ngayHen_gioHen",
                table: "LichHens");

            migrationBuilder.DropIndex(
                name: "IX_LichHens_nhanVienId",
                table: "LichHens");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_Customer_Time",
                table: "LichHens",
                columns: new[] { "customerId", "ngayHen", "gioHen" },
                unique: true,
                filter: "[trangThai] <> 'DaHuy'");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_Time",
                table: "LichHens",
                columns: new[] { "ngayHen", "gioHen" });

            migrationBuilder.CreateIndex(
                name: "IX_LichHens_nhanVienId_ngayHen_gioHen",
                table: "LichHens",
                columns: new[] { "nhanVienId", "ngayHen", "gioHen" },
                unique: true,
                filter: "[nhanVienId] IS NOT NULL AND [trangThai] <> 'DaHuy'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LichHen_Customer_Time",
                table: "LichHens");

            migrationBuilder.DropIndex(
                name: "IX_LichHen_Time",
                table: "LichHens");

            migrationBuilder.DropIndex(
                name: "IX_LichHens_nhanVienId_ngayHen_gioHen",
                table: "LichHens");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_Customer_Time",
                table: "LichHens",
                columns: new[] { "customerId", "ngayHen", "gioHen" });

            migrationBuilder.CreateIndex(
                name: "IX_LichHens_ngayHen_gioHen",
                table: "LichHens",
                columns: new[] { "ngayHen", "gioHen" },
                unique: true,
                filter: "[trangThai] <> 'DaHuy'");

            migrationBuilder.CreateIndex(
                name: "IX_LichHens_nhanVienId",
                table: "LichHens",
                column: "nhanVienId");
        }
    }
}
