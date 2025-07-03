using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoi4Strats.Migrations
{
    /// <inheritdoc />
    public partial class benny : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TypeofTemplate",
                table: "Guides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypeofTemplate",
                table: "Guides");
        }
    }
}
