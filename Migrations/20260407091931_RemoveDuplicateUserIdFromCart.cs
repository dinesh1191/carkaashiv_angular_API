using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace carkaashiv_angular_API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicateUserIdFromCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
             name: "FK_tbl_cart_tbl_user_UserId",
             table: "tbl_cart");

            migrationBuilder.DropIndex(
                name: "IX_tbl_cart_UserId",
                table: "tbl_cart");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "tbl_cart");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
            name: "UserId",
            table: "tbl_cart",
            type: "integer",
            nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_cart_UserId",
                table: "tbl_cart",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_cart_tbl_user_UserId",
                table: "tbl_cart",
                column: "UserId",
                principalTable: "tbl_user",
                principalColumn: "u_id");
        }
    }
}
