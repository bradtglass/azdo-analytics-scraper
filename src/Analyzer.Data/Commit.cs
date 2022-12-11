using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Analyzer.Data;

[Index(nameof(Sha), IsUnique = true)]
public class Commit
{
    public Commit(byte[] sha, DateTimeOffset commitTimestamp, DateTimeOffset authorTimestamp, string message)
    {
        Sha = sha;
        CommitTimestamp = commitTimestamp;
        AuthorTimestamp = authorTimestamp;
        Message = message;
    }

    [Key] [MaxLength(20)] public byte[] Sha { get; set; }

    public Identity Commiter { get; set; } = null!;

    public Identity Author { get; set; } = null!;
    
    public Guid CommiterId { get; set; }

    public Guid AuthorId { get; set; }

    public DateTimeOffset CommitTimestamp { get; set; }

    public DateTimeOffset AuthorTimestamp { get; set; }

    public string Message { get; set; }

    public Push Push { get; set; } = null!;
    
    public PullRequest? MergingPullRequest { get; set; }

    public static byte[] ConvertSha(string value)
    {
        Debug.Assert(value.Length == 40);
        var span = value.AsSpan();
        var result = new byte[20];

        for (var i = 0; i < 20; i++)
        {
            var b = byte.Parse(span.Slice(i * 2, 2), NumberStyles.HexNumber);
            result[i] = b;
        }

        return result;
    }
    
    public static string ConvertSha(byte[] value)
    {
        Debug.Assert(value.Length == 20);
        return BitConverter.ToString(value).Replace("-", string.Empty);
    }
}