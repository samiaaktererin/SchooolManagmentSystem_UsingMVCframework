using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMvcApp.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentRoll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Roll",
                table: "Students",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roll",
                table: "Students");
        }
    }
}
