using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnSPA.Migrations
{
    /// <inheritdoc />
    public partial class LichHenUP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionHash",
                table: "LichHens",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionHash",
                table: "LichHens");
        }
    }
}
