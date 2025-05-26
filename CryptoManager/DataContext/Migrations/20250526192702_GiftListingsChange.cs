using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataContext.Migrations
{
    /// <inheritdoc />
    public partial class GiftListingsChange : Migration
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GiftListings_Users_RecieverUserId",
                table: "GiftListings");

            migrationBuilder.DropForeignKey(
                name: "FK_GiftListings_Users_SenderUserId",
                table: "GiftListings");

            migrationBuilder.AddForeignKey(
                name: "FK_GiftListings_Users_RecieverUserId",
                table: "GiftListings",
                column: "RecieverUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GiftListings_Users_SenderUserId",
                table: "GiftListings",
                column: "SenderUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
