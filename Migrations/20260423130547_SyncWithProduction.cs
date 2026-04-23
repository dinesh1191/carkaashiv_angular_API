using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace carkaashiv_angular_API.Migrations
{
    /// <inheritdoc />
    public partial class SyncWithProduction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_items_order_id",
                table: "tbl_order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_items_part_id",
                table: "tbl_order_items",
                column: "part_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_order_items_tbl_orders_order_id",
                table: "tbl_order_items",
                column: "order_id",
                principalTable: "tbl_orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_order_items_tbl_part_part_id",
                table: "tbl_order_items",
                column: "part_id",
                principalTable: "tbl_part",
                principalColumn: "part_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_order_items_tbl_orders_order_id",
                table: "tbl_order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_order_items_tbl_part_part_id",
                table: "tbl_order_items");

            migrationBuilder.DropIndex(
                name: "IX_tbl_order_items_order_id",
                table: "tbl_order_items");

            migrationBuilder.DropIndex(
                name: "IX_tbl_order_items_part_id",
                table: "tbl_order_items");
        }
    }
}
