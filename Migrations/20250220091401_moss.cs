using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sp_back.Migrations
{
    /// <inheritdoc />
    public partial class moss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Nonavailabilities_MissionID",
                table: "Nonavailabilities",
                column: "MissionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Nonavailabilities_Missions_MissionID",
                table: "Nonavailabilities",
                column: "MissionID",
                principalTable: "Missions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nonavailabilities_Missions_MissionID",
                table: "Nonavailabilities");

            migrationBuilder.DropIndex(
                name: "IX_Nonavailabilities_MissionID",
                table: "Nonavailabilities");
        }
    }
}
