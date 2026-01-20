namespace PerlinNoise.Core.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPerlinNoiseCore(this IServiceCollection services)
    {
        // DI registrations will be added in subsequent phases
        return services;
    }
}
