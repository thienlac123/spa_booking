using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnSPA.Migrations
{
    /// <inheritdoc />
    public partial class AddLichHenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LichHens",
                table: "LichHens");

            migrationBuilder.RenameTable(
                name: "LichHens",
                newName: "LichHen");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LichHen",
                table: "LichHen",
                column: "lichHenId");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_dichVuId",
                table: "LichHen",
                column: "dichVuId");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_nhanVienId",
                table: "LichHen",
                column: "nhanVienId");

            migrationBuilder.AddForeignKey(
                name: "FK_LichHen_DichVus_dichVuId",
                table: "LichHen",
                column: "dichVuId",
                principalTable: "DichVus",
                principalColumn: "DichVuId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LichHen_NhanViens_nhanVienId",
                table: "LichHen",
                column: "nhanVienId",
                principalTable: "NhanViens",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichHen_DichVus_dichVuId",
                table: "LichHen");

            migrationBuilder.DropForeignKey(
                name: "FK_LichHen_NhanViens_nhanVienId",
                table: "LichHen");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LichHen",
                table: "LichHen");

            migrationBuilder.DropIndex(
                name: "IX_LichHen_dichVuId",
                table: "LichHen");

            migrationBuilder.DropIndex(
                name: "IX_LichHen_nhanVienId",
                table: "LichHen");

            migrationBuilder.RenameTable(
                name: "LichHen",
                newName: "LichHens");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LichHens",
                table: "LichHens",
                column: "lichHenId");
        }
    }
}
