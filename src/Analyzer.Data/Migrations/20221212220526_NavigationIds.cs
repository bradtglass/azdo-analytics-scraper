using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Analyzer.Data.Migrations
{
    /// <inheritdoc />
    public partial class NavigationIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PullRequests_Commits_MergeCommitSha",
                schema: "azdo",
                table: "PullRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PullRequests_Identities_IdentityId",
                schema: "azdo",
                table: "PullRequests");

            migrationBuilder.DropIndex(
                name: "IX_Pushes_DevOpsId",
                schema: "azdo",
                table: "Pushes");

            migrationBuilder.DropIndex(
                name: "IX_PullRequests_DevOpsId",
                schema: "azdo",
                table: "PullRequests");

            migrationBuilder.DropIndex(
                name: "IX_PullRequests_IdentityId",
                schema: "azdo",
                table: "PullRequests");

            migrationBuilder.DropIndex(
                name: "IX_PullRequests_MergeCommitSha",
                schema: "azdo",
                table: "PullRequests");

            migrationBuilder.DropColumn(
                name: "IdentityId",
                schema: "azdo",
                table: "PullRequests");

            migrationBuilder.DropColumn(
                name: "MergeCommitSha",
                schema: "azdo",
                table: "PullRequests");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                schema: "azdo",
                table: "PullRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<byte[]>(
                name: "MergeCommitId",
                schema: "azdo",
                table: "PullRequests",
                type: "varbinary(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pushes_DevOpsId",
                schema: "azdo",
                table: "Pushes",
                column: "DevOpsId");

            migrationBuilder.CreateIndex(
                name: "IX_PullRequests_CreatedById",
                schema: "azdo",
                table: "PullRequests",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PullRequests_DevOpsId",
                schema: "azdo",
                table: "PullRequests",
                column: "DevOpsId");

            migrationBuilder.CreateIndex(
                name: "IX_PullRequests_MergeCommitId",
                schema: "azdo",
                table: "PullRequests",
                column: "MergeCommitId",
                unique: true,
                filter: "[MergeCommitId] IS NOT NULL");

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
                name: "FK_PullRequests_Identities_CreatedById",
                schema: "azdo",
                table: "PullRequests",
                column: "CreatedById",
                principalSchema: "azdo",
                principalTable: "Identities",
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
                name: "FK_PullRequests_Identities_CreatedById",
                schema: "azdo",
                table: "PullRequests");

            migrationBuilder.DropIndex(
                name: "IX_Pushes_DevOpsId",
                schema: "azdo",
                table: "Pushes");

            migrationBuilder.DropIndex(
                name: "IX_PullRequests_CreatedById",
                schema: "azdo",
                table: "PullRequests");

            migrationBuilder.DropIndex(
                name: "IX_PullRequests_DevOpsId",
                schema: "azdo",
                table: "PullRequests");

            migrationBuilder.DropIndex(
                name: "IX_PullRequests_MergeCommitId",
                schema: "azdo",
                table: "PullRequests");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                schema: "azdo",
                table: "PullRequests");

            migrationBuilder.DropColumn(
                name: "MergeCommitId",
                schema: "azdo",
                table: "PullRequests");

            migrationBuilder.AddColumn<Guid>(
                name: "IdentityId",
                schema: "azdo",
                table: "PullRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "MergeCommitSha",
                schema: "azdo",
                table: "PullRequests",
                type: "varbinary(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pushes_DevOpsId",
                schema: "azdo",
                table: "Pushes",
                column: "DevOpsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PullRequests_DevOpsId",
                schema: "azdo",
                table: "PullRequests",
                column: "DevOpsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PullRequests_IdentityId",
                schema: "azdo",
                table: "PullRequests",
                column: "IdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_PullRequests_MergeCommitSha",
                schema: "azdo",
                table: "PullRequests",
                column: "MergeCommitSha");

            migrationBuilder.AddForeignKey(
                name: "FK_PullRequests_Commits_MergeCommitSha",
                schema: "azdo",
                table: "PullRequests",
                column: "MergeCommitSha",
                principalSchema: "azdo",
                principalTable: "Commits",
                principalColumn: "Sha");

            migrationBuilder.AddForeignKey(
                name: "FK_PullRequests_Identities_IdentityId",
                schema: "azdo",
                table: "PullRequests",
                column: "IdentityId",
                principalSchema: "azdo",
                principalTable: "Identities",
                principalColumn: "Id");
        }
    }
}
