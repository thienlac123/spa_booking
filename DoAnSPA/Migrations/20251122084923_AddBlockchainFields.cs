using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnSPA.Migrations
{
    /// <inheritdoc />
    public partial class AddBlockchainFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlockchainId",
                table: "LichHens",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CancelBefore",
                table: "LichHens",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GraceUntil",
                table: "LichHens",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockchainId",
                table: "LichHens");

            migrationBuilder.DropColumn(
                name: "CancelBefore",
                table: "LichHens");

            migrationBuilder.DropColumn(
                name: "GraceUntil",
                table: "LichHens");
        }
    }
}
