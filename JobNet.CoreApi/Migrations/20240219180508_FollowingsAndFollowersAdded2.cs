using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobNet.CoreApi.Migrations
{
    /// <inheritdoc />
    public partial class FollowingsAndFollowersAdded2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRelationship_Users_UserId",
                table: "UserRelationship");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRelationship_Users_UserId1",
                table: "UserRelationship");

            migrationBuilder.DropIndex(
                name: "IX_UserRelationship_UserId",
                table: "UserRelationship");

            migrationBuilder.DropIndex(
                name: "IX_UserRelationship_UserId1",
                table: "UserRelationship");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserRelationship");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserRelationship");

            migrationBuilder.RenameColumn(
                name: "FollowingId",
                table: "UserRelationship",
                newName: "FollowingUserId");

            migrationBuilder.RenameColumn(
                name: "FollowerId",
                table: "UserRelationship",
                newName: "FollowerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRelationship_FollowerUserId",
                table: "UserRelationship",
                column: "FollowerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRelationship_FollowingUserId",
                table: "UserRelationship",
                column: "FollowingUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRelationship_Users_FollowerUserId",
                table: "UserRelationship",
                column: "FollowerUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRelationship_Users_FollowingUserId",
                table: "UserRelationship",
                column: "FollowingUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRelationship_Users_FollowerUserId",
                table: "UserRelationship");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRelationship_Users_FollowingUserId",
                table: "UserRelationship");

            migrationBuilder.DropIndex(
                name: "IX_UserRelationship_FollowerUserId",
                table: "UserRelationship");

            migrationBuilder.DropIndex(
                name: "IX_UserRelationship_FollowingUserId",
                table: "UserRelationship");

            migrationBuilder.RenameColumn(
                name: "FollowingUserId",
                table: "UserRelationship",
                newName: "FollowingId");

            migrationBuilder.RenameColumn(
                name: "FollowerUserId",
                table: "UserRelationship",
                newName: "FollowerId");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserRelationship",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "UserRelationship",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRelationship_UserId",
                table: "UserRelationship",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRelationship_UserId1",
                table: "UserRelationship",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRelationship_Users_UserId",
                table: "UserRelationship",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRelationship_Users_UserId1",
                table: "UserRelationship",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
