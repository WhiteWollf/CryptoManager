using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataContext.Migrations
{
    /// <inheritdoc />
    public partial class GiftListingsChange2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GiftListings_Users_RecieverUserId",
                table: "GiftListings");

            migrationBuilder.DropForeignKey(
                name: "FK_GiftListings_Users_SenderUserId",
                table: "GiftListings");

            migrationBuilder.DropIndex(
                name: "IX_GiftListings_RecieverUserId",
                table: "GiftListings");

            migrationBuilder.DropIndex(
                name: "IX_GiftListings_SenderUserId",
                table: "GiftListings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_GiftListings_RecieverUserId",
                table: "GiftListings",
                column: "RecieverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GiftListings_SenderUserId",
                table: "GiftListings",
                column: "SenderUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GiftListings_Users_RecieverUserId",
                table: "GiftListings",
                column: "RecieverUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GiftListings_Users_SenderUserId",
                table: "GiftListings",
                column: "SenderUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
