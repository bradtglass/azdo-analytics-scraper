using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index(nameof(DevOpsId), IsUnique = true)]
public class Project
{
    public required Guid Id { get; set; }

    public required DevOpsGuid DevOpsId { get; set; }

    public required string Name { get; set; }

    public required List<Repository> Repositories { get; set; }

    public required string Organisation { get; set; }
}