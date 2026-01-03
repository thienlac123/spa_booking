using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnSPA.Migrations
{
    /// <inheritdoc />
    public partial class Add_Combo_SingleBooking_XOR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichHens_DichVus_dichVuId",
                table: "LichHens");

            migrationBuilder.AlterColumn<string>(
                name: "trangThai",
                table: "LichHens",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "dichVuId",
                table: "LichHens",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "TenNguoiDat",
                table: "LichHens",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "LichHens",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "comboId",
                table: "LichHens",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenCombo",
                table: "ComboDichVus",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_LichHens_comboId",
                table: "LichHens",
                column: "comboId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_LichHen_DichVu_Combo_XOR",
                table: "LichHens",
                sql: "(CASE WHEN [dichVuId] IS NOT NULL THEN 1 ELSE 0 END\r\n             + CASE WHEN [comboId]  IS NOT NULL THEN 1 ELSE 0 END) = 1");

            migrationBuilder.AddForeignKey(
                name: "FK_LichHens_ComboDichVus_comboId",
                table: "LichHens",
                column: "comboId",
                principalTable: "ComboDichVus",
                principalColumn: "ComboDichVuId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LichHens_DichVus_dichVuId",
                table: "LichHens",
                column: "dichVuId",
                principalTable: "DichVus",
                principalColumn: "DichVuId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichHens_ComboDichVus_comboId",
                table: "LichHens");

            migrationBuilder.DropForeignKey(
                name: "FK_LichHens_DichVus_dichVuId",
                table: "LichHens");

            migrationBuilder.DropIndex(
                name: "IX_LichHens_comboId",
                table: "LichHens");

            migrationBuilder.DropCheckConstraint(
                name: "CK_LichHen_DichVu_Combo_XOR",
                table: "LichHens");

            migrationBuilder.DropColumn(
                name: "comboId",
                table: "LichHens");

            migrationBuilder.AlterColumn<string>(
                name: "trangThai",
                table: "LichHens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<int>(
                name: "dichVuId",
                table: "LichHens",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenNguoiDat",
                table: "LichHens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "LichHens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "TenCombo",
                table: "ComboDichVus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddForeignKey(
                name: "FK_LichHens_DichVus_dichVuId",
                table: "LichHens",
                column: "dichVuId",
                principalTable: "DichVus",
                principalColumn: "DichVuId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
