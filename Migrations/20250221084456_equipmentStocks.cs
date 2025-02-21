using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sp_back.Migrations
{
    /// <inheritdoc />
    public partial class equipmentStocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_EquipmentStock_EquipmentStockId",
                table: "Equipments");

            migrationBuilder.DropTable(
                name: "EquipmentStock");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_EquipmentStockId",
                table: "Equipments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentStock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentStock", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_EquipmentStockId",
                table: "Equipments",
                column: "EquipmentStockId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_EquipmentStock_EquipmentStockId",
                table: "Equipments",
                column: "EquipmentStockId",
                principalTable: "EquipmentStock",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
