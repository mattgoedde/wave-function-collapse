namespace PerlinNoise.Core.Abstractions;

public interface INoiseToTileMapper
{
    TileType MapNoiseToTile(float noiseValue);
}
