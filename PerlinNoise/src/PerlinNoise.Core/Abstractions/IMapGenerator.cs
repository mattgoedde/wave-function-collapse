namespace PerlinNoise.Core.Abstractions;

public interface IMapGenerator
{
    Tile[,] GenerateMap(int width, int height, int seed);
}
