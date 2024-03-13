using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeakUpCSharp.Data.Migrations
{
    /// <inheritdoc />
    public partial class kms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_CourseSections_SectionId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseSections_AspNetUsers_LastEditorId",
                table: "CourseSections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseSections",
                table: "CourseSections");

            migrationBuilder.RenameTable(
                name: "CourseSections",
                newName: "Sections");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Sections",
                newName: "Title");

            migrationBuilder.RenameIndex(
                name: "IX_CourseSections_LastEditorId",
                table: "Sections",
                newName: "IX_Sections_LastEditorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sections",
                table: "Sections",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Sections_SectionId",
                table: "Cards",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_AspNetUsers_LastEditorId",
                table: "Sections",
                column: "LastEditorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Sections_SectionId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Sections_AspNetUsers_LastEditorId",
                table: "Sections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sections",
                table: "Sections");

            migrationBuilder.RenameTable(
                name: "Sections",
                newName: "CourseSections");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "CourseSections",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Sections_LastEditorId",
                table: "CourseSections",
                newName: "IX_CourseSections_LastEditorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseSections",
                table: "CourseSections",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_CourseSections_SectionId",
                table: "Cards",
                column: "SectionId",
                principalTable: "CourseSections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSections_AspNetUsers_LastEditorId",
                table: "CourseSections",
                column: "LastEditorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
