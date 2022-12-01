using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Analyzer.Data;

[Index(nameof(DevOpsId), IsUnique = true)]
public class Repository
{
    public Guid Id { get; set; }

    public required DevOpsGuid DevOpsId { get; set; }

    public required string Name { get; set; }

    public required Project Project { get; set; }

    public List<PullRequest> PullRequests { get; } = new();

    public List<Push> Pushes { get; } = new();
}