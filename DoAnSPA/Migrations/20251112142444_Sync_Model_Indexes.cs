using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnSPA.Migrations
{
    /// <inheritdoc />
    public partial class Sync_Model_Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_LichHen_DichVu_Combo_XOR",
                table: "LichHens");

            migrationBuilder.AddCheckConstraint(
                name: "CK_LichHen_DichVu_Combo_XOR",
                table: "LichHens",
                sql: "(CASE WHEN [dichVuId] IS NOT NULL THEN 1 ELSE 0 END\r\n              + CASE WHEN [comboId]  IS NOT NULL THEN 1 ELSE 0 END) = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_LichHen_DichVu_Combo_XOR",
                table: "LichHens");

            migrationBuilder.AddCheckConstraint(
                name: "CK_LichHen_DichVu_Combo_XOR",
                table: "LichHens",
                sql: "(CASE WHEN [dichVuId] IS NOT NULL THEN 1 ELSE 0 END\r\n             + CASE WHEN [comboId]  IS NOT NULL THEN 1 ELSE 0 END) = 1");
        }
    }
}
