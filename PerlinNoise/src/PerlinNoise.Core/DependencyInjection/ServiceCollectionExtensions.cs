namespace PerlinNoise.Core.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using PerlinNoise.Core.Abstractions;
using PerlinNoise.Core.Implementations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPerlinNoiseCore(this IServiceCollection services)
    {
        services.AddSingleton<INoiseGenerator, PerlinNoiseGenerator>();
        services.AddSingleton<INoiseToTileMapper, NoiseToTileMapper>();
        services.AddSingleton<IMapGenerator, MapGenerator>();
        return services;
    }
}
