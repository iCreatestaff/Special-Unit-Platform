using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sp_back.Migrations
{
    /// <inheritdoc />
    public partial class trainingIdattribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TrainingID",
                table: "Nonavailabilities",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrainingID",
                table: "Nonavailabilities");
        }
    }
}
