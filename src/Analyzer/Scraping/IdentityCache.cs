using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Analyzer.Client;
using Analyzer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace Analyzer.Scraping;

public class IdentityCache : IIdentityCache
{
    private readonly Dictionary<string, Guid> cachedIdentities = new();
    private readonly AnalyticsScraperClient client;

    private readonly DevOpsContext context;

    public IdentityCache(DevOpsContext context, AnalyticsScraperClient client)
    {
        this.context = context;
        this.client = client;
    }

    public async ValueTask<Guid> GetIdentityIdAsync(GitUserDate userDate) =>
        await GetOrAddIdentityAsync(userDate.Name, userDate.Email, null);

    public async ValueTask<Guid> GetIdentityIdAsync(IdentityRef idRef) =>
        await GetOrAddIdentityAsync(idRef.DisplayName, idRef.UniqueName, DevOpsGuid.From(Guid.Parse(idRef.Id)));

    private async ValueTask<Guid> GetOrAddIdentityAsync(string name, string email, DevOpsGuid? devOpsId)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);

        email = email.ToLower();
        if (cachedIdentities.TryGetValue(email, out var id))
            return id;

        var identity = await context.Identities.SingleOrDefaultAsync(i => i.UniqueName == email);

        if (identity is null)
        {
            if (!devOpsId.HasValue)
            {
                var doIdentity = await client.FindIdentityByEmailAsync(email);
                if (doIdentity is not null)
                    devOpsId = DevOpsGuid.From(doIdentity.Id);
            }

            identity = new Identity
            {
                DisplayName = name,
                UniqueName = email,
                DevOpsId = devOpsId
            };

            context.Identities.Add(identity);
            await context.SaveChangesAsync();
        }

        cachedIdentities.Add(email, identity.Id);
        return identity.Id;
    }
}