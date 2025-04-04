using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sp_back.Migrations
{
    /// <inheritdoc />
    public partial class onetoone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_RequestMaintenances_RequestMaintenanceId",
                table: "Maintenances");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_RequestMaintenanceId",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "RequestMaintenanceId",
                table: "Maintenances");

            migrationBuilder.AddColumn<int>(
                name: "MaintenanceId",
                table: "RequestMaintenances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RequestMaintenances_MaintenanceId",
                table: "RequestMaintenances",
                column: "MaintenanceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestMaintenances_Maintenances_MaintenanceId",
                table: "RequestMaintenances",
                column: "MaintenanceId",
                principalTable: "Maintenances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestMaintenances_Maintenances_MaintenanceId",
                table: "RequestMaintenances");

            migrationBuilder.DropIndex(
                name: "IX_RequestMaintenances_MaintenanceId",
                table: "RequestMaintenances");

            migrationBuilder.DropColumn(
                name: "MaintenanceId",
                table: "RequestMaintenances");

            migrationBuilder.AddColumn<int>(
                name: "RequestMaintenanceId",
                table: "Maintenances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_RequestMaintenanceId",
                table: "Maintenances",
                column: "RequestMaintenanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_RequestMaintenances_RequestMaintenanceId",
                table: "Maintenances",
                column: "RequestMaintenanceId",
                principalTable: "RequestMaintenances",
                principalColumn: "Id");
        }
    }
}
