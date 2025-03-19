using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sp_back.Migrations
{
    /// <inheritdoc />
    public partial class newClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageAgents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    ReceiverId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    AccountId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageAgents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageAgents_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MessageAgents_Accounts_AccountId1",
                        column: x => x.AccountId1,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MessageAgents_Accounts_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MessageAgents_Accounts_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageAgents_AccountId",
                table: "MessageAgents",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageAgents_AccountId1",
                table: "MessageAgents",
                column: "AccountId1");

            migrationBuilder.CreateIndex(
                name: "IX_MessageAgents_ReceiverId",
                table: "MessageAgents",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageAgents_SenderId",
                table: "MessageAgents",
                column: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageAgents");
        }
    }
}
