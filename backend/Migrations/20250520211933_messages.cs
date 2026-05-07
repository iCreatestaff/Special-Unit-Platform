using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sp_back.Migrations
{
    /// <inheritdoc />
    public partial class messages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nonavailabilities_SubEquipments_SubEquipmentId",
                table: "Nonavailabilities");

            migrationBuilder.RenameColumn(
                name: "SubEquipmentId",
                table: "Nonavailabilities",
                newName: "SubequipmentID");

            migrationBuilder.RenameIndex(
                name: "IX_Nonavailabilities_SubEquipmentId",
                table: "Nonavailabilities",
                newName: "IX_Nonavailabilities_SubequipmentID");

            migrationBuilder.AddColumn<string>(
                name: "SenderName",
                table: "MessageAgents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Nonavailabilities_SubEquipments_SubequipmentID",
                table: "Nonavailabilities",
                column: "SubequipmentID",
                principalTable: "SubEquipments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nonavailabilities_SubEquipments_SubequipmentID",
                table: "Nonavailabilities");

            migrationBuilder.DropColumn(
                name: "SenderName",
                table: "MessageAgents");

            migrationBuilder.RenameColumn(
                name: "SubequipmentID",
                table: "Nonavailabilities",
                newName: "SubEquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Nonavailabilities_SubequipmentID",
                table: "Nonavailabilities",
                newName: "IX_Nonavailabilities_SubEquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nonavailabilities_SubEquipments_SubEquipmentId",
                table: "Nonavailabilities",
                column: "SubEquipmentId",
                principalTable: "SubEquipments",
                principalColumn: "Id");
        }
    }
}
