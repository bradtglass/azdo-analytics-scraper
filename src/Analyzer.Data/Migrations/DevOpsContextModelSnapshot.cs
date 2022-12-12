﻿// <auto-generated />
using System;
using Analyzer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Analyzer.Data.Migrations
{
    [DbContext(typeof(DevOpsContext))]
    partial class DevOpsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("azdo")
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Analyzer.Data.Commit", b =>
                {
                    b.Property<byte[]>("Sha")
                        .HasMaxLength(20)
                        .HasColumnType("varbinary(20)");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("AuthorTimestamp")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("CommitTimestamp")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("CommiterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PushId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Sha");

                    b.HasIndex("AuthorId");

                    b.HasIndex("CommiterId");

                    b.HasIndex("PushId");

                    b.HasIndex("Sha")
                        .IsUnique();

                    b.ToTable("Commits", "azdo");
                });

            modelBuilder.Entity("Analyzer.Data.Identity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("DevOpsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("DevOpsId")
                        .IsUnique()
                        .HasFilter("[DevOpsId] IS NOT NULL");

                    b.HasIndex("UniqueName")
                        .IsUnique();

                    b.ToTable("Identities", "azdo");
                });

            modelBuilder.Entity("Analyzer.Data.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DevOpsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Organisation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DevOpsId")
                        .IsUnique();

                    b.ToTable("Projects", "azdo");
                });

            modelBuilder.Entity("Analyzer.Data.PullRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset?>("ClosedTimestamp")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedTimestamp")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("DevOpsId")
                        .HasColumnType("int");

                    b.Property<byte[]>("MergeCommitId")
                        .HasMaxLength(20)
                        .HasColumnType("varbinary(20)");

                    b.Property<Guid>("RepositoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("DevOpsId");

                    b.HasIndex("MergeCommitId")
                        .IsUnique()
                        .HasFilter("[MergeCommitId] IS NOT NULL");

                    b.HasIndex("RepositoryId");

                    b.ToTable("PullRequests", "azdo");
                });

            modelBuilder.Entity("Analyzer.Data.Push", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DevOpsId")
                        .HasColumnType("int");

                    b.Property<Guid>("IdentityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RepositoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("DevOpsId");

                    b.HasIndex("IdentityId");

                    b.HasIndex("RepositoryId");

                    b.ToTable("Pushes", "azdo");
                });

            modelBuilder.Entity("Analyzer.Data.Repository", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DevOpsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("DevOpsId")
                        .IsUnique();

                    b.HasIndex("ProjectId");

                    b.ToTable("Repositories", "azdo");
                });

            modelBuilder.Entity("Analyzer.Data.Commit", b =>
                {
                    b.HasOne("Analyzer.Data.Identity", "Author")
                        .WithMany("AuthoredCommits")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Analyzer.Data.Identity", "Commiter")
                        .WithMany("CommitedCommits")
                        .HasForeignKey("CommiterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Analyzer.Data.Push", "Push")
                        .WithMany("Commits")
                        .HasForeignKey("PushId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Commiter");

                    b.Navigation("Push");
                });

            modelBuilder.Entity("Analyzer.Data.PullRequest", b =>
                {
                    b.HasOne("Analyzer.Data.Identity", "CreatedBy")
                        .WithMany("PullRequests")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Analyzer.Data.Commit", "MergeCommit")
                        .WithOne("MergingPullRequest")
                        .HasForeignKey("Analyzer.Data.PullRequest", "MergeCommitId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Analyzer.Data.Repository", "Repository")
                        .WithMany("PullRequests")
                        .HasForeignKey("RepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");

                    b.Navigation("MergeCommit");

                    b.Navigation("Repository");
                });

            modelBuilder.Entity("Analyzer.Data.Push", b =>
                {
                    b.HasOne("Analyzer.Data.Identity", "Identity")
                        .WithMany("Pushes")
                        .HasForeignKey("IdentityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Analyzer.Data.Repository", "Repository")
                        .WithMany("Pushes")
                        .HasForeignKey("RepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Identity");

                    b.Navigation("Repository");
                });

            modelBuilder.Entity("Analyzer.Data.Repository", b =>
                {
                    b.HasOne("Analyzer.Data.Project", "Project")
                        .WithMany("Repositories")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Analyzer.Data.Commit", b =>
                {
                    b.Navigation("MergingPullRequest");
                });

            modelBuilder.Entity("Analyzer.Data.Identity", b =>
                {
                    b.Navigation("AuthoredCommits");

                    b.Navigation("CommitedCommits");

                    b.Navigation("PullRequests");

                    b.Navigation("Pushes");
                });

            modelBuilder.Entity("Analyzer.Data.Project", b =>
                {
                    b.Navigation("Repositories");
                });

            modelBuilder.Entity("Analyzer.Data.Push", b =>
                {
                    b.Navigation("Commits");
                });

            modelBuilder.Entity("Analyzer.Data.Repository", b =>
                {
                    b.Navigation("PullRequests");

                    b.Navigation("Pushes");
                });
#pragma warning restore 612, 618
        }
    }
}
