using System;
using Microsoft.EntityFrameworkCore;

namespace Analyzer.Data;

[Index(nameof(DevOpsId), IsUnique = false)]
public class PullRequest : IHasDevOpsId<DevOpsIntId>
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public required DevOpsIntId DevOpsId { get; set; }

    public required DateTimeOffset CreatedTimestamp { get; set; }

    public required Repository Repository { get; set; }

    public required PullRequestState State { get; set; }

    public DateTimeOffset? ClosedTimestamp { get; set; }

    public Commit? MergeCommit { get; set; }
}