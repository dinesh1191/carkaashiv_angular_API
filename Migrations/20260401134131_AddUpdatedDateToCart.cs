using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace carkaashiv_angular_API.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedDateToCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "tbl_cart",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "tbl_cart",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_cart_part_id",
                table: "tbl_cart",
                column: "part_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_cart_UserId",
                table: "tbl_cart",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_cart_tbl_part_part_id",
                table: "tbl_cart",
                column: "part_id",
                principalTable: "tbl_part",
                principalColumn: "part_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_cart_tbl_user_UserId",
                table: "tbl_cart",
                column: "UserId",
                principalTable: "tbl_user",
                principalColumn: "u_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_cart_tbl_part_part_id",
                table: "tbl_cart");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_cart_tbl_user_UserId",
                table: "tbl_cart");

            migrationBuilder.DropIndex(
                name: "IX_tbl_cart_part_id",
                table: "tbl_cart");

            migrationBuilder.DropIndex(
                name: "IX_tbl_cart_UserId",
                table: "tbl_cart");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "tbl_cart");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "tbl_cart");
        }
    }
}
