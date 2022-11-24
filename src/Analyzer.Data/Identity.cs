using System;
using Microsoft.EntityFrameworkCore;

namespace Analyzer.Data;

[Index(nameof(DevOpsId), IsUnique = true)]
[Index(nameof(UniqueName), IsUnique = true)]
public class Identity
{
    public required Guid Id { get; set; }

    public DevOpsGuid? DevOpsId { get; set; }

    public required string DisplayName { get; set; }

    public required string UniqueName { get; set; }
}