using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sp_back.Migrations
{
    /// <inheritdoc />
    public partial class createMission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Missions_MissionId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Missions_MissionId",
                table: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_MissionId",
                table: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_MissionId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "MissionId",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "MissionId",
                table: "Accounts");

            migrationBuilder.AddColumn<string>(
                name: "AssignedAccounts",
                table: "Missions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "AssignedEquipment",
                table: "Missions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedAccounts",
                table: "Missions");

            migrationBuilder.DropColumn(
                name: "AssignedEquipment",
                table: "Missions");

            migrationBuilder.AddColumn<int>(
                name: "MissionId",
                table: "Equipments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MissionId",
                table: "Accounts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_MissionId",
                table: "Equipments",
                column: "MissionId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Missions_MissionId",
                table: "Equipments",
                column: "MissionId",
                principalTable: "Missions",
                principalColumn: "Id");
        }
    }
}
