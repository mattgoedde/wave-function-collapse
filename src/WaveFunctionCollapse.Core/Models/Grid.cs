namespace WaveFunctionCollapse.Core.Models;

public class TileGrid
{
    private readonly Tile[,] _tiles;
    public int Width { get; }
    public int Height { get; }

    public TileGrid(int width, int height)
    {
        Width = width;
        Height = height;
        _tiles = new Tile[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                _tiles[x, y] = new Tile(x, y);
            }
        }
    }

    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            throw new ArgumentOutOfRangeException($"Coordinates ({x}, {y}) out of bounds");
        
        return _tiles[x, y];
    }

    public void SetTile(int x, int y, Tile tile)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            throw new ArgumentOutOfRangeException($"Coordinates ({x}, {y}) out of bounds");
        
        _tiles[x, y] = tile;
    }

    public IEnumerable<Tile> GetAllTiles()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                yield return _tiles[x, y];
            }
        }
    }

    public IEnumerable<Tile> GetUncollapsedTiles()
    {
        return GetAllTiles().Where(t => !t.IsCollapsed);
    }

    public bool IsFullyCollapsed => GetUncollapsedTiles().Count() == 0;

    public void Reset()
    {
        foreach (var tile in GetAllTiles())
        {
            tile.Reset();
        }
    }
}
