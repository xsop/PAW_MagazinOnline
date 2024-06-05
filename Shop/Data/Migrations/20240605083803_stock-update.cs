using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.Data.Migrations
{
    /// <inheritdoc />
    public partial class stockupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stock_ItemId",
                table: "Stock");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_ItemId",
                table: "Stock",
                column: "ItemId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stock_ItemId",
                table: "Stock");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_ItemId",
                table: "Stock",
                column: "ItemId");
        }
    }
}
