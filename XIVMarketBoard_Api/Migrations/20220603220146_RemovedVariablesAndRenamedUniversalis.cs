using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XIVMarketBoard_Api.Migrations
{
    public partial class RemovedVariablesAndRenamedUniversalis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Items_ItemId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_UniversalisQueries_UniversalisQueryId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Worlds_WorldId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleHistory_Items_ItemId",
                table: "SaleHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleHistory_UniversalisQueries_UniversalisQueryId",
                table: "SaleHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleHistory_Worlds_WorldId",
                table: "SaleHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversalisQueries_Items_itemId",
                table: "UniversalisQueries");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversalisQueries_Worlds_worldId",
                table: "UniversalisQueries");

            migrationBuilder.DropIndex(
                name: "IX_SaleHistory_ItemId",
                table: "SaleHistory");

            migrationBuilder.DropIndex(
                name: "IX_SaleHistory_WorldId",
                table: "SaleHistory");

            migrationBuilder.DropIndex(
                name: "IX_Posts_ItemId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_WorldId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "PostedDate",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "WorldId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "worldId",
                table: "UniversalisQueries",
                newName: "WorldId");

            migrationBuilder.RenameColumn(
                name: "regularSaleVelocity",
                table: "UniversalisQueries",
                newName: "RegularSaleVelocity");

            migrationBuilder.RenameColumn(
                name: "nqSaleVelocity",
                table: "UniversalisQueries",
                newName: "NqSaleVelocity");

            migrationBuilder.RenameColumn(
                name: "minPriceNQ",
                table: "UniversalisQueries",
                newName: "MinPriceNQ");

            migrationBuilder.RenameColumn(
                name: "minPriceHQ",
                table: "UniversalisQueries",
                newName: "MinPriceHQ");

            migrationBuilder.RenameColumn(
                name: "minPrice",
                table: "UniversalisQueries",
                newName: "MinPrice");

            migrationBuilder.RenameColumn(
                name: "maxPriceNQ",
                table: "UniversalisQueries",
                newName: "MaxPriceNQ");

            migrationBuilder.RenameColumn(
                name: "maxPriceHQ",
                table: "UniversalisQueries",
                newName: "MaxPriceHQ");

            migrationBuilder.RenameColumn(
                name: "maxPrice",
                table: "UniversalisQueries",
                newName: "MaxPrice");

            migrationBuilder.RenameColumn(
                name: "lastUploadDate",
                table: "UniversalisQueries",
                newName: "LastUploadDate");

            migrationBuilder.RenameColumn(
                name: "itemId",
                table: "UniversalisQueries",
                newName: "ItemId");

            migrationBuilder.RenameColumn(
                name: "hqSaleVelocity",
                table: "UniversalisQueries",
                newName: "HqSaleVelocity");

            migrationBuilder.RenameColumn(
                name: "currentAveragePrinceNQ",
                table: "UniversalisQueries",
                newName: "CurrentAveragePrinceNQ");

            migrationBuilder.RenameColumn(
                name: "currentAveragePriceHQ",
                table: "UniversalisQueries",
                newName: "CurrentAveragePriceHQ");

            migrationBuilder.RenameColumn(
                name: "currentAveragePrice",
                table: "UniversalisQueries",
                newName: "CurrentAveragePrice");

            migrationBuilder.RenameColumn(
                name: "averagePriceNQ",
                table: "UniversalisQueries",
                newName: "AveragePriceNQ");

            migrationBuilder.RenameColumn(
                name: "averagePriceHQ",
                table: "UniversalisQueries",
                newName: "AveragePriceHQ");

            migrationBuilder.RenameColumn(
                name: "averagePrice",
                table: "UniversalisQueries",
                newName: "AveragePrice");

            migrationBuilder.RenameIndex(
                name: "IX_UniversalisQueries_worldId",
                table: "UniversalisQueries",
                newName: "IX_UniversalisQueries_WorldId");

            migrationBuilder.RenameIndex(
                name: "IX_UniversalisQueries_itemId",
                table: "UniversalisQueries",
                newName: "IX_UniversalisQueries_ItemId");

            migrationBuilder.RenameColumn(
                name: "WorldId",
                table: "SaleHistory",
                newName: "Total");

            migrationBuilder.RenameColumn(
                name: "UniversalisQueryId",
                table: "SaleHistory",
                newName: "UniversalisEntryId");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "SaleHistory",
                newName: "Quantity");

            migrationBuilder.RenameIndex(
                name: "IX_SaleHistory_UniversalisQueryId",
                table: "SaleHistory",
                newName: "IX_SaleHistory_UniversalisEntryId");

            migrationBuilder.RenameColumn(
                name: "UniversalisQueryId",
                table: "Posts",
                newName: "UniversalisEntryId");

            migrationBuilder.RenameColumn(
                name: "QueryDate",
                table: "Posts",
                newName: "LastReviewDate");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UniversalisQueryId",
                table: "Posts",
                newName: "IX_Posts_UniversalisEntryId");

            migrationBuilder.AddColumn<string>(
                name: "BuyerName",
                table: "SaleHistory",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Posts",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "SellerId",
                table: "Posts",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_UniversalisQueries_UniversalisEntryId",
                table: "Posts",
                column: "UniversalisEntryId",
                principalTable: "UniversalisQueries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleHistory_UniversalisQueries_UniversalisEntryId",
                table: "SaleHistory",
                column: "UniversalisEntryId",
                principalTable: "UniversalisQueries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UniversalisQueries_Items_ItemId",
                table: "UniversalisQueries",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UniversalisQueries_Worlds_WorldId",
                table: "UniversalisQueries",
                column: "WorldId",
                principalTable: "Worlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_UniversalisQueries_UniversalisEntryId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleHistory_UniversalisQueries_UniversalisEntryId",
                table: "SaleHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversalisQueries_Items_ItemId",
                table: "UniversalisQueries");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversalisQueries_Worlds_WorldId",
                table: "UniversalisQueries");

            migrationBuilder.DropColumn(
                name: "BuyerName",
                table: "SaleHistory");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "WorldId",
                table: "UniversalisQueries",
                newName: "worldId");

            migrationBuilder.RenameColumn(
                name: "RegularSaleVelocity",
                table: "UniversalisQueries",
                newName: "regularSaleVelocity");

            migrationBuilder.RenameColumn(
                name: "NqSaleVelocity",
                table: "UniversalisQueries",
                newName: "nqSaleVelocity");

            migrationBuilder.RenameColumn(
                name: "MinPriceNQ",
                table: "UniversalisQueries",
                newName: "minPriceNQ");

            migrationBuilder.RenameColumn(
                name: "MinPriceHQ",
                table: "UniversalisQueries",
                newName: "minPriceHQ");

            migrationBuilder.RenameColumn(
                name: "MinPrice",
                table: "UniversalisQueries",
                newName: "minPrice");

            migrationBuilder.RenameColumn(
                name: "MaxPriceNQ",
                table: "UniversalisQueries",
                newName: "maxPriceNQ");

            migrationBuilder.RenameColumn(
                name: "MaxPriceHQ",
                table: "UniversalisQueries",
                newName: "maxPriceHQ");

            migrationBuilder.RenameColumn(
                name: "MaxPrice",
                table: "UniversalisQueries",
                newName: "maxPrice");

            migrationBuilder.RenameColumn(
                name: "LastUploadDate",
                table: "UniversalisQueries",
                newName: "lastUploadDate");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "UniversalisQueries",
                newName: "itemId");

            migrationBuilder.RenameColumn(
                name: "HqSaleVelocity",
                table: "UniversalisQueries",
                newName: "hqSaleVelocity");

            migrationBuilder.RenameColumn(
                name: "CurrentAveragePrinceNQ",
                table: "UniversalisQueries",
                newName: "currentAveragePrinceNQ");

            migrationBuilder.RenameColumn(
                name: "CurrentAveragePriceHQ",
                table: "UniversalisQueries",
                newName: "currentAveragePriceHQ");

            migrationBuilder.RenameColumn(
                name: "CurrentAveragePrice",
                table: "UniversalisQueries",
                newName: "currentAveragePrice");

            migrationBuilder.RenameColumn(
                name: "AveragePriceNQ",
                table: "UniversalisQueries",
                newName: "averagePriceNQ");

            migrationBuilder.RenameColumn(
                name: "AveragePriceHQ",
                table: "UniversalisQueries",
                newName: "averagePriceHQ");

            migrationBuilder.RenameColumn(
                name: "AveragePrice",
                table: "UniversalisQueries",
                newName: "averagePrice");

            migrationBuilder.RenameIndex(
                name: "IX_UniversalisQueries_WorldId",
                table: "UniversalisQueries",
                newName: "IX_UniversalisQueries_worldId");

            migrationBuilder.RenameIndex(
                name: "IX_UniversalisQueries_ItemId",
                table: "UniversalisQueries",
                newName: "IX_UniversalisQueries_itemId");

            migrationBuilder.RenameColumn(
                name: "UniversalisEntryId",
                table: "SaleHistory",
                newName: "UniversalisQueryId");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "SaleHistory",
                newName: "WorldId");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "SaleHistory",
                newName: "ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_SaleHistory_UniversalisEntryId",
                table: "SaleHistory",
                newName: "IX_SaleHistory_UniversalisQueryId");

            migrationBuilder.RenameColumn(
                name: "UniversalisEntryId",
                table: "Posts",
                newName: "UniversalisQueryId");

            migrationBuilder.RenameColumn(
                name: "LastReviewDate",
                table: "Posts",
                newName: "QueryDate");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UniversalisEntryId",
                table: "Posts",
                newName: "IX_Posts_UniversalisQueryId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Posts",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PostedDate",
                table: "Posts",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "WorldId",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SaleHistory_ItemId",
                table: "SaleHistory",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleHistory_WorldId",
                table: "SaleHistory",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ItemId",
                table: "Posts",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_WorldId",
                table: "Posts",
                column: "WorldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Items_ItemId",
                table: "Posts",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_UniversalisQueries_UniversalisQueryId",
                table: "Posts",
                column: "UniversalisQueryId",
                principalTable: "UniversalisQueries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Worlds_WorldId",
                table: "Posts",
                column: "WorldId",
                principalTable: "Worlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleHistory_Items_ItemId",
                table: "SaleHistory",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleHistory_UniversalisQueries_UniversalisQueryId",
                table: "SaleHistory",
                column: "UniversalisQueryId",
                principalTable: "UniversalisQueries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleHistory_Worlds_WorldId",
                table: "SaleHistory",
                column: "WorldId",
                principalTable: "Worlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UniversalisQueries_Items_itemId",
                table: "UniversalisQueries",
                column: "itemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UniversalisQueries_Worlds_worldId",
                table: "UniversalisQueries",
                column: "worldId",
                principalTable: "Worlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
