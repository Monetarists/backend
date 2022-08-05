using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XIVMarketBoard_Api.Migrations
{
    public partial class ChangeJobToUpper : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Jobs_jobId",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "jobId",
                table: "Recipes",
                newName: "JobId");

            migrationBuilder.RenameIndex(
                name: "IX_Recipes_jobId",
                table: "Recipes",
                newName: "IX_Recipes_JobId");

            migrationBuilder.AlterColumn<int>(
                name: "ClassJobCategoryTargetID",
                table: "Jobs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Jobs_JobId",
                table: "Recipes",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Jobs_JobId",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "JobId",
                table: "Recipes",
                newName: "jobId");

            migrationBuilder.RenameIndex(
                name: "IX_Recipes_JobId",
                table: "Recipes",
                newName: "IX_Recipes_jobId");

            migrationBuilder.AlterColumn<int>(
                name: "ClassJobCategoryTargetID",
                table: "Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
