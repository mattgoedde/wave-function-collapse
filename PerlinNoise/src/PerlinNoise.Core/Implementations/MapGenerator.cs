namespace PerlinNoise.Core.Implementations;

using Abstractions;

public class MapGenerator : IMapGenerator
{
    private readonly INoiseGenerator _noiseGenerator;
    private readonly INoiseToTileMapper _tileMapper;

    public MapGenerator(INoiseGenerator noiseGenerator, INoiseToTileMapper tileMapper)
    {
        _noiseGenerator = noiseGenerator ?? throw new ArgumentNullException(nameof(noiseGenerator));
        _tileMapper = tileMapper ?? throw new ArgumentNullException(nameof(tileMapper));
    }

    public Tile[,] GenerateMap(int width, int height, int seed)
    {
        if (width <= 0)
            throw new ArgumentException("Width must be positive", nameof(width));
        if (height <= 0)
            throw new ArgumentException("Height must be positive", nameof(height));

        // Generate 2D noise field
        float[,] noiseField = _noiseGenerator.GenerateNoise(width, height, seed);

        // Create tile map
        Tile[,] tileMap = new Tile[height, width];

        // Convert each noise value to a tile
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float noiseValue = noiseField[y, x];
                TileType tileType = _tileMapper.MapNoiseToTile(noiseValue);
                tileMap[y, x] = new Tile(tileType);
            }
        }

        return tileMap;
    }
}
