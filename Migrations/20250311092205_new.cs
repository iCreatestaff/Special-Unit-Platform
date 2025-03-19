using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sp_back.Migrations
{
    /// <inheritdoc />
    public partial class @new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageAgents_Accounts_AccountId",
                table: "MessageAgents");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageAgents_Accounts_AccountId1",
                table: "MessageAgents");

            migrationBuilder.DropIndex(
                name: "IX_MessageAgents_AccountId",
                table: "MessageAgents");

            migrationBuilder.DropIndex(
                name: "IX_MessageAgents_AccountId1",
                table: "MessageAgents");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "MessageAgents");

            migrationBuilder.DropColumn(
                name: "AccountId1",
                table: "MessageAgents");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "MessageAgents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "MessageAgents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountId1",
                table: "MessageAgents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "MessageAgents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MessageAgents_AccountId",
                table: "MessageAgents",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageAgents_AccountId1",
                table: "MessageAgents",
                column: "AccountId1");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageAgents_Accounts_AccountId",
                table: "MessageAgents",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageAgents_Accounts_AccountId1",
                table: "MessageAgents",
                column: "AccountId1",
                principalTable: "Accounts",
                principalColumn: "Id");
        }
    }
}
