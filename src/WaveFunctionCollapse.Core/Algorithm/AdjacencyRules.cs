namespace WaveFunctionCollapse.Core.Algorithm;

using WaveFunctionCollapse.Core.Models;

public class AdjacencyRules
{
    private readonly Dictionary<TileType, HashSet<TileType>> _adjacencyMap;

    public AdjacencyRules()
    {
        _adjacencyMap = new Dictionary<TileType, HashSet<TileType>>();
        InitializeDefaultRules();
    }

    private void InitializeDefaultRules()
    {
        foreach (var tileType in Enum.GetValues(typeof(TileType)).Cast<TileType>())
        {
            _adjacencyMap[tileType] = new HashSet<TileType> 
            { 
                TileType.Grass, 
                TileType.Water, 
                TileType.Mountain, 
                TileType.Beach 
            };
        }
    }

    public bool CanBeAdjacent(TileType tile1, TileType tile2)
    {
        return _adjacencyMap.TryGetValue(tile1, out var adjacentTypes) && adjacentTypes.Contains(tile2);
    }

    public IEnumerable<TileType> GetValidNeighbors(TileType tileType)
    {
        return _adjacencyMap.TryGetValue(tileType, out var neighbors) 
            ? neighbors 
            : Enumerable.Empty<TileType>();
    }
}
