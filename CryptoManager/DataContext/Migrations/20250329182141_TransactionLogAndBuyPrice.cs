using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataContext.Migrations
{
    /// <inheritdoc />
    public partial class TransactionLogAndBuyPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BuyPrice",
                table: "WalletCrypto",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "TransactionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CryptoId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PricePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionLogs_Cryptos_CryptoId",
                        column: x => x.CryptoId,
                        principalTable: "Cryptos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_CryptoId",
                table: "TransactionLogs",
                column: "CryptoId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_UserId",
                table: "TransactionLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionLogs");

            migrationBuilder.DropColumn(
                name: "BuyPrice",
                table: "WalletCrypto");
        }
    }
}
