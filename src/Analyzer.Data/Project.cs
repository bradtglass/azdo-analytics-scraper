using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Analyzer.Data;

[Index(nameof(DevOpsId), IsUnique = true)]
public class Project
{
    public Project(DevOpsGuid devOpsId)
    {
        DevOpsId = devOpsId;
    }

    public Guid Id { get; set; }

    public DevOpsGuid DevOpsId { get; set; }

    public string Name { get; set; } = null!;

    public List<Repository> Repositories { get; } = new();

    public string Organisation { get; set; } = null!;
}