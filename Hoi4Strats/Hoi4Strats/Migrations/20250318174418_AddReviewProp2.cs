using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoi4Strats.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewProp2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Guides",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Guides");
        }
    }
}
