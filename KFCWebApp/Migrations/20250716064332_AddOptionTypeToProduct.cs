using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFCWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddOptionTypeToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OptionType",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OptionType",
                table: "Products");
        }
    }
}
