using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Analyzer.Data;

public static class DevOpsContextExtensions
{
    public static async ValueTask<Identity?> FindDevOpsIdentityAsync(this DevOpsContext context)
    {
        const string tfsIdentityIdRaw = "00000002-0000-8888-8000-000000000000";
        var tfsIdentityId = DevOpsGuid.From(Guid.Parse(tfsIdentityIdRaw));

        return await context.Identities.FirstOrDefaultAsync(i => i.DevOpsId == tfsIdentityId);
    }
}