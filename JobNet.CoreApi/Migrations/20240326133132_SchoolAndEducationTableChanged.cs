using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobNet.CoreApi.Migrations
{
    /// <inheritdoc />
    public partial class SchoolAndEducationTableChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SchoolName",
                table: "Educations");

            migrationBuilder.AddColumn<int>(
                name: "SchoolId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Educations",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Educations",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Users_SchoolId",
                table: "Users",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Schools_SchoolId",
                table: "Users",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "SchoolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Schools_SchoolId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SchoolId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Educations");

            migrationBuilder.AddColumn<string>(
                name: "SchoolName",
                table: "Educations",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
