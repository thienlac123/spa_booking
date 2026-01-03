using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnSPA.Migrations
{
    /// <inheritdoc />
    public partial class Add_Unique_Slot_Global : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LichHens_ngayHen_gioHen",
                table: "LichHens",
                columns: new[] { "ngayHen", "gioHen" },
                unique: true,
                filter: "[trangThai] <> 'DaHuy'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LichHens_ngayHen_gioHen",
                table: "LichHens");
        }
    }
}
