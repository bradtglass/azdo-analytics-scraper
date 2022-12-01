using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Analyzer.Data;

[Index(nameof(DevOpsId), IsUnique = true)]
public class Push
{
    public Guid Id { get; set; }

    public required Identity Identity { get; set; }

    public required DevOpsIntId DevOpsId { get; set; }

    public required DateTimeOffset Timestamp { get; set; }

    public List<Commit> Commits { get; } = new();
    
    public required Repository Repository { get; set; }
}