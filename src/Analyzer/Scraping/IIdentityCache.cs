using System;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace Analyzer.Scraping;

public interface IIdentityCache
{
    ValueTask<Guid> GetIdentityIdAsync(GitUserDate userDate);
    
    ValueTask<Guid> GetIdentityIdAsync(IdentityRef idRef);
}