using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchIQ.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProcessManufacturingSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProcessedProduct",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ProcessTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ProcessType = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExpectedYieldPercentage = table.Column<decimal>(type: "TEXT", nullable: true),
                    StandardProcessingTimeMinutes = table.Column<int>(type: "INTEGER", nullable: true),
                    Instructions = table.Column<string>(type: "TEXT", nullable: true),
                    QualityCheckpoints = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionBatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BatchNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ProcessType = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LocationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    TotalInputCost = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    ProcessingCost = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    YieldEfficiency = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    QualityNotes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    SupervisorId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CompletionDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionBatches_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProcessTemplateInputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProcessTemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    StandardQuantity = table.Column<decimal>(type: "TEXT", nullable: true),
                    StandardUnit = table.Column<int>(type: "INTEGER", nullable: true),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessTemplateInputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessTemplateInputs_ProcessTemplates_ProcessTemplateId",
                        column: x => x.ProcessTemplateId,
                        principalTable: "ProcessTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessTemplateInputs_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessTemplateOutputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProcessTemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExpectedYieldPercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessTemplateOutputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessTemplateOutputs_ProcessTemplates_ProcessTemplateId",
                        column: x => x.ProcessTemplateId,
                        principalTable: "ProcessTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessTemplateOutputs_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BatchInputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductionBatchId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantityUsed = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Unit = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseQuantityUsed = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CostPerUnit = table.Column<decimal>(type: "decimal(12,4)", nullable: false),
                    SourceLocationId = table.Column<int>(type: "INTEGER", nullable: true),
                    SourceBatchNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchInputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchInputs_Locations_SourceLocationId",
                        column: x => x.SourceLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BatchInputs_ProductionBatches_ProductionBatchId",
                        column: x => x.ProductionBatchId,
                        principalTable: "ProductionBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchInputs_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BatchOutputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductionBatchId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantityProduced = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Unit = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseQuantityProduced = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    YieldPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    CostPerUnit = table.Column<decimal>(type: "decimal(12,4)", nullable: true),
                    TotalAllocatedCost = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    DestinationLocationId = table.Column<int>(type: "INTEGER", nullable: true),
                    QualityGrade = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    QuantityVariance = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchOutputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchOutputs_Locations_DestinationLocationId",
                        column: x => x.DestinationLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BatchOutputs_ProductionBatches_ProductionBatchId",
                        column: x => x.ProductionBatchId,
                        principalTable: "ProductionBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchOutputs_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsProcessedProduct",
                table: "Products",
                column: "IsProcessedProduct");

            migrationBuilder.CreateIndex(
                name: "IX_BatchInputs_ProductId",
                table: "BatchInputs",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchInputs_ProductionBatchId",
                table: "BatchInputs",
                column: "ProductionBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchInputs_SourceBatchNumber",
                table: "BatchInputs",
                column: "SourceBatchNumber");

            migrationBuilder.CreateIndex(
                name: "IX_BatchInputs_SourceLocationId",
                table: "BatchInputs",
                column: "SourceLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchOutputs_DestinationLocationId",
                table: "BatchOutputs",
                column: "DestinationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchOutputs_ProductId",
                table: "BatchOutputs",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchOutputs_ProductionBatchId",
                table: "BatchOutputs",
                column: "ProductionBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchOutputs_QualityGrade",
                table: "BatchOutputs",
                column: "QualityGrade");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessTemplateInputs_ProcessTemplateId",
                table: "ProcessTemplateInputs",
                column: "ProcessTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessTemplateInputs_ProductId",
                table: "ProcessTemplateInputs",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessTemplateOutputs_ProcessTemplateId",
                table: "ProcessTemplateOutputs",
                column: "ProcessTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessTemplateOutputs_ProductId",
                table: "ProcessTemplateOutputs",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionBatches_BatchNumber",
                table: "ProductionBatches",
                column: "BatchNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductionBatches_LocationId",
                table: "ProductionBatches",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionBatches_ProcessType",
                table: "ProductionBatches",
                column: "ProcessType");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionBatches_ProcessType_ProductionDate",
                table: "ProductionBatches",
                columns: new[] { "ProcessType", "ProductionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductionBatches_ProductionDate",
                table: "ProductionBatches",
                column: "ProductionDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionBatches_Status",
                table: "ProductionBatches",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatchInputs");

            migrationBuilder.DropTable(
                name: "BatchOutputs");

            migrationBuilder.DropTable(
                name: "ProcessTemplateInputs");

            migrationBuilder.DropTable(
                name: "ProcessTemplateOutputs");

            migrationBuilder.DropTable(
                name: "ProductionBatches");

            migrationBuilder.DropTable(
                name: "ProcessTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Products_IsProcessedProduct",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsProcessedProduct",
                table: "Products");
        }
    }
}
