using System.Collections.Generic;
using System.Threading;
using Autofac;

namespace Analyzer.Scraping;

public class ScraperRunner : IScraperRunner
{
   private readonly IComponentContext componentContext;

   public ScraperRunner(IComponentContext componentContext)
   {
      this.componentContext = componentContext;
   }

   public IAsyncEnumerable<IScraperDefinition> RunAsync(IScraperDefinition definition, CancellationToken ct)
   {
      var definitionType = definition.GetType();
      var genericRunnerType = typeof(GenericScraperRunner<>).MakeGenericType(definitionType);

      var uncastRunner = componentContext.Resolve(genericRunnerType,
         new TypedParameter(definitionType, definition));
      var genericRunner = (GenericScraperRunnerBase)uncastRunner;
      
      return genericRunner.RunAsync(ct);
   }
}