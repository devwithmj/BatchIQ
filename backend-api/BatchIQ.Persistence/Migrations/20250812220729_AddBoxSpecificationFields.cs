using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchIQ.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBoxSpecificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BoxDescription",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BoxUnit",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BoxWeight",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PiecesPerBox",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoxDescription",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BoxUnit",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BoxWeight",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PiecesPerBox",
                table: "Products");
        }
    }
}
