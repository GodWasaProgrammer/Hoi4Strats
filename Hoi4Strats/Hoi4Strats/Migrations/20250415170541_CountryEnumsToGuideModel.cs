using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoi4Strats.Migrations
{
    /// <inheritdoc />
    public partial class CountryEnumsToGuideModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MajorCountry",
                table: "Guides",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinorCountry",
                table: "Guides",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MajorCountry",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "MinorCountry",
                table: "Guides");
        }
    }
}
