using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Analyzer.Data;

[Index(nameof(DevOpsId), IsUnique = true)]
[Index(nameof(UniqueName), IsUnique = true)]
public class Identity : IHasDevOpsId<DevOpsGuid?>
{
    public Guid Id { get; set; }

    public DevOpsGuid? DevOpsId { get; set; }

    public string DisplayName { get; set; } = null!;

    public string UniqueName { get; set; } = null!;

    public List<Commit> AuthoredCommits { get; } = new();

    public List<Commit> CommitedCommits { get; } = new();

    public List<PullRequest> PullRequests { get; } = new();
}