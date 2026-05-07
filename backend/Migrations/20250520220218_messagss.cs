using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sp_back.Migrations
{
    /// <inheritdoc />
    public partial class messagss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageAgents_Accounts_SenderId",
                table: "MessageAgents");

            migrationBuilder.DropIndex(
                name: "IX_MessageAgents_SenderId",
                table: "MessageAgents");

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "MessageAgents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageAgents_AccountId",
                table: "MessageAgents",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageAgents_Accounts_AccountId",
                table: "MessageAgents",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageAgents_Accounts_AccountId",
                table: "MessageAgents");

            migrationBuilder.DropIndex(
                name: "IX_MessageAgents_AccountId",
                table: "MessageAgents");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "MessageAgents");

            migrationBuilder.CreateIndex(
                name: "IX_MessageAgents_SenderId",
                table: "MessageAgents",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageAgents_Accounts_SenderId",
                table: "MessageAgents",
                column: "SenderId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
