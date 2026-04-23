using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace carkaashiv_angular_API.Migrations
{
#nullable disable
    /// <inheritdoc />
    public partial class AlignProdSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =========================
            // tbl_orders fixes
            // =========================

            migrationBuilder.AddColumn<string>(
                name: "invoice_number",
                table: "tbl_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "subtotal_amount",
                table: "tbl_orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0.0m);

            migrationBuilder.AddColumn<decimal>(
                name: "tax_amount",
                table: "tbl_orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0.0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "total_amount",
                table: "tbl_orders",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0.0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "tbl_orders",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "tbl_orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            // =========================
            // tbl_order_items fixes
            // =========================
            migrationBuilder.AlterColumn<int>(
          name: "order_id",
          table: "tbl_order_items",
          nullable: false,
          oldClrType: typeof(int),
          oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "part_id",
                table: "tbl_order_items",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "quantity",
                table: "tbl_order_items",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "unit_price",
                table: "tbl_order_items",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "total_price",
                table: "tbl_order_items",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            // =========================
            // tbl_cart fixes
            // =========================

            migrationBuilder.AlterColumn<DateTime>(
                name: "added_date",
                table: "tbl_cart",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_cart_tbl_part_part_id",
                table: "tbl_cart",
                column: "part_id",
                principalTable: "tbl_part",
                principalColumn: "part_id",
                onDelete: ReferentialAction.Cascade);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {  
            // Reverse changes (safe rollback)

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_cart_tbl_part_part_id",
                table: "tbl_cart");

            migrationBuilder.DropColumn(
                name: "invoice_number",
                table: "tbl_orders");

            migrationBuilder.DropColumn(
                name: "subtotal_amount",
                table: "tbl_orders");

            migrationBuilder.DropColumn(
                name: "tax_amount",
                table: "tbl_orders");

        }
    }
}
