using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace carkaashiv_angular_API.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "payment_method",
                table: "tbl_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment_reference",
                table: "tbl_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment_proof_url",
                table: "tbl_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment_status",
                table: "tbl_orders",
                type: "text",
                nullable: true,
                defaultValue: "Pending");

            migrationBuilder.AddColumn<DateTime>(
                name: "payment_submitted_at",
                table: "tbl_orders",
                type: "timestamp with time zone",
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "payment_method", table: "tbl_orders");
            migrationBuilder.DropColumn(name: "payment_reference", table: "tbl_orders");
            migrationBuilder.DropColumn(name: "payment_proof_url", table: "tbl_orders");
            migrationBuilder.DropColumn(name: "payment_status", table: "tbl_orders");
            migrationBuilder.DropColumn(name: "payment_submitted_at", table: "tbl_orders");

        }
    }
}
