using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileUploader.Migrations
{
    /// <inheritdoc />
    public partial class AddSizeAndMimeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MimeType",
                table: "Files",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "Files",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MimeType",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Files");
        }
    }
}
