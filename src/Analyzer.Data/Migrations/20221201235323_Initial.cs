using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Analyzer.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "azdo");

            migrationBuilder.CreateTable(
                name: "Identities",
                schema: "azdo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DevOpsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UniqueName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "azdo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DevOpsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Organisation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Repositories",
                schema: "azdo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DevOpsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repositories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repositories_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "azdo",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pushes",
                schema: "azdo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DevOpsId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RepositoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pushes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pushes_Identities_IdentityId",
                        column: x => x.IdentityId,
                        principalSchema: "azdo",
                        principalTable: "Identities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pushes_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalSchema: "azdo",
                        principalTable: "Repositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Commits",
                schema: "azdo",
                columns: table => new
                {
                    Sha = table.Column<byte[]>(type: "varbinary(20)", maxLength: 20, nullable: false),
                    CommiterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommitTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AuthorTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PushId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commits", x => x.Sha);
                    table.ForeignKey(
                        name: "FK_Commits_Identities_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "azdo",
                        principalTable: "Identities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commits_Identities_CommiterId",
                        column: x => x.CommiterId,
                        principalSchema: "azdo",
                        principalTable: "Identities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commits_Pushes_PushId",
                        column: x => x.PushId,
                        principalSchema: "azdo",
                        principalTable: "Pushes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PullRequests",
                schema: "azdo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DevOpsId = table.Column<int>(type: "int", nullable: false),
                    CreatedTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RepositoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    ClosedTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    MergeCommitSha = table.Column<byte[]>(type: "varbinary(20)", nullable: true),
                    IdentityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PullRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PullRequests_Commits_MergeCommitSha",
                        column: x => x.MergeCommitSha,
                        principalSchema: "azdo",
                        principalTable: "Commits",
                        principalColumn: "Sha");
                    table.ForeignKey(
                        name: "FK_PullRequests_Identities_IdentityId",
                        column: x => x.IdentityId,
                        principalSchema: "azdo",
                        principalTable: "Identities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PullRequests_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalSchema: "azdo",
                        principalTable: "Repositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Commits_AuthorId",
                schema: "azdo",
                table: "Commits",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Commits_CommiterId",
                schema: "azdo",
                table: "Commits",
                column: "CommiterId");

            migrationBuilder.CreateIndex(
                name: "IX_Commits_PushId",
                schema: "azdo",
                table: "Commits",
                column: "PushId");

            migrationBuilder.CreateIndex(
                name: "IX_Commits_Sha",
                schema: "azdo",
                table: "Commits",
                column: "Sha",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Identities_DevOpsId",
                schema: "azdo",
                table: "Identities",
                column: "DevOpsId",
                unique: true,
                filter: "[DevOpsId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Identities_UniqueName",
                schema: "azdo",
                table: "Identities",
                column: "UniqueName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_DevOpsId",
                schema: "azdo",
                table: "Projects",
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

            migrationBuilder.CreateIndex(
                name: "IX_PullRequests_RepositoryId",
                schema: "azdo",
                table: "PullRequests",
                column: "RepositoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Pushes_DevOpsId",
                schema: "azdo",
                table: "Pushes",
                column: "DevOpsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pushes_IdentityId",
                schema: "azdo",
                table: "Pushes",
                column: "IdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_Pushes_RepositoryId",
                schema: "azdo",
                table: "Pushes",
                column: "RepositoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_DevOpsId",
                schema: "azdo",
                table: "Repositories",
                column: "DevOpsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_ProjectId",
                schema: "azdo",
                table: "Repositories",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PullRequests",
                schema: "azdo");

            migrationBuilder.DropTable(
                name: "Commits",
                schema: "azdo");

            migrationBuilder.DropTable(
                name: "Pushes",
                schema: "azdo");

            migrationBuilder.DropTable(
                name: "Identities",
                schema: "azdo");

            migrationBuilder.DropTable(
                name: "Repositories",
                schema: "azdo");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "azdo");
        }
    }
}
