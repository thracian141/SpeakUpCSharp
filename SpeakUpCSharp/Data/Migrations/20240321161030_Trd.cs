using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeakUpCSharp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Trd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SectionLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SectionId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    CourseCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SectionLinks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SectionLinks_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SectionLinks_SectionId",
                table: "SectionLinks",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionLinks_UserId",
                table: "SectionLinks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SectionLinks");
        }
    }
}
