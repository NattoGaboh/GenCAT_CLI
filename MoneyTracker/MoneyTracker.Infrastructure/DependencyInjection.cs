using Microsoft.Extensions.DependencyInjection;
using { { ProjectName} }.Infrastructure.Persistence;

namespace {{ProjectName }}.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        return services;
    }
}