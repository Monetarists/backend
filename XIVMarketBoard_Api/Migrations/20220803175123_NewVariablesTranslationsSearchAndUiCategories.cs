using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XIVMarketBoard_Api.Migrations
{
    public partial class NewVariablesTranslationsSearchAndUiCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Recipes",
                newName: "Name_ja");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Jobs",
                newName: "Name_ja");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Items",
                newName: "Name_ja");

            migrationBuilder.AddColumn<bool>(
                name: "IsExpert",
                table: "Recipes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSpecializationRequired",
                table: "Recipes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name_de",
                table: "Recipes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Name_en",
                table: "Recipes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Name_fr",
                table: "Recipes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Abbreviation",
                table: "Jobs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ClassJobCategoryTargetID",
                table: "Jobs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DohDolJobIndex",
                table: "Jobs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Name_de",
                table: "Jobs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Name_en",
                table: "Jobs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Name_fr",
                table: "Jobs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "CanBeHq",
                table: "Items",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ItemSearchCategoryId",
                table: "Items",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemUICategoryId",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name_de",
                table: "Items",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Name_en",
                table: "Items",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Name_fr",
                table: "Items",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemSearchCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name_en = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name_de = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name_fr = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name_ja = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSearchCategory", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemUICategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name_en = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name_de = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name_fr = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name_ja = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemUICategory", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemSearchCategoryId",
                table: "Items",
                column: "ItemSearchCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemUICategoryId",
                table: "Items",
                column: "ItemUICategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ItemSearchCategory_ItemSearchCategoryId",
                table: "Items",
                column: "ItemSearchCategoryId",
                principalTable: "ItemSearchCategory",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ItemUICategory_ItemUICategoryId",
                table: "Items",
                column: "ItemUICategoryId",
                principalTable: "ItemUICategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_ItemSearchCategory_ItemSearchCategoryId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_ItemUICategory_ItemUICategoryId",
                table: "Items");

            migrationBuilder.DropTable(
                name: "ItemSearchCategory");

            migrationBuilder.DropTable(
                name: "ItemUICategory");

            migrationBuilder.DropIndex(
                name: "IX_Items_ItemSearchCategoryId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_ItemUICategoryId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsExpert",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "IsSpecializationRequired",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Name_de",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Name_en",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Name_fr",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Abbreviation",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ClassJobCategoryTargetID",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "DohDolJobIndex",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Name_de",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Name_en",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Name_fr",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CanBeHq",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemSearchCategoryId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemUICategoryId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Name_de",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Name_en",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Name_fr",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "Name_ja",
                table: "Recipes",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Name_ja",
                table: "Jobs",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Name_ja",
                table: "Items",
                newName: "Name");
        }
    }
}
