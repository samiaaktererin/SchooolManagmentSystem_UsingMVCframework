using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyMvcApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Classrooms_ClassroomId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Sections_SectionId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Classrooms_ClassAssignedId",
                table: "Teachers");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherSubjects_Subjects_SubjectId",
                table: "TeacherSubjects");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "TeacherLeaves");

            migrationBuilder.DropIndex(
                name: "IX_TeacherSubjects_SubjectId",
                table: "TeacherSubjects");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "TeacherSubjects",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ClassroomId",
                table: "TeacherSubjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubjectName",
                table: "TeacherSubjects",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SalaryMonth",
                table: "TeacherSalaries",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "TeacherSalaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Teachers",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Teachers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "TeacherAttendances",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Students",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjects_ClassroomId",
                table: "TeacherSubjects",
                column: "ClassroomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Classrooms_ClassroomId",
                table: "Students",
                column: "ClassroomId",
                principalTable: "Classrooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Sections_SectionId",
                table: "Students",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Classrooms_ClassAssignedId",
                table: "Teachers",
                column: "ClassAssignedId",
                principalTable: "Classrooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherSubjects_Classrooms_ClassroomId",
                table: "TeacherSubjects",
                column: "ClassroomId",
                principalTable: "Classrooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Classrooms_ClassroomId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Sections_SectionId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Classrooms_ClassAssignedId",
                table: "Teachers");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherSubjects_Classrooms_ClassroomId",
                table: "TeacherSubjects");

            migrationBuilder.DropIndex(
                name: "IX_TeacherSubjects_ClassroomId",
                table: "TeacherSubjects");

            migrationBuilder.DropColumn(
                name: "ClassroomId",
                table: "TeacherSubjects");

            migrationBuilder.DropColumn(
                name: "SubjectName",
                table: "TeacherSubjects");

            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Teachers");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "TeacherSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SalaryMonth",
                table: "TeacherSalaries",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "TeacherSalaries",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Teachers",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "TeacherAttendances",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Students",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: true);

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TeacherLeaves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reason = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ToDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherLeaves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherLeaves_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Mathematics" },
                    { 2, "English" },
                    { 3, "Bangla" },
                    { 4, "Science" },
                    { 5, "Social Studies" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjects_SubjectId",
                table: "TeacherSubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLeaves_TeacherId",
                table: "TeacherLeaves",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Classrooms_ClassroomId",
                table: "Students",
                column: "ClassroomId",
                principalTable: "Classrooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Sections_SectionId",
                table: "Students",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Classrooms_ClassAssignedId",
                table: "Teachers",
                column: "ClassAssignedId",
                principalTable: "Classrooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherSubjects_Subjects_SubjectId",
                table: "TeacherSubjects",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
