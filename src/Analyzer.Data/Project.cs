using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Analyzer.Data;

[Index(nameof(DevOpsId), IsUnique = true)]
public class Project
{
    public Guid Id { get; set; }

    public required DevOpsGuid DevOpsId { get; set; }

    public required string Name { get; set; }

    public List<Repository> Repositories { get; } = new();

    public required string Organisation { get; set; }
}