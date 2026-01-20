namespace PerlinNoise.Core.Abstractions;

using PerlinNoise.Core;

public interface IMapGenerator
{
    Tile[,] GenerateMap(int width, int height, int seed);
}
