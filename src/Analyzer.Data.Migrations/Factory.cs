using Autofac;
using Microsoft.EntityFrameworkCore.Design;

namespace Analyzer.Data.Migrations;

public class Factory : IDesignTimeDbContextFactory<DevOpsContext>
{
    public DevOpsContext CreateDbContext(string[] args) => Services.CreateContainer().Resolve<DevOpsContext>();
}