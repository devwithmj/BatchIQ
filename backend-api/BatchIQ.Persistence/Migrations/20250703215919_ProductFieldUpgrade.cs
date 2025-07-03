using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchIQ.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProductFieldUpgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SizeUnit",
                table: "Products",
                newName: "UnitType");

            migrationBuilder.RenameColumn(
                name: "Size",
                table: "Products",
                newName: "SizeValue");

            migrationBuilder.RenameColumn(
                name: "PersianName",
                table: "Products",
                newName: "NameFa");

            migrationBuilder.RenameColumn(
                name: "Brand",
                table: "Products",
                newName: "BrandFa");

            migrationBuilder.AddColumn<string>(
                name: "BrandEn",
                table: "Products",
                type: "TEXT",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Products",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(12,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrandEn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "UnitType",
                table: "Products",
                newName: "SizeUnit");

            migrationBuilder.RenameColumn(
                name: "SizeValue",
                table: "Products",
                newName: "Size");

            migrationBuilder.RenameColumn(
                name: "NameFa",
                table: "Products",
                newName: "PersianName");

            migrationBuilder.RenameColumn(
                name: "BrandFa",
                table: "Products",
                newName: "Brand");
        }
    }
}
