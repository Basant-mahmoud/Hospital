using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateinbannerandserviceandbranchandspecilizationrelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banners_Branches_BranchId",
                table: "Banners");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Branches_BranchId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_News_Branches_BranchId",
                table: "News");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Branches_BranchId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Specializations_SpecializationId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_BranchId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_SpecializationId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Banners_BranchId",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "SpecializationId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Banners");

            migrationBuilder.CreateTable(
                name: "BranchServices",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchServices", x => new { x.BranchId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_BranchServices_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BranchServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchSpecializations",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    SpecializationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchSpecializations", x => new { x.BranchId, x.SpecializationId });
                    table.ForeignKey(
                        name: "FK_BranchSpecializations_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BranchSpecializations_Specializations_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Specializations",
                        principalColumn: "SpecializationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BranchServices_ServiceId",
                table: "BranchServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchSpecializations_SpecializationId",
                table: "BranchSpecializations",
                column: "SpecializationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Branches_BranchId",
                table: "Events",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Branches_BranchId",
                table: "News",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Branches_BranchId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_News_Branches_BranchId",
                table: "News");

            migrationBuilder.DropTable(
                name: "BranchServices");

            migrationBuilder.DropTable(
                name: "BranchSpecializations");

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpecializationId",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Banners",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_BranchId",
                table: "Services",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_SpecializationId",
                table: "Services",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_BranchId",
                table: "Banners",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Banners_Branches_BranchId",
                table: "Banners",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Branches_BranchId",
                table: "Events",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_News_Branches_BranchId",
                table: "News",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Branches_BranchId",
                table: "Services",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Specializations_SpecializationId",
                table: "Services",
                column: "SpecializationId",
                principalTable: "Specializations",
                principalColumn: "SpecializationId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
