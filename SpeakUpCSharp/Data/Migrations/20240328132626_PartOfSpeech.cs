using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeakUpCSharp.Data.Migrations
{
    /// <inheritdoc />
    public partial class PartOfSpeech : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PartOfSpeech",
                table: "CourseCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartOfSpeech",
                table: "CourseCards");
        }
    }
}
