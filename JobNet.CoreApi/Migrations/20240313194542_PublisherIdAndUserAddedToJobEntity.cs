using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobNet.CoreApi.Migrations
{
    /// <inheritdoc />
    public partial class PublisherIdAndUserAddedToJobEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PublisherId",
                table: "Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PublisherUserUserId",
                table: "Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_PublisherUserUserId",
                table: "Jobs",
                column: "PublisherUserUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_PublisherUserUserId",
                table: "Jobs",
                column: "PublisherUserUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_PublisherUserUserId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_PublisherUserUserId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "PublisherId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "PublisherUserUserId",
                table: "Jobs");
        }
    }
}
