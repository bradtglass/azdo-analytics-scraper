using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Analyzer.Data;

[Index(nameof(DevOpsId), IsUnique = true)]
public class Repository
{
    public Repository(DevOpsGuid devOpsId)
    {
        DevOpsId = devOpsId;
    }

    public Guid Id { get; set; }

    public DevOpsGuid DevOpsId { get; set; }

    public string Name { get; set; } = null!;

    public Project Project { get; set; } = null!;

    public List<PullRequest> PullRequests { get; } = new();

    public List<Push> Pushes { get; } = new();
}