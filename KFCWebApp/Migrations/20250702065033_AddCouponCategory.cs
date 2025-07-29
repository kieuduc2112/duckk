using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFCWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCouponCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CouponCategoryId",
                table: "Coupons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Coupons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "CouponCategory",
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
                    table.PrimaryKey("PK_CouponCategory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_CouponCategoryId",
                table: "Coupons",
                column: "CouponCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_CouponCategory_CouponCategoryId",
                table: "Coupons",
                column: "CouponCategoryId",
                principalTable: "CouponCategory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_CouponCategory_CouponCategoryId",
                table: "Coupons");

            migrationBuilder.DropTable(
                name: "CouponCategory");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_CouponCategoryId",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "CouponCategoryId",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Coupons");
        }
    }
}
