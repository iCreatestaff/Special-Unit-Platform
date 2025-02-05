using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sp_back.Migrations
{
    /// <inheritdoc />
    public partial class equipments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentMission");

            migrationBuilder.AddColumn<int>(
                name: "MissionId",
                table: "Equipments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_MissionId",
                table: "Equipments",
                column: "MissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Missions_MissionId",
                table: "Equipments",
                column: "MissionId",
                principalTable: "Missions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Missions_MissionId",
                table: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_MissionId",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "MissionId",
                table: "Equipments");

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
                name: "IX_EquipmentMission_MissionsId",
                table: "EquipmentMission",
                column: "MissionsId");
        }
    }
}
