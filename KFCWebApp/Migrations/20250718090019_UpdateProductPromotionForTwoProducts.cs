using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFCWebApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductPromotionForTwoProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "ProductPromotions",
                newName: "ProductId2");

            migrationBuilder.AddColumn<int>(
                name: "Product1Id",
                table: "ProductPromotions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Product2Id",
                table: "ProductPromotions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductId1",
                table: "ProductPromotions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductPromotions_Product1Id",
                table: "ProductPromotions",
                column: "Product1Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPromotions_Product2Id",
                table: "ProductPromotions",
                column: "Product2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPromotions_Products_Product1Id",
                table: "ProductPromotions",
                column: "Product1Id",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPromotions_Products_Product2Id",
                table: "ProductPromotions",
                column: "Product2Id",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPromotions_Products_Product1Id",
                table: "ProductPromotions");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPromotions_Products_Product2Id",
                table: "ProductPromotions");

            migrationBuilder.DropIndex(
                name: "IX_ProductPromotions_Product1Id",
                table: "ProductPromotions");

            migrationBuilder.DropIndex(
                name: "IX_ProductPromotions_Product2Id",
                table: "ProductPromotions");

            migrationBuilder.DropColumn(
                name: "Product1Id",
                table: "ProductPromotions");

            migrationBuilder.DropColumn(
                name: "Product2Id",
                table: "ProductPromotions");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "ProductPromotions");

            migrationBuilder.RenameColumn(
                name: "ProductId2",
                table: "ProductPromotions",
                newName: "ProductId");
        }
    }
}
