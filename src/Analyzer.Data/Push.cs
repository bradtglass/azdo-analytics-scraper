using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Analyzer.Data;

[Index(nameof(DevOpsId), IsUnique = true)]
public class Push
{
    public required Guid Id { get; set; }

    public required Identity Identity { get; set; }

    public required DevOpsIntId DevOpsId { get; set; }

    public required DateTimeOffset Timestamp { get; set; }

    public required List<Commit> Commits { get; set; }
}