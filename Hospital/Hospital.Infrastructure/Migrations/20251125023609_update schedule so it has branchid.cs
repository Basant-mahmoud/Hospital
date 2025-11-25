using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateschedulesoithasbranchid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_BranchId",
                table: "Schedules",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Branches_BranchId",
                table: "Schedules",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Branches_BranchId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_BranchId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Schedules");
        }
    }
}
