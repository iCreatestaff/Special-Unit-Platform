using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sp_back.Migrations
{
    /// <inheritdoc />
    public partial class MissionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Missions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missions", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "EquipmentMission",
                columns: table => new
                {
                    AssignedEquipmentId = table.Column<int>(type: "int", nullable: false),
                    MissionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentMission", x => new { x.AssignedEquipmentId, x.MissionsId });
                    table.ForeignKey(
                        name: "FK_EquipmentMission_Equipments_AssignedEquipmentId",
                        column: x => x.AssignedEquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentMission_Missions_MissionsId",
                        column: x => x.MissionsId,
                        principalTable: "Missions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountMission_MissionsId",
                table: "AccountMission",
                column: "MissionsId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMission_MissionsId",
                table: "EquipmentMission",
                column: "MissionsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountMission");

            migrationBuilder.DropTable(
                name: "EquipmentMission");

            migrationBuilder.DropTable(
                name: "Missions");
        }
    }
}
