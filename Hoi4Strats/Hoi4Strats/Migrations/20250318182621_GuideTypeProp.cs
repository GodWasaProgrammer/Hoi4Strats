using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoi4Strats.Migrations
{
    /// <inheritdoc />
    public partial class GuideTypeProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GuideType",
                table: "Guides",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuideType",
                table: "Guides");
        }
    }
}
