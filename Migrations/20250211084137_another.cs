using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sp_back.Migrations
{
    /// <inheritdoc />
    public partial class another : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountMission");

            migrationBuilder.AddColumn<int>(
                name: "MissionId",
                table: "Accounts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_MissionId",
                table: "Accounts",
                column: "MissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Missions_MissionId",
                table: "Accounts",
                column: "MissionId",
                principalTable: "Missions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Missions_MissionId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_MissionId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "MissionId",
                table: "Accounts");

            migrationBuilder.CreateTable(
                name: "AccountMission",
                columns: table => new
                {
                    AssignedAccountsId = table.Column<int>(type: "int", nullable: false),
                    MissionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountMission", x => new { x.AssignedAccountsId, x.MissionsId });
                    table.ForeignKey(
                        name: "FK_AccountMission_Accounts_AssignedAccountsId",
                        column: x => x.AssignedAccountsId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountMission_Missions_MissionsId",
                        column: x => x.MissionsId,
                        principalTable: "Missions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountMission_MissionsId",
                table: "AccountMission",
                column: "MissionsId");
        }
    }
}
