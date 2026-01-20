namespace PerlinNoise.Core;

public class Tile
{
    public TileType Type { get; }
    public string Color { get; }

    public Tile(TileType type)
    {
        Type = type;
        Color = TileColor.GetColor(type);
    }
}
