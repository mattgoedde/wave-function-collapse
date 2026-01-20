namespace PerlinNoise.Core;

public static class TileColor
{
    public const string Water = "#0000FF";
    public const string Beach = "#FFFF00";
    public const string Grass = "#00AA00";
    public const string Mountain = "#808080";

    public static string GetColor(TileType tileType) => tileType switch
    {
        TileType.Water => Water,
        TileType.Beach => Beach,
        TileType.Grass => Grass,
        TileType.Mountain => Mountain,
        _ => throw new ArgumentException($"Unknown tile type: {tileType}")
    };
}
