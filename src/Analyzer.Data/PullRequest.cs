using System;
using Microsoft.EntityFrameworkCore;

namespace Analyzer.Data;

[Index(nameof(DevOpsId), IsUnique = false)]
public class PullRequest : IHasDevOpsId<DevOpsIntId>
{
    public PullRequest(string title, DateTimeOffset createdTimestamp, DateTimeOffset? closedTimestamp, PullRequestState state, DevOpsIntId devOpsId)
    {
        Title = title;
        CreatedTimestamp = createdTimestamp;
        State = state;
        ClosedTimestamp = closedTimestamp;
        DevOpsId = devOpsId;
    }

    public Guid Id { get; set; }

    public string Title { get; set; }

    public DateTimeOffset CreatedTimestamp { get; set; }

    public Repository Repository { get; set; } = null!;

    public PullRequestState State { get; set; }

    public DateTimeOffset? ClosedTimestamp { get; set; }

    public Commit? MergeCommit { get; set; }
    
    public byte[]? MergeCommitId { get; set; }

    public DevOpsIntId DevOpsId { get; set; }

    public Identity CreatedBy { get; set; } = null!;
    
    public Guid CreatedById { get; set; }
}