using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace XIVMarketBoard_Api.Migrations
{
    public partial class InitialCreateSupaBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataCenters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Region = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataCenters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemSearchCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name_en = table.Column<string>(type: "text", nullable: false),
                    Name_de = table.Column<string>(type: "text", nullable: false),
                    Name_fr = table.Column<string>(type: "text", nullable: false),
                    Name_ja = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSearchCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemUICategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name_en = table.Column<string>(type: "text", nullable: false),
                    Name_de = table.Column<string>(type: "text", nullable: false),
                    Name_fr = table.Column<string>(type: "text", nullable: false),
                    Name_ja = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemUICategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    CharacterName = table.Column<string>(type: "text", nullable: true),
                    ApiAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    WebAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Worlds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DataCenterId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worlds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Worlds_DataCenters_DataCenterId",
                        column: x => x.DataCenterId,
                        principalTable: "DataCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name_en = table.Column<string>(type: "text", nullable: false),
                    Name_de = table.Column<string>(type: "text", nullable: false),
                    Name_fr = table.Column<string>(type: "text", nullable: false),
                    Name_ja = table.Column<string>(type: "text", nullable: false),
                    ItemSearchCategoryId = table.Column<int>(type: "integer", nullable: true),
                    ItemUICategoryId = table.Column<int>(type: "integer", nullable: false),
                    CanBeCrafted = table.Column<bool>(type: "boolean", nullable: true),
                    CanBeHq = table.Column<bool>(type: "boolean", nullable: false),
                    IsMarketable = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_ItemSearchCategory_ItemSearchCategoryId",
                        column: x => x.ItemSearchCategoryId,
                        principalTable: "ItemSearchCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Items_ItemUICategory_ItemUICategoryId",
                        column: x => x.ItemUICategoryId,
                        principalTable: "ItemUICategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name_en = table.Column<string>(type: "text", nullable: false),
                    Name_fr = table.Column<string>(type: "text", nullable: false),
                    Name_ja = table.Column<string>(type: "text", nullable: false),
                    Name_de = table.Column<string>(type: "text", nullable: false),
                    Abbreviation = table.Column<string>(type: "text", nullable: false),
                    ClassJobCategoryTargetID = table.Column<int>(type: "integer", nullable: true),
                    DohDolJobIndex = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Retainers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    WorldId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Retainers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Retainers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Retainers_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UniversalisEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    WorldId = table.Column<int>(type: "integer", nullable: false),
                    LastUploadDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    QueryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CurrentAveragePrice = table.Column<double>(type: "double precision", nullable: false),
                    CurrentAveragePrinceNQ = table.Column<double>(type: "double precision", nullable: false),
                    CurrentAveragePriceHQ = table.Column<double>(type: "double precision", nullable: false),
                    RegularSaleVelocity = table.Column<double>(type: "double precision", nullable: false),
                    NqSaleVelocity = table.Column<double>(type: "double precision", nullable: false),
                    HqSaleVelocity = table.Column<double>(type: "double precision", nullable: false),
                    AveragePrice = table.Column<double>(type: "double precision", nullable: false),
                    AveragePriceNQ = table.Column<double>(type: "double precision", nullable: false),
                    AveragePriceHQ = table.Column<double>(type: "double precision", nullable: false),
                    MinPrice = table.Column<double>(type: "double precision", nullable: false),
                    MinPriceNQ = table.Column<double>(type: "double precision", nullable: false),
                    MinPriceHQ = table.Column<double>(type: "double precision", nullable: false),
                    MaxPrice = table.Column<double>(type: "double precision", nullable: false),
                    MaxPriceNQ = table.Column<double>(type: "double precision", nullable: false),
                    MaxPriceHQ = table.Column<double>(type: "double precision", nullable: false),
                    NqListingsCount = table.Column<int>(type: "integer", nullable: true),
                    HqListingsCount = table.Column<int>(type: "integer", nullable: true),
                    NqSaleCount = table.Column<int>(type: "integer", nullable: true),
                    HqSaleCount = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniversalisEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UniversalisEntries_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniversalisEntries_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name_en = table.Column<string>(type: "text", nullable: false),
                    Name_de = table.Column<string>(type: "text", nullable: false),
                    Name_fr = table.Column<string>(type: "text", nullable: false),
                    Name_ja = table.Column<string>(type: "text", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    AmountResult = table.Column<int>(type: "integer", nullable: false),
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    IsExpert = table.Column<bool>(type: "boolean", nullable: false),
                    IsSpecializationRequired = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recipes_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Recipes_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    RetainerId = table.Column<int>(type: "integer", nullable: true),
                    RetainerName = table.Column<string>(type: "text", nullable: false),
                    SellerId = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    TotalAmount = table.Column<double>(type: "double precision", nullable: false),
                    HighQuality = table.Column<bool>(type: "boolean", nullable: false),
                    LastReviewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UniversalisEntryId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Retainers_RetainerId",
                        column: x => x.RetainerId,
                        principalTable: "Retainers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_UniversalisEntries_UniversalisEntryId",
                        column: x => x.UniversalisEntryId,
                        principalTable: "UniversalisEntries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SaleHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HighQuality = table.Column<bool>(type: "boolean", nullable: false),
                    SaleDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    BuyerName = table.Column<string>(type: "text", nullable: true),
                    Total = table.Column<double>(type: "double precision", nullable: false),
                    UniversalisEntryId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleHistory_UniversalisEntries_UniversalisEntryId",
                        column: x => x.UniversalisEntryId,
                        principalTable: "UniversalisEntries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    RecipeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingredients_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ingredients_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_ItemId",
                table: "Ingredients",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_RecipeId",
                table: "Ingredients",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemSearchCategoryId",
                table: "Items",
                column: "ItemSearchCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemUICategoryId",
                table: "Items",
                column: "ItemUICategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_UserId",
                table: "Jobs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_RetainerId",
                table: "Posts",
                column: "RetainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UniversalisEntryId",
                table: "Posts",
                column: "UniversalisEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_ItemId",
                table: "Recipes",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_JobId",
                table: "Recipes",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Retainers_UserId",
                table: "Retainers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Retainers_WorldId",
                table: "Retainers",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleHistory_UniversalisEntryId",
                table: "SaleHistory",
                column: "UniversalisEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalisEntries_ItemId",
                table: "UniversalisEntries",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalisEntries_WorldId",
                table: "UniversalisEntries",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_DataCenterId",
                table: "Worlds",
                column: "DataCenterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "SaleHistory");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "Retainers");

            migrationBuilder.DropTable(
                name: "UniversalisEntries");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Worlds");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ItemSearchCategory");

            migrationBuilder.DropTable(
                name: "ItemUICategory");

            migrationBuilder.DropTable(
                name: "DataCenters");
        }
    }
}
