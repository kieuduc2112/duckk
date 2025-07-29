using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFCWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddProductAndComboPromotionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComboPromotions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RewardType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RewardValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RewardProductId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboPromotions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductPromotions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    PercentDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AmountDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPromotions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComboPromotionItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComboPromotionId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboPromotionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComboPromotionItems_ComboPromotions_ComboPromotionId",
                        column: x => x.ComboPromotionId,
                        principalTable: "ComboPromotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComboPromotionItems_ComboPromotionId",
                table: "ComboPromotionItems",
                column: "ComboPromotionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComboPromotionItems");

            migrationBuilder.DropTable(
                name: "ProductPromotions");

            migrationBuilder.DropTable(
                name: "ComboPromotions");
        }
    }
}
