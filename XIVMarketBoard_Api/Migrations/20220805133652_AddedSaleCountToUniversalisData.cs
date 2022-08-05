using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XIVMarketBoard_Api.Migrations
{
    public partial class AddedSaleCountToUniversalisData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CraftingCost",
                table: "UniversalisEntries");

            migrationBuilder.AddColumn<int>(
                name: "HqSaleCount",
                table: "UniversalisEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NqSaleCount",
                table: "UniversalisEntries",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HqSaleCount",
                table: "UniversalisEntries");

            migrationBuilder.DropColumn(
                name: "NqSaleCount",
                table: "UniversalisEntries");

            migrationBuilder.AddColumn<double>(
                name: "CraftingCost",
                table: "UniversalisEntries",
                type: "double",
                nullable: true);
        }
    }
}
