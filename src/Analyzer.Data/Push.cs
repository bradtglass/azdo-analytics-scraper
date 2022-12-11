using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Analyzer.Data;

[Index(nameof(DevOpsId), IsUnique = false)]
public class Push : IHasDevOpsId<DevOpsIntId>
{
    public Push(DevOpsIntId devOpsId)
    {
        DevOpsId = devOpsId;
    }

    public Guid Id { get; set; }

    public Identity Identity { get; set; } = null!;
    
    public Guid IdentityId { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public List<Commit> Commits { get; } = new();

    public Repository Repository { get; set; } = null!;

    public DevOpsIntId DevOpsId { get; set; }
}