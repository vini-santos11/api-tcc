using System;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Application.AutoMapper;
public static class AutoMapperSetup
{
    public static void AddAutoMapperSetup(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddSingleton<IConfigurationProvider>(AutoMapperConfig.RegisterMappings());
        services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));
    }
}
