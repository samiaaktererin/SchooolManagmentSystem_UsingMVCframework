using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyMvcApp.Migrations
{
    /// <inheritdoc />
    public partial class AddClassSectionDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Classrooms",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Class 1" },
                    { 2, "Class 2" },
                    { 3, "Class 3" },
                    { 4, "Class 4" },
                    { 5, "Class 5" },
                    { 6, "Class 6" },
                    { 7, "Class 7" },
                    { 8, "Class 8" },
                    { 9, "Class 9" },
                    { 10, "Class 10" }
                });

            migrationBuilder.InsertData(
                table: "Sections",
                columns: new[] { "Id", "ClassroomId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "A" },
                    { 2, 1, "B" },
                    { 3, 1, "C" },
                    { 4, 1, "D" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Classrooms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Classrooms",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Classrooms",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Classrooms",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Classrooms",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Classrooms",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Classrooms",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Classrooms",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Classrooms",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Classrooms",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
