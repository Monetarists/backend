using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XIVMarketBoard_Api.Migrations
{
    public partial class ChangedClassJobCategoryTargetIDToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ClassJobCategoryTargetID",
                table: "Jobs",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ClassJobCategoryTargetID",
                table: "Jobs",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
