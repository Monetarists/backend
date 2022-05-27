using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XIVMarketBoard_Api.Migrations
{
    public partial class ChangedNamesOfEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WorldName",
                table: "Servers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "RetainerName",
                table: "Retainers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "JobName",
                table: "Jobs",
                newName: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Servers",
                newName: "WorldName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Retainers",
                newName: "RetainerName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Jobs",
                newName: "JobName");
        }
    }
}
