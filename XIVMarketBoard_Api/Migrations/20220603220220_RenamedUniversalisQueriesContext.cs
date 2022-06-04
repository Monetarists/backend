using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XIVMarketBoard_Api.Migrations
{
    public partial class RenamedUniversalisQueriesContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_UniversalisQueries",
                table: "UniversalisQueries");

            migrationBuilder.RenameTable(
                name: "UniversalisQueries",
                newName: "UniversalisEntries");

            migrationBuilder.RenameIndex(
                name: "IX_UniversalisQueries_WorldId",
                table: "UniversalisEntries",
                newName: "IX_UniversalisEntries_WorldId");

            migrationBuilder.RenameIndex(
                name: "IX_UniversalisQueries_ItemId",
                table: "UniversalisEntries",
                newName: "IX_UniversalisEntries_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UniversalisEntries",
                table: "UniversalisEntries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_UniversalisEntries_UniversalisEntryId",
                table: "Posts",
                column: "UniversalisEntryId",
                principalTable: "UniversalisEntries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleHistory_UniversalisEntries_UniversalisEntryId",
                table: "SaleHistory",
                column: "UniversalisEntryId",
                principalTable: "UniversalisEntries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UniversalisEntries_Items_ItemId",
                table: "UniversalisEntries",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UniversalisEntries_Worlds_WorldId",
                table: "UniversalisEntries",
                column: "WorldId",
                principalTable: "Worlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_UniversalisEntries_UniversalisEntryId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleHistory_UniversalisEntries_UniversalisEntryId",
                table: "SaleHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversalisEntries_Items_ItemId",
                table: "UniversalisEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversalisEntries_Worlds_WorldId",
                table: "UniversalisEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UniversalisEntries",
                table: "UniversalisEntries");

            migrationBuilder.RenameTable(
                name: "UniversalisEntries",
                newName: "UniversalisQueries");

            migrationBuilder.RenameIndex(
                name: "IX_UniversalisEntries_WorldId",
                table: "UniversalisQueries",
                newName: "IX_UniversalisQueries_WorldId");

            migrationBuilder.RenameIndex(
                name: "IX_UniversalisEntries_ItemId",
                table: "UniversalisQueries",
                newName: "IX_UniversalisQueries_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UniversalisQueries",
                table: "UniversalisQueries",
                column: "Id");

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
    }
}
