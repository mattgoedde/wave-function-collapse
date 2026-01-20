namespace PerlinNoise.Core.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using PerlinNoise.Core.Abstractions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPerlinNoiseCore(this IServiceCollection services)
    {
        // DI registrations will be added in subsequent phases
        return services;
    }
}
