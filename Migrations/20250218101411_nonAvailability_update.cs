using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sp_back.Migrations
{
    /// <inheritdoc />
    public partial class nonAvailability_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "Nonavailabilities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SubEquipmentId",
                table: "Nonavailabilities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Nonavailabilities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Nonavailabilities_SubEquipmentId",
                table: "Nonavailabilities",
                column: "SubEquipmentId");

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
                name: "FK_Nonavailabilities_SubEquipments_SubEquipmentId",
                table: "Nonavailabilities");

            migrationBuilder.DropIndex(
                name: "IX_Nonavailabilities_SubEquipmentId",
                table: "Nonavailabilities");

            migrationBuilder.DropColumn(
                name: "SubEquipmentId",
                table: "Nonavailabilities");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Nonavailabilities");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "Nonavailabilities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
