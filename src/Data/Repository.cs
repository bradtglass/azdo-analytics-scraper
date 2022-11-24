using System;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index(nameof(DevOpsId), IsUnique = true)]
public class Repository
{
    public required Guid Id { get; set; }

    public required DevOpsGuid DevOpsId { get; set; }

    public required string Name { get; set; }

    public required Project Project { get; set; }
}