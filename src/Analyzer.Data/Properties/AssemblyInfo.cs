using Vogen;

[assembly:
    VogenDefaults(conversions: Conversions.SystemTextJson | 
                               Conversions.EfCoreValueConverter)]