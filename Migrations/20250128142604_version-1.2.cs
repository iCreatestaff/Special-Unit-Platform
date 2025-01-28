using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sp_back.Migrations
{
    /// <inheritdoc />
    public partial class version12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubEquipmentIds",
                table: "Equipments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubEquipmentIds",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
