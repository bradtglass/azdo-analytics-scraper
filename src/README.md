# Azure DevOps Analytics

This project runs analytics scraping on top of the Azure DevOps git REST API.

## Data
Data is stored using EF Core, to add a new migration run the following command:
```
dotnet ef migrations add {-MigrationName-} -p .\src\Analyzer.Data\Analyzer.Data.csproj -s .\src\Analyzer.Data.Migrations\Analyzer.Data.Migrations.csproj
```

To apply migrations to the database:
```
dotnet ef database update -p .\src\Analyzer.Data\Analyzer.Data.csproj -s .\src\Analyzer.Data.Migrations\Analyzer.Data.Migrations.csproj
```