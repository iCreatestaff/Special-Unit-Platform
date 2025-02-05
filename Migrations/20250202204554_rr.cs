using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sp_back.Migrations
{
    /// <inheritdoc />
    public partial class rr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MissionAccounts_Accounts_AssignedAccountsId",
                table: "MissionAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_MissionAccounts_Missions_MissionsId",
                table: "MissionAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_MissionEquipment_Equipments_AssignedEquipmentId",
                table: "MissionEquipment");

            migrationBuilder.DropForeignKey(
                name: "FK_MissionEquipment_Missions_MissionsId",
                table: "MissionEquipment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MissionEquipment",
                table: "MissionEquipment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MissionAccounts",
                table: "MissionAccounts");

            migrationBuilder.RenameTable(
                name: "MissionEquipment",
                newName: "EquipmentMission");

            migrationBuilder.RenameTable(
                name: "MissionAccounts",
                newName: "AccountMission");

            migrationBuilder.RenameIndex(
                name: "IX_MissionEquipment_MissionsId",
                table: "EquipmentMission",
                newName: "IX_EquipmentMission_MissionsId");

            migrationBuilder.RenameIndex(
                name: "IX_MissionAccounts_MissionsId",
                table: "AccountMission",
                newName: "IX_AccountMission_MissionsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipmentMission",
                table: "EquipmentMission",
                columns: new[] { "AssignedEquipmentId", "MissionsId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountMission",
                table: "AccountMission",
                columns: new[] { "AssignedAccountsId", "MissionsId" });

            migrationBuilder.CreateTable(
                name: "AccountMissions",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    MissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountMissions", x => new { x.AccountId, x.MissionId });
                    table.ForeignKey(
                        name: "FK_AccountMissions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountMissions_Missions_MissionId",
                        column: x => x.MissionId,
                        principalTable: "Missions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentMissions",
                columns: table => new
                {
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    MissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentMissions", x => new { x.EquipmentId, x.MissionId });
                    table.ForeignKey(
                        name: "FK_EquipmentMissions_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentMissions_Missions_MissionId",
                        column: x => x.MissionId,
                        principalTable: "Missions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountMissions_MissionId",
                table: "AccountMissions",
                column: "MissionId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMissions_MissionId",
                table: "EquipmentMissions",
                column: "MissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMission_Accounts_AssignedAccountsId",
                table: "AccountMission",
                column: "AssignedAccountsId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMission_Missions_MissionsId",
                table: "AccountMission",
                column: "MissionsId",
                principalTable: "Missions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentMission_Equipments_AssignedEquipmentId",
                table: "EquipmentMission",
                column: "AssignedEquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentMission_Missions_MissionsId",
                table: "EquipmentMission",
                column: "MissionsId",
                principalTable: "Missions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountMission_Accounts_AssignedAccountsId",
                table: "AccountMission");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountMission_Missions_MissionsId",
                table: "AccountMission");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentMission_Equipments_AssignedEquipmentId",
                table: "EquipmentMission");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentMission_Missions_MissionsId",
                table: "EquipmentMission");

            migrationBuilder.DropTable(
                name: "AccountMissions");

            migrationBuilder.DropTable(
                name: "EquipmentMissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipmentMission",
                table: "EquipmentMission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountMission",
                table: "AccountMission");

            migrationBuilder.RenameTable(
                name: "EquipmentMission",
                newName: "MissionEquipment");

            migrationBuilder.RenameTable(
                name: "AccountMission",
                newName: "MissionAccounts");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentMission_MissionsId",
                table: "MissionEquipment",
                newName: "IX_MissionEquipment_MissionsId");

            migrationBuilder.RenameIndex(
                name: "IX_AccountMission_MissionsId",
                table: "MissionAccounts",
                newName: "IX_MissionAccounts_MissionsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MissionEquipment",
                table: "MissionEquipment",
                columns: new[] { "AssignedEquipmentId", "MissionsId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_MissionAccounts",
                table: "MissionAccounts",
                columns: new[] { "AssignedAccountsId", "MissionsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MissionAccounts_Accounts_AssignedAccountsId",
                table: "MissionAccounts",
                column: "AssignedAccountsId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MissionAccounts_Missions_MissionsId",
                table: "MissionAccounts",
                column: "MissionsId",
                principalTable: "Missions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MissionEquipment_Equipments_AssignedEquipmentId",
                table: "MissionEquipment",
                column: "AssignedEquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MissionEquipment_Missions_MissionsId",
                table: "MissionEquipment",
                column: "MissionsId",
                principalTable: "Missions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
