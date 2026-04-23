using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace carkaashiv_angular_API.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingTablesToProd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
            name: "OrderIdempotencies",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),

                IdempotencyKey = table.Column<string>(type: "text", nullable: false),

                UserId = table.Column<int>(nullable: false),

                OrderId = table.Column<int>(nullable: false),

                CreatedAt = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: false,
                    defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrderIdempotencies", x => x.Id);
            });

            migrationBuilder.CreateIndex(
                name: "IX_OrderIdempotencies_UserId_IdempotencyKey",
                table: "OrderIdempotencies",
                columns: new[] { "UserId", "IdempotencyKey" },
                unique: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
            name: "OrderIdempotencies");

        }
    }
}
