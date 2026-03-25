using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace carkaashiv_angular_API.Migrations
{
    /// <inheritdoc />
    public partial class AddImageKeyPart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "part-image_key",
                table: "tbl_part",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "part-image_key",
                table: "tbl_part");
        }
    }
}
