using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataContext.Migrations
{
    /// <inheritdoc />
    public partial class CryptoPriceLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "Cryptos",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "CryptoPriceLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CryptoId = table.Column<int>(type: "int", nullable: false),
                    OldPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NewPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoPriceLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CryptoPriceLogs_Cryptos_CryptoId",
                        column: x => x.CryptoId,
                        principalTable: "Cryptos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CryptoPriceLogs_CryptoId",
                table: "CryptoPriceLogs",
                column: "CryptoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CryptoPriceLogs");

            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "Cryptos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(6)",
                oldMaxLength: 6);
        }
    }
}
