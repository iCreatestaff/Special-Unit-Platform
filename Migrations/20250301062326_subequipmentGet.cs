using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sp_back.Migrations
{
    /// <inheritdoc />
    public partial class subequipmentGet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_SubEquipments_SubEquipmentId",
                table: "Maintenances");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_SubEquipments_SubEquipmentId",
                table: "Maintenances",
                column: "SubEquipmentId",
                principalTable: "SubEquipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_SubEquipments_SubEquipmentId",
                table: "Maintenances");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_SubEquipments_SubEquipmentId",
                table: "Maintenances",
                column: "SubEquipmentId",
                principalTable: "SubEquipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
