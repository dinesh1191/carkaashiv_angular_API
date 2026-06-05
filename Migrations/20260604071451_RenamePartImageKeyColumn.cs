using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace carkaashiv_angular_API.Migrations
{
    /// <inheritdoc />
    public partial class RenamePartImageKeyColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        migrationBuilder.RenameColumn(
        name: "part-image_key",
        table: "tbl_part",
        newName: "part_image_key");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           migrationBuilder.RenameColumn(
           name: "part_image_key",
           table: "tbl_part",
           newName: "part-image_key");
        }
    }
}
