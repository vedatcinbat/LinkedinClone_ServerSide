using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobNet.CoreApi.Migrations
{
    /// <inheritdoc />
    public partial class UserJobLikesRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserJobLikes");

            migrationBuilder.AddColumn<int>(
                name: "JobId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_JobId",
                table: "Users",
                column: "JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Jobs_JobId",
                table: "Users",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "JobId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Jobs_JobId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_JobId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "UserJobLikes",
                columns: table => new
                {
                    UserJobLikeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserJobLikes", x => x.UserJobLikeId);
                    table.ForeignKey(
                        name: "FK_UserJobLikes_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserJobLikes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserJobLikes_JobId",
                table: "UserJobLikes",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_UserJobLikes_UserId",
                table: "UserJobLikes",
                column: "UserId");
        }
    }
}
