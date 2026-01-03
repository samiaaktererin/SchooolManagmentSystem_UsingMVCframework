using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMvcApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSectionToTeacherSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "TeacherSubjects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjects_SectionId",
                table: "TeacherSubjects",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherSubjects_Sections_SectionId",
                table: "TeacherSubjects",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherSubjects_Sections_SectionId",
                table: "TeacherSubjects");

            migrationBuilder.DropIndex(
                name: "IX_TeacherSubjects_SectionId",
                table: "TeacherSubjects");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "TeacherSubjects");
        }
    }
}
