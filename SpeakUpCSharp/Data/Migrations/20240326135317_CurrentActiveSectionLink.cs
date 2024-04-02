using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeakUpCSharp.Data.Migrations
{
    /// <inheritdoc />
    public partial class CurrentActiveSectionLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Completed",
                table: "SectionLinks",
                newName: "CurrentActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentActive",
                table: "SectionLinks",
                newName: "Completed");
        }
    }
}
