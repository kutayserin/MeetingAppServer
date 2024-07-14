using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingApp.Migrations
{
    /// <inheritdoc />
    public partial class FixedDoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DocumentUrl",
                table: "Meetings",
                newName: "DocumentName");

            migrationBuilder.AddColumn<string>(
                name: "DocumentContentType",
                table: "Meetings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "DocumentData",
                table: "Meetings",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentContentType",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "DocumentData",
                table: "Meetings");

            migrationBuilder.RenameColumn(
                name: "DocumentName",
                table: "Meetings",
                newName: "DocumentUrl");
        }
    }
}
