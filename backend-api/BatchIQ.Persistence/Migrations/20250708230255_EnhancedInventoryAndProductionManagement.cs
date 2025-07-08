using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchIQ.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnhancedInventoryAndProductionManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Locations_FromLocationId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Locations_ToLocationId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Products_ProductId",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_FromLocationId",
                table: "InventoryTransactions");

            migrationBuilder.AlterColumn<int>(
                name: "UnitType",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 10,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "BaseUnit",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 10);

            migrationBuilder.AddColumn<bool>(
                name: "IsManufactured",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ProductType",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<decimal>(
                name: "BaseQuantity",
                table: "InventoryTransactions",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BatchNumber",
                table: "InventoryTransactions",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductionTransactionId",
                table: "InventoryTransactions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Unit",
                table: "InventoryTransactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 10);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "InventoryTransactions",
                type: "decimal(12,4)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductBOMs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    ComponentProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantityRequired = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Unit = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 10),
                    CostPerUnit = table.Column<decimal>(type: "decimal(12,4)", nullable: true),
                    IsCritical = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBOMs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductBOMs_Products_ComponentProductId",
                        column: x => x.ComponentProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductBOMs_Products_ParentProductId",
                        column: x => x.ParentProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantityProduced = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Unit = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 10),
                    BaseQuantityProduced = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    LocationId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BatchNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    TotalMaterialCost = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    ProductionCost = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    QualityNotes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    SupervisorId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionTransactions_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductionTransactions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsManufactured",
                table: "Products",
                column: "IsManufactured");

            migrationBuilder.CreateIndex(
                name: "IX_Products_NameEn_BrandEn",
                table: "Products",
                columns: new[] { "NameEn", "BrandEn" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_NameFa_BrandFa",
                table: "Products",
                columns: new[] { "NameFa", "BrandFa" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductType",
                table: "Products",
                column: "ProductType");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_BatchNumber",
                table: "InventoryTransactions",
                column: "BatchNumber");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_FromLocationId_ToLocationId",
                table: "InventoryTransactions",
                columns: new[] { "FromLocationId", "ToLocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_ProductId_Timestamp",
                table: "InventoryTransactions",
                columns: new[] { "ProductId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_ProductionTransactionId",
                table: "InventoryTransactions",
                column: "ProductionTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_TransactionType",
                table: "InventoryTransactions",
                column: "TransactionType");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBOM_Parent_Component",
                table: "ProductBOMs",
                columns: new[] { "ParentProductId", "ComponentProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductBOMs_ComponentProductId",
                table: "ProductBOMs",
                column: "ComponentProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBOMs_IsCritical",
                table: "ProductBOMs",
                column: "IsCritical");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBOMs_ParentProductId",
                table: "ProductBOMs",
                column: "ParentProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBOMs_Sequence",
                table: "ProductBOMs",
                column: "Sequence");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionTransactions_BatchNumber",
                table: "ProductionTransactions",
                column: "BatchNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductionTransactions_LocationId",
                table: "ProductionTransactions",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionTransactions_ProductId",
                table: "ProductionTransactions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionTransactions_ProductId_ProductionDate",
                table: "ProductionTransactions",
                columns: new[] { "ProductId", "ProductionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductionTransactions_ProductionDate",
                table: "ProductionTransactions",
                column: "ProductionDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionTransactions_Status",
                table: "ProductionTransactions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionTransactions_Status_ProductionDate",
                table: "ProductionTransactions",
                columns: new[] { "Status", "ProductionDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Locations_FromLocationId",
                table: "InventoryTransactions",
                column: "FromLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Locations_ToLocationId",
                table: "InventoryTransactions",
                column: "ToLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_ProductionTransactions_ProductionTransactionId",
                table: "InventoryTransactions",
                column: "ProductionTransactionId",
                principalTable: "ProductionTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Products_ProductId",
                table: "InventoryTransactions",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Locations_FromLocationId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Locations_ToLocationId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_ProductionTransactions_ProductionTransactionId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Products_ProductId",
                table: "InventoryTransactions");

            migrationBuilder.DropTable(
                name: "ProductBOMs");

            migrationBuilder.DropTable(
                name: "ProductionTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Products_IsManufactured",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_NameEn_BrandEn",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_NameFa_BrandFa",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductType",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_BatchNumber",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_FromLocationId_ToLocationId",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_ProductId_Timestamp",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_ProductionTransactionId",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_TransactionType",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "BaseUnit",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsManufactured",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductType",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BaseQuantity",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "BatchNumber",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "ProductionTransactionId",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "InventoryTransactions");

            migrationBuilder.AlterColumn<int>(
                name: "UnitType",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 10);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_FromLocationId",
                table: "InventoryTransactions",
                column: "FromLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Locations_FromLocationId",
                table: "InventoryTransactions",
                column: "FromLocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Locations_ToLocationId",
                table: "InventoryTransactions",
                column: "ToLocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Products_ProductId",
                table: "InventoryTransactions",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
