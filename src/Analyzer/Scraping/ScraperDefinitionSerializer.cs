using System;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Analyzer.Scraping;

public class ScraperDefinitionSerializer : IScraperDefinitionInterpreter
{
    public async ValueTask<IScraperDefinition> InterpretAsync(Stream stream, CancellationToken ct)
    {
        Type targetType;
        using (var reader = new StreamReader(stream, leaveOpen: true))
        {
            var line = await reader.ReadLineAsync(ct);

            if (string.IsNullOrEmpty(line))
                throw new DataException("Missing scraper definition type information");

            targetType = Type.GetType(line) ?? throw new DataException($"Failed to get type: {line}");
        }

        var definition = await JsonSerializer.DeserializeAsync(stream, targetType, cancellationToken: ct) ??
                         throw new InvalidDataException($"Failed to deserialize message into {targetType}");

        return (IScraperDefinition)definition;
    }
}