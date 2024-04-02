using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeakUpCSharp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SectionLinkHasCompletedProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "SectionLinks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "SectionLinks");
        }
    }
}
