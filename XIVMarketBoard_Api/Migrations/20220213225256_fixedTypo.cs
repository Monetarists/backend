using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XIVMarketBoard_Api.Migrations
{
    public partial class fixedTypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Recipes_RecipeId",
                table: "Ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Items_ItemId",
                table: "Recipes");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Jobs_jobId",
                table: "Recipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recipies",
                table: "Recipies",
                column: "Id");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recipies",
                table: "Recipies");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Recipes_RecipeId",
                table: "Ingredients",
                column: "RecipeId",
                principalTable: "Recipes",
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
        }
    }
}
