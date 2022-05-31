using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XIVMarketBoard_Api.Migrations
{
    public partial class renamedPostsAndServers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MbPosts_Items_ItemId",
                table: "MbPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_MbPosts_Retainers_RetainerId",
                table: "MbPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_MbPosts_Servers_WorldId",
                table: "MbPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_MbPosts_UniversalisQueries_UniversalisQueryId",
                table: "MbPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_MbPosts_Users_UserName",
                table: "MbPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Retainers_Servers_WorldId",
                table: "Retainers");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleHistory_Servers_WorldId",
                table: "SaleHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Servers_DataCenters_DataCenterId",
                table: "Servers");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversalisQueries_Servers_worldId",
                table: "UniversalisQueries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Servers",
                table: "Servers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MbPosts",
                table: "MbPosts");

            migrationBuilder.RenameTable(
                name: "Servers",
                newName: "Worlds");

            migrationBuilder.RenameTable(
                name: "MbPosts",
                newName: "Posts");

            migrationBuilder.RenameIndex(
                name: "IX_Servers_DataCenterId",
                table: "Worlds",
                newName: "IX_Worlds_DataCenterId");

            migrationBuilder.RenameIndex(
                name: "IX_MbPosts_WorldId",
                table: "Posts",
                newName: "IX_Posts_WorldId");

            migrationBuilder.RenameIndex(
                name: "IX_MbPosts_UserName",
                table: "Posts",
                newName: "IX_Posts_UserName");

            migrationBuilder.RenameIndex(
                name: "IX_MbPosts_UniversalisQueryId",
                table: "Posts",
                newName: "IX_Posts_UniversalisQueryId");

            migrationBuilder.RenameIndex(
                name: "IX_MbPosts_RetainerId",
                table: "Posts",
                newName: "IX_Posts_RetainerId");

            migrationBuilder.RenameIndex(
                name: "IX_MbPosts_ItemId",
                table: "Posts",
                newName: "IX_Posts_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Worlds",
                table: "Worlds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                table: "Posts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Items_ItemId",
                table: "Posts",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Retainers_RetainerId",
                table: "Posts",
                column: "RetainerId",
                principalTable: "Retainers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_UniversalisQueries_UniversalisQueryId",
                table: "Posts",
                column: "UniversalisQueryId",
                principalTable: "UniversalisQueries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_UserName",
                table: "Posts",
                column: "UserName",
                principalTable: "Users",
                principalColumn: "UserName");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Worlds_WorldId",
                table: "Posts",
                column: "WorldId",
                principalTable: "Worlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Retainers_Worlds_WorldId",
                table: "Retainers",
                column: "WorldId",
                principalTable: "Worlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleHistory_Worlds_WorldId",
                table: "SaleHistory",
                column: "WorldId",
                principalTable: "Worlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UniversalisQueries_Worlds_worldId",
                table: "UniversalisQueries",
                column: "worldId",
                principalTable: "Worlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Worlds_DataCenters_DataCenterId",
                table: "Worlds",
                column: "DataCenterId",
                principalTable: "DataCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Items_ItemId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Retainers_RetainerId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_UniversalisQueries_UniversalisQueryId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_UserName",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Worlds_WorldId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Retainers_Worlds_WorldId",
                table: "Retainers");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleHistory_Worlds_WorldId",
                table: "SaleHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversalisQueries_Worlds_worldId",
                table: "UniversalisQueries");

            migrationBuilder.DropForeignKey(
                name: "FK_Worlds_DataCenters_DataCenterId",
                table: "Worlds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Worlds",
                table: "Worlds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                table: "Posts");

            migrationBuilder.RenameTable(
                name: "Worlds",
                newName: "Servers");

            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "MbPosts");

            migrationBuilder.RenameIndex(
                name: "IX_Worlds_DataCenterId",
                table: "Servers",
                newName: "IX_Servers_DataCenterId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_WorldId",
                table: "MbPosts",
                newName: "IX_MbPosts_WorldId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UserName",
                table: "MbPosts",
                newName: "IX_MbPosts_UserName");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UniversalisQueryId",
                table: "MbPosts",
                newName: "IX_MbPosts_UniversalisQueryId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_RetainerId",
                table: "MbPosts",
                newName: "IX_MbPosts_RetainerId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_ItemId",
                table: "MbPosts",
                newName: "IX_MbPosts_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Servers",
                table: "Servers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MbPosts",
                table: "MbPosts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MbPosts_Items_ItemId",
                table: "MbPosts",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MbPosts_Retainers_RetainerId",
                table: "MbPosts",
                column: "RetainerId",
                principalTable: "Retainers",
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
                name: "FK_MbPosts_Users_UserName",
                table: "MbPosts",
                column: "UserName",
                principalTable: "Users",
                principalColumn: "UserName");

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
                name: "FK_Servers_DataCenters_DataCenterId",
                table: "Servers",
                column: "DataCenterId",
                principalTable: "DataCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UniversalisQueries_Servers_worldId",
                table: "UniversalisQueries",
                column: "worldId",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
