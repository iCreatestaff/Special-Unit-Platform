using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sp_back.Migrations
{
    /// <inheritdoc />
    public partial class Equipment_nonAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nonavailabilities_SubEquipments_SubEquipmentId",
                table: "Nonavailabilities");

            migrationBuilder.AddColumn<int>(
                name: "EquipmentId",
                table: "Nonavailabilities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nonavailabilities_EquipmentId",
                table: "Nonavailabilities",
                column: "EquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nonavailabilities_Equipments_EquipmentId",
                table: "Nonavailabilities",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Nonavailabilities_SubEquipments_SubEquipmentId",
                table: "Nonavailabilities",
                column: "SubEquipmentId",
                principalTable: "SubEquipments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nonavailabilities_Equipments_EquipmentId",
                table: "Nonavailabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Nonavailabilities_SubEquipments_SubEquipmentId",
                table: "Nonavailabilities");

            migrationBuilder.DropIndex(
                name: "IX_Nonavailabilities_EquipmentId",
                table: "Nonavailabilities");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                table: "Nonavailabilities");

            migrationBuilder.AddForeignKey(
                name: "FK_Nonavailabilities_SubEquipments_SubEquipmentId",
                table: "Nonavailabilities",
                column: "SubEquipmentId",
                principalTable: "SubEquipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
