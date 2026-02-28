using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace carkaashiv_angular_API.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAndUpdatedToPart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "idt",
                table: "tbl_part",
                newName: "created_at");

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "tbl_part",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "tbl_part");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "tbl_part",
                newName: "idt");
        }
    }
}
