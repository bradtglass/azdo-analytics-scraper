using Vogen;

[assembly:
    VogenDefaults(conversions: Conversions.SystemTextJson | Conversions.NewtonsoftJson |
                               Conversions.EfCoreValueConverter)]