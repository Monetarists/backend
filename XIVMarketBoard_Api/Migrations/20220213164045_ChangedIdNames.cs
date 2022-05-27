using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XIVMarketBoard_Api.Migrations
{
    public partial class ChangedIdNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SaleHistoryId",
                table: "SaleHistory",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "RetainerId",
                table: "Retainers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "RecipeId",
                table: "Recipes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PostId",
                table: "MbPosts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "Items",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SaleHistory",
                newName: "SaleHistoryId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Retainers",
                newName: "RetainerId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Recipes",
                newName: "RecipeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "MbPosts",
                newName: "PostId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Items",
                newName: "ItemId");
        }
    }
}
