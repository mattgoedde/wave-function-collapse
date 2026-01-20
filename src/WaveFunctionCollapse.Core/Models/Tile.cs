namespace WaveFunctionCollapse.Core.Models;

public class Tile
{
    public int X { get; }
    public int Y { get; }
    public TileType? Type { get; set; }
    public HashSet<TileType> PossibleTypes { get; set; }

    public Tile(int x, int y)
    {
        X = x;
        Y = y;
        Type = null;
        PossibleTypes = new HashSet<TileType> { TileType.Grass, TileType.Water, TileType.Mountain, TileType.Beach };
    }

    public bool IsCollapsed => Type.HasValue;

    public int Entropy => PossibleTypes.Count;

    public void Collapse(TileType type)
    {
        Type = type;
        PossibleTypes.Clear();
        PossibleTypes.Add(type);
    }

    public void RemovePossibleType(TileType type)
    {
        PossibleTypes.Remove(type);
    }

    public void Reset()
    {
        Type = null;
        PossibleTypes = new HashSet<TileType> { TileType.Grass, TileType.Water, TileType.Mountain, TileType.Beach };
    }
}
