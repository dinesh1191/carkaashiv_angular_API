using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace carkaashiv_angular_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgresBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderIdempotencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdempotencyKey = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderIdempotencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderPayments",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    payment_method = table.Column<string>(type: "text", nullable: false),
                    payment_reference = table.Column<string>(type: "text", nullable: false),
                    payment_proof_url = table.Column<string>(type: "text", nullable: true),
                    submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPayments", x => x.payment_id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_emp",
                columns: table => new
                {
                    emp_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    emp_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    emp_phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    emp_email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    emp_role = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    emp_pass = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_emp", x => x.emp_id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_part",
                columns: table => new
                {
                    part_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    emp_id = table.Column<int>(type: "integer", nullable: false),
                    part_name = table.Column<string>(type: "text", nullable: false),
                    part_detail = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    part_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    part_stock = table.Column<int>(type: "integer", nullable: false),
                    partimage_key = table.Column<string>(name: "part-image_key", type: "text", nullable: true),
                    part_image = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_part", x => x.part_id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_user",
                columns: table => new
                {
                    u_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    u_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    u_phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    u_email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    u_pass = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    u_role = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_user", x => x.u_id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_cart",
                columns: table => new
                {
                    cart_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    u_id = table.Column<int>(type: "integer", nullable: false),
                    part_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    added_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "Now()"),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_cart", x => x.cart_id);
                    table.ForeignKey(
                        name: "FK_tbl_cart_tbl_part_part_id",
                        column: x => x.part_id,
                        principalTable: "tbl_part",
                        principalColumn: "part_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_cart_tbl_user_u_id",
                        column: x => x.u_id,
                        principalTable: "tbl_user",
                        principalColumn: "u_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_orders",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    u_id = table.Column<int>(type: "integer", nullable: false),
                    subtotal_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    order_status = table.Column<int>(type: "integer", nullable: false),
                    invoice_number = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    payment_method = table.Column<string>(type: "text", nullable: true),
                    payment_reference = table.Column<string>(type: "text", nullable: true),
                    payment_proof_url = table.Column<string>(type: "text", nullable: true),
                    payment_status = table.Column<int>(type: "integer", nullable: false),
                    payment_submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    verified_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    payment_mismatch_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    payment_verified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_orders", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_tbl_orders_tbl_user_u_id",
                        column: x => x.u_id,
                        principalTable: "tbl_user",
                        principalColumn: "u_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_order_items",
                columns: table => new
                {
                    order_item_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    part_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_order_items", x => x.order_item_id);
                    table.ForeignKey(
                        name: "FK_tbl_order_items_tbl_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "tbl_orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_order_items_tbl_part_part_id",
                        column: x => x.part_id,
                        principalTable: "tbl_part",
                        principalColumn: "part_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderIdempotencies_UserId_IdempotencyKey",
                table: "OrderIdempotencies",
                columns: new[] { "UserId", "IdempotencyKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_cart_part_id",
                table: "tbl_cart",
                column: "part_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_cart_u_id",
                table: "tbl_cart",
                column: "u_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_items_order_id",
                table: "tbl_order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_items_part_id",
                table: "tbl_order_items",
                column: "part_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_orders_u_id",
                table: "tbl_orders",
                column: "u_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderIdempotencies");

            migrationBuilder.DropTable(
                name: "OrderPayments");

            migrationBuilder.DropTable(
                name: "tbl_cart");

            migrationBuilder.DropTable(
                name: "tbl_emp");

            migrationBuilder.DropTable(
                name: "tbl_order_items");

            migrationBuilder.DropTable(
                name: "tbl_orders");

            migrationBuilder.DropTable(
                name: "tbl_part");

            migrationBuilder.DropTable(
                name: "tbl_user");
        }
    }
}
