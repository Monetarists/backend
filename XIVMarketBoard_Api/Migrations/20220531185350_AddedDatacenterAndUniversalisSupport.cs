using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XIVMarketBoard_Api.Migrations
{
    public partial class AddedDatacenterAndUniversalisSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Recipies_RecipeId",
                table: "Ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipies_Items_ItemId",
                table: "Recipies");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipies_Jobs_jobId",
                table: "Recipies");

            migrationBuilder.DropForeignKey(
                name: "FK_Retainers_Servers_WorldName",
                table: "Retainers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Servers",
                table: "Servers");

            migrationBuilder.DropIndex(
                name: "IX_Retainers_WorldName",
                table: "Retainers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recipies",
                table: "Recipies");

            migrationBuilder.DropColumn(
                name: "DataCenter",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "WorldName",
                table: "Retainers");

            migrationBuilder.DropColumn(
                name: "LastSeen",
                table: "Items");

            migrationBuilder.RenameTable(
                name: "Recipies",
                newName: "Recipes");

            migrationBuilder.RenameIndex(
                name: "IX_Recipies_jobId",
                table: "Recipes",
                newName: "IX_Recipes_jobId");

            migrationBuilder.RenameIndex(
                name: "IX_Recipies_ItemId",
                table: "Recipes",
                newName: "IX_Recipes_ItemId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Servers",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Servers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "DataCenterId",
                table: "Servers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UniversalisQueryId",
                table: "SaleHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorldId",
                table: "SaleHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorldId",
                table: "Retainers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "QueryDate",
                table: "MbPosts",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UniversalisQueryId",
                table: "MbPosts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorldId",
                table: "MbPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Servers",
                table: "Servers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DataCenters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Region = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataCenters", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UniversalisQueries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    itemId = table.Column<int>(type: "int", nullable: false),
                    worldId = table.Column<int>(type: "int", nullable: false),
                    lastUploadDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    QueryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    currentAveragePrice = table.Column<int>(type: "int", nullable: false),
                    currentAveragePrinceNQ = table.Column<int>(type: "int", nullable: false),
                    currentAveragePriceHQ = table.Column<int>(type: "int", nullable: false),
                    regularSaleVelocity = table.Column<int>(type: "int", nullable: false),
                    nqSaleVelocity = table.Column<int>(type: "int", nullable: false),
                    hqSaleVelocity = table.Column<int>(type: "int", nullable: false),
                    averagePrice = table.Column<int>(type: "int", nullable: false),
                    averagePriceNQ = table.Column<int>(type: "int", nullable: false),
                    averagePriceHQ = table.Column<int>(type: "int", nullable: false),
                    minPrice = table.Column<int>(type: "int", nullable: false),
                    minPriceNQ = table.Column<int>(type: "int", nullable: false),
                    minPriceHQ = table.Column<int>(type: "int", nullable: false),
                    maxPrice = table.Column<int>(type: "int", nullable: false),
                    maxPriceNQ = table.Column<int>(type: "int", nullable: false),
                    maxPriceHQ = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniversalisQueries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UniversalisQueries_Items_itemId",
                        column: x => x.itemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniversalisQueries_Servers_worldId",
                        column: x => x.worldId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_DataCenterId",
                table: "Servers",
                column: "DataCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleHistory_UniversalisQueryId",
                table: "SaleHistory",
                column: "UniversalisQueryId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleHistory_WorldId",
                table: "SaleHistory",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Retainers_WorldId",
                table: "Retainers",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_MbPosts_UniversalisQueryId",
                table: "MbPosts",
                column: "UniversalisQueryId");

            migrationBuilder.CreateIndex(
                name: "IX_MbPosts_WorldId",
                table: "MbPosts",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalisQueries_itemId",
                table: "UniversalisQueries",
                column: "itemId");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalisQueries_worldId",
                table: "UniversalisQueries",
                column: "worldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Recipes_RecipeId",
                table: "Ingredients",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MbPosts_Servers_WorldId",
                table: "MbPosts",
                column: "WorldId",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MbPosts_UniversalisQueries_UniversalisQueryId",
                table: "MbPosts",
                column: "UniversalisQueryId",
                principalTable: "UniversalisQueries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Items_ItemId",
                table: "Recipes",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Jobs_jobId",
                table: "Recipes",
                column: "jobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Retainers_Servers_WorldId",
                table: "Retainers",
                column: "WorldId",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleHistory_Servers_WorldId",
                table: "SaleHistory",
                column: "WorldId",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleHistory_UniversalisQueries_UniversalisQueryId",
                table: "SaleHistory",
                column: "UniversalisQueryId",
                principalTable: "UniversalisQueries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_DataCenters_DataCenterId",
                table: "Servers",
                column: "DataCenterId",
                principalTable: "DataCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Recipes_RecipeId",
                table: "Ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_MbPosts_Servers_WorldId",
                table: "MbPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_MbPosts_UniversalisQueries_UniversalisQueryId",
                table: "MbPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Items_ItemId",
                table: "Recipes");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Jobs_jobId",
                table: "Recipes");

            migrationBuilder.DropForeignKey(
                name: "FK_Retainers_Servers_WorldId",
                table: "Retainers");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleHistory_Servers_WorldId",
                table: "SaleHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleHistory_UniversalisQueries_UniversalisQueryId",
                table: "SaleHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Servers_DataCenters_DataCenterId",
                table: "Servers");

            migrationBuilder.DropTable(
                name: "DataCenters");

            migrationBuilder.DropTable(
                name: "UniversalisQueries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Servers",
                table: "Servers");

            migrationBuilder.DropIndex(
                name: "IX_Servers_DataCenterId",
                table: "Servers");

            migrationBuilder.DropIndex(
                name: "IX_SaleHistory_UniversalisQueryId",
                table: "SaleHistory");

            migrationBuilder.DropIndex(
                name: "IX_SaleHistory_WorldId",
                table: "SaleHistory");

            migrationBuilder.DropIndex(
                name: "IX_Retainers_WorldId",
                table: "Retainers");

            migrationBuilder.DropIndex(
                name: "IX_MbPosts_UniversalisQueryId",
                table: "MbPosts");

            migrationBuilder.DropIndex(
                name: "IX_MbPosts_WorldId",
                table: "MbPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "DataCenterId",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "UniversalisQueryId",
                table: "SaleHistory");

            migrationBuilder.DropColumn(
                name: "WorldId",
                table: "SaleHistory");

            migrationBuilder.DropColumn(
                name: "WorldId",
                table: "Retainers");

            migrationBuilder.DropColumn(
                name: "QueryDate",
                table: "MbPosts");

            migrationBuilder.DropColumn(
                name: "UniversalisQueryId",
                table: "MbPosts");

            migrationBuilder.DropColumn(
                name: "WorldId",
                table: "MbPosts");

            migrationBuilder.RenameTable(
                name: "Recipes",
                newName: "Recipies");

            migrationBuilder.RenameIndex(
                name: "IX_Recipes_jobId",
                table: "Recipies",
                newName: "IX_Recipies_jobId");

            migrationBuilder.RenameIndex(
                name: "IX_Recipes_ItemId",
                table: "Recipies",
                newName: "IX_Recipies_ItemId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Servers",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DataCenter",
                table: "Servers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "WorldName",
                table: "Retainers",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSeen",
                table: "Items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Servers",
                table: "Servers",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recipies",
                table: "Recipies",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Retainers_WorldName",
                table: "Retainers",
                column: "WorldName");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Recipies_RecipeId",
                table: "Ingredients",
                column: "RecipeId",
                principalTable: "Recipies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipies_Items_ItemId",
                table: "Recipies",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipies_Jobs_jobId",
                table: "Recipies",
                column: "jobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Retainers_Servers_WorldName",
                table: "Retainers",
                column: "WorldName",
                principalTable: "Servers",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
