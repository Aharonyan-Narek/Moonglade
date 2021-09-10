﻿using Microsoft.Extensions.DependencyInjection;

namespace Moonglade.Syndication
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSyndication(this IServiceCollection services)
        {
            services.AddScoped<ISyndicationDataSource, SyndicationDataSource>();
            services.AddScoped<IOpmlWriter, StringOpmlWriter>();

            return services;
        }
    }
}
