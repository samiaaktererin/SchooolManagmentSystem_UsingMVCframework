using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyMvcApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherSubjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Approved",
                table: "TeacherLeaves");

            migrationBuilder.RenameColumn(
                name: "ForMonth",
                table: "TeacherSalaries",
                newName: "SalaryMonth");

            migrationBuilder.RenameColumn(
                name: "RequestedAt",
                table: "TeacherLeaves",
                newName: "AppliedAt");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "TeacherSalaries",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "TeacherSalaries",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Teachers",
                keyColumn: "Name",
                keyValue: null,
                column: "Name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Teachers",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Teachers",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ClassAssignedId",
                table: "Teachers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Teachers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "TeacherLeaves",
                keyColumn: "Reason",
                keyValue: null,
                column: "Reason",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "TeacherLeaves",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TeacherLeaves",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Mathematics");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Bangla");

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 4, "Science" },
                    { 5, "Social Studies" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_ClassAssignedId",
                table: "Teachers",
                column: "ClassAssignedId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Classrooms_ClassAssignedId",
                table: "Teachers",
                column: "ClassAssignedId",
                principalTable: "Classrooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Classrooms_ClassAssignedId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_ClassAssignedId",
                table: "Teachers");

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "Note",
                table: "TeacherSalaries");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "TeacherSalaries");

            migrationBuilder.DropColumn(
                name: "ClassAssignedId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TeacherLeaves");

            migrationBuilder.RenameColumn(
                name: "SalaryMonth",
                table: "TeacherSalaries",
                newName: "ForMonth");

            migrationBuilder.RenameColumn(
                name: "AppliedAt",
                table: "TeacherLeaves",
                newName: "RequestedAt");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Teachers",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Teachers",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Teachers",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Teachers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "TeacherLeaves",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "TeacherLeaves",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Math");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Science");
        }
    }
}
