using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFCWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddComboSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "OrderDetails",
                newName: "ItemType");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "OrderDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ComboId",
                table: "OrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ComboId1",
                table: "OrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ComboCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Combos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OriginalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ComboCategoryId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Combos_ComboCategories_ComboCategoryId",
                        column: x => x.ComboCategoryId,
                        principalTable: "ComboCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComboItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComboId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsExchangeable = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComboItems_Combos_ComboId",
                        column: x => x.ComboId,
                        principalTable: "Combos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComboItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ComboId",
                table: "OrderDetails",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ComboId1",
                table: "OrderDetails",
                column: "ComboId1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderDetail_ProductOrCombo",
                table: "OrderDetails",
                sql: "(ProductId IS NOT NULL AND ComboId IS NULL) OR (ProductId IS NULL AND ComboId IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_ComboItems_ComboId",
                table: "ComboItems",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboItems_ProductId",
                table: "ComboItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Combos_ComboCategoryId",
                table: "Combos",
                column: "ComboCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Combos_ComboId",
                table: "OrderDetails",
                column: "ComboId",
                principalTable: "Combos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Combos_ComboId1",
                table: "OrderDetails",
                column: "ComboId1",
                principalTable: "Combos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Combos_ComboId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Combos_ComboId1",
                table: "OrderDetails");

            migrationBuilder.DropTable(
                name: "ComboItems");

            migrationBuilder.DropTable(
                name: "Combos");

            migrationBuilder.DropTable(
                name: "ComboCategories");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ComboId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ComboId1",
                table: "OrderDetails");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderDetail_ProductOrCombo",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ComboId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ComboId1",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "ItemType",
                table: "OrderDetails",
                newName: "ProductName");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
