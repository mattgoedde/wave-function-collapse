namespace PerlinNoise.Core.Implementations;

using Abstractions;

public class NoiseToTileMapper : INoiseToTileMapper
{
    public TileType MapNoiseToTile(float noiseValue)
    {
        if (noiseValue < 0.0f || noiseValue > 1.0f)
        {
            throw new ArgumentException($"Noise value must be between 0.0 and 1.0, but was {noiseValue}");
        }

        return noiseValue switch
        {
            >= 0.00f and < 0.25f => TileType.Water,
            >= 0.25f and < 0.50f => TileType.Beach,
            >= 0.50f and < 0.75f => TileType.Grass,
            >= 0.75f and <= 1.00f => TileType.Mountain,
            _ => throw new InvalidOperationException($"Unexpected noise value: {noiseValue}")
        };
    }
}
