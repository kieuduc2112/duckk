using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFCWebApp.Migrations
{
    /// <inheritdoc />
    public partial class RenameCouponCategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_CouponCategory_CouponCategoryId",
                table: "Coupons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CouponCategory",
                table: "CouponCategory");

            migrationBuilder.RenameTable(
                name: "CouponCategory",
                newName: "CouponCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CouponCategories",
                table: "CouponCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_CouponCategories_CouponCategoryId",
                table: "Coupons",
                column: "CouponCategoryId",
                principalTable: "CouponCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_CouponCategories_CouponCategoryId",
                table: "Coupons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CouponCategories",
                table: "CouponCategories");

            migrationBuilder.RenameTable(
                name: "CouponCategories",
                newName: "CouponCategory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CouponCategory",
                table: "CouponCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_CouponCategory_CouponCategoryId",
                table: "Coupons",
                column: "CouponCategoryId",
                principalTable: "CouponCategory",
                principalColumn: "Id");
        }
    }
}
