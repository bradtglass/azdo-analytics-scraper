using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Analyzer.Data;

[Index(nameof(DevOpsId), IsUnique = true)]
[Index(nameof(UniqueName), IsUnique = true)]
public class Identity
{
    public Guid Id { get; set; }

    public DevOpsGuid? DevOpsId { get; set; }

    public required string DisplayName { get; set; }

    public required string UniqueName { get; set; }

    public List<Commit> AuthoredCommits { get; } = new();

    public List<Commit> CommitedCommits { get; } = new();

    public List<PullRequest> PullRequests { get; } = new();
}