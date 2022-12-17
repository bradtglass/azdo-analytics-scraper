using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Analyzer.Data.Migrations
{
    /// <inheritdoc />
    public partial class GitShaAsString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PullRequests_Commits_MergeCommitId",
                schema: "azdo",
                table: "PullRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Commits_Pushes_PushId",
                schema: "azdo",
                table: "Commits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Commits",
                schema: "azdo",
                table: "Commits");
            
            migrationBuilder.AlterColumn<string>(
                name: "MergeCommitId",
                schema: "azdo",
                table: "PullRequests",
                type: "nchar(40)",
                fixedLength: true,
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Sha",
                schema: "azdo",
                table: "Commits",
                type: "nchar(40)",
                fixedLength: true,
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(20)",
                oldMaxLength: 20);
            
            migrationBuilder.AddPrimaryKey(
                name: "PK_Commits",
                schema: "azdo",
                table: "Commits",
                column: "Sha");

            migrationBuilder.AddForeignKey(
                name: "FK_PullRequests_Commits_MergeCommitId",
                schema: "azdo",
                table: "PullRequests",
                column: "MergeCommitId",
                principalSchema: "azdo",
                principalTable: "Commits",
                principalColumn: "Sha",
                onDelete: ReferentialAction.Restrict);
            
            migrationBuilder.AddForeignKey(
                name: "FK_Commits_Pushes_PushId",
                schema: "azdo",
                table: "Commits",
                column: "PushId",
                principalSchema: "azdo",
                principalTable: "Pushes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PullRequests_Commits_MergeCommitId",
                schema: "azdo",
                table: "PullRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Commits_Pushes_PushId",
                schema: "azdo",
                table: "Commits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Commits",
                schema: "azdo",
                table: "Commits");
            
            migrationBuilder.AlterColumn<byte[]>(
                name: "MergeCommitId",
                schema: "azdo",
                table: "PullRequests",
                type: "varbinary(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(40)",
                oldFixedLength: true,
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Sha",
                schema: "azdo",
                table: "Commits",
                type: "varbinary(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nchar(40)",
                oldFixedLength: true,
                oldMaxLength: 40);
            
            migrationBuilder.AddPrimaryKey(
                name: "PK_Commits",
                schema: "azdo",
                table: "Commits",
                column: "Sha");

            migrationBuilder.AddForeignKey(
                name: "FK_PullRequests_Commits_MergeCommitId",
                schema: "azdo",
                table: "PullRequests",
                column: "MergeCommitId",
                principalSchema: "azdo",
                principalTable: "Commits",
                principalColumn: "Sha",
                onDelete: ReferentialAction.Restrict);
            
            migrationBuilder.AddForeignKey(
                name: "FK_Commits_Pushes_PushId",
                schema: "azdo",
                table: "Commits",
                column: "PushId",
                principalSchema: "azdo",
                principalTable: "Pushes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
