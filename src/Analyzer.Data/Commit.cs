using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Analyzer.Data;

[Index(nameof(Sha), IsUnique = true)]
public class Commit
{
    [MaxLength(20)] public required byte[] Sha { get; set; }

    public required Repository Repository { get; set; }

    public required Identity Commiter { get; set; }

    public required Identity Author { get; set; }

    public required DateTimeOffset CommitTimestamp { get; set; }

    public required DateTimeOffset AuthorTimestamp { get; set; }

    public required string Message { get; set; }
}