using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Analyzer.Data;

[Index(nameof(Sha), IsUnique = true)]
public class Commit
{
    public Commit(GitSha sha, DateTimeOffset commitTimestamp, DateTimeOffset authorTimestamp, string message)
    {
        Sha = sha;
        CommitTimestamp = commitTimestamp;
        AuthorTimestamp = authorTimestamp;
        Message = message;
    }
    
    [Key]    
    public GitSha Sha { get; set; }

    public Identity Commiter { get; set; } = null!;

    public Identity Author { get; set; } = null!;
    
    public Guid CommiterId { get; set; }

    public Guid AuthorId { get; set; }

    public DateTimeOffset CommitTimestamp { get; set; }

    public DateTimeOffset AuthorTimestamp { get; set; }

    public string Message { get; set; }

    public Push Push { get; set; } = null!;
    
    public PullRequest? MergingPullRequest { get; set; }
}