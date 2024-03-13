using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeakUpCSharp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedSections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastReviewDate",
                table: "CourseSections");

            migrationBuilder.DropColumn(
                name: "NextReviewDate",
                table: "CourseSections");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "CourseSections",
                newName: "LastEditorId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CourseSections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEdited",
                table: "CourseSections",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_CourseSections_LastEditorId",
                table: "CourseSections",
                column: "LastEditorId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSections_AspNetUsers_LastEditorId",
                table: "CourseSections",
                column: "LastEditorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSections_AspNetUsers_LastEditorId",
                table: "CourseSections");

            migrationBuilder.DropIndex(
                name: "IX_CourseSections_LastEditorId",
                table: "CourseSections");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CourseSections");

            migrationBuilder.DropColumn(
                name: "LastEdited",
                table: "CourseSections");

            migrationBuilder.RenameColumn(
                name: "LastEditorId",
                table: "CourseSections",
                newName: "Level");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReviewDate",
                table: "CourseSections",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextReviewDate",
                table: "CourseSections",
                type: "datetime2",
                nullable: true);
        }
    }
}
