using System;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index(nameof(DevOpsId), IsUnique = true)]
public class PullRequest
{
    public required Guid Id { get; set; }

    public required string Title { get; set; }

    public required DevOpsIntId DevOpsId { get; set; }

    public required DateTimeOffset CreatedTimestamp { get; set; }

    public required Repository Repository { get; set; }

    public required PullRequestState State { get; set; }

    public DateTimeOffset? ClosedTimestamp { get; set; }

    public Commit? MergeCommit { get; set; }
}