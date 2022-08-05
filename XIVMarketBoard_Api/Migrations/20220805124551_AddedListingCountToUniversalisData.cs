using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XIVMarketBoard_Api.Migrations
{
    public partial class AddedListingCountToUniversalisData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CraftingCost",
                table: "UniversalisEntries",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HqListingsCount",
                table: "UniversalisEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NqListingsCount",
                table: "UniversalisEntries",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CraftingCost",
                table: "UniversalisEntries");

            migrationBuilder.DropColumn(
                name: "HqListingsCount",
                table: "UniversalisEntries");

            migrationBuilder.DropColumn(
                name: "NqListingsCount",
                table: "UniversalisEntries");
        }
    }
}
