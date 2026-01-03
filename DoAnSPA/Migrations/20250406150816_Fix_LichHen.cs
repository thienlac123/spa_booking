using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnSPA.Migrations
{
    /// <inheritdoc />
    public partial class Fix_LichHen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameTable(
                name: "LichHen",
                newName: "LichHens");

            migrationBuilder.RenameIndex(
                name: "IX_LichHen_nhanVienId",
                table: "LichHens",
                newName: "IX_LichHens_nhanVienId");

            migrationBuilder.RenameIndex(
                name: "IX_LichHen_dichVuId",
                table: "LichHens",
                newName: "IX_LichHens_dichVuId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LichHens",
                table: "LichHens",
                column: "lichHenId");

            migrationBuilder.AddForeignKey(
                name: "FK_LichHens_DichVus_dichVuId",
                table: "LichHens",
                column: "dichVuId",
                principalTable: "DichVus",
                principalColumn: "DichVuId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LichHens_NhanViens_nhanVienId",
                table: "LichHens",
                column: "nhanVienId",
                principalTable: "NhanViens",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichHens_DichVus_dichVuId",
                table: "LichHens");

            migrationBuilder.DropForeignKey(
                name: "FK_LichHens_NhanViens_nhanVienId",
                table: "LichHens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LichHens",
                table: "LichHens");

            migrationBuilder.RenameTable(
                name: "LichHens",
                newName: "LichHen");

            migrationBuilder.RenameIndex(
                name: "IX_LichHens_nhanVienId",
                table: "LichHen",
                newName: "IX_LichHen_nhanVienId");

            migrationBuilder.RenameIndex(
                name: "IX_LichHens_dichVuId",
                table: "LichHen",
                newName: "IX_LichHen_dichVuId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LichHen",
                table: "LichHen",
                column: "lichHenId");

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
    }
}
