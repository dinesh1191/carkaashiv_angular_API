using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace carkaashiv_angular_API.Migrations
{
    /// <inheritdoc />
    public partial class RefactorOrderAndPaymentStatusEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
        name: "status",
        table: "tbl_orders",
        newName: "order_status");

            // Remove old default ('Pending')
            migrationBuilder.Sql("""
        ALTER TABLE tbl_orders
        ALTER COLUMN order_status DROP DEFAULT;
    """);

            // Drop old string payment_status column
            migrationBuilder.DropColumn(
                name: "payment_status",
                table: "tbl_orders");

            // Recreate payment_status as integer enum
            migrationBuilder.AddColumn<int>(
                name: "payment_status",
                table: "tbl_orders",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            // Convert order_status text → integer
            migrationBuilder.Sql("""
        ALTER TABLE tbl_orders
        ALTER COLUMN order_status TYPE integer
        USING CASE
            WHEN order_status = 'Pending' THEN 1
            WHEN order_status = 'Confirmed' THEN 2
            WHEN order_status = 'Shipped' THEN 3
            WHEN order_status = 'Delivered' THEN 4
            WHEN order_status = 'Cancelled' THEN 5
            ELSE 1
        END;
    """);

            // Add new integer default
            migrationBuilder.Sql("""
        ALTER TABLE tbl_orders
        ALTER COLUMN order_status SET DEFAULT 1;
    """);
        }
            
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "order_status",
                table: "tbl_orders",
                newName: "status");

       

            migrationBuilder.AlterColumn<string>(
                name: "payment_status",
                table: "tbl_orders",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "tbl_orders",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
