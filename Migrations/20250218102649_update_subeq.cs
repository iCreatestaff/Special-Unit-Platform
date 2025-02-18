using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sp_back.Migrations
{
    /// <inheritdoc />
    public partial class update_subeq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nonavailabilities_SubEquipments_SubEquipmentId",
                table: "Nonavailabilities");

            migrationBuilder.AddForeignKey(
                name: "FK_Nonavailabilities_SubEquipments_SubEquipmentId",
                table: "Nonavailabilities",
                column: "SubEquipmentId",
                principalTable: "SubEquipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nonavailabilities_SubEquipments_SubEquipmentId",
                table: "Nonavailabilities");

            migrationBuilder.AddForeignKey(
                name: "FK_Nonavailabilities_SubEquipments_SubEquipmentId",
                table: "Nonavailabilities",
                column: "SubEquipmentId",
                principalTable: "SubEquipments",
                principalColumn: "Id");
        }
    }
}
