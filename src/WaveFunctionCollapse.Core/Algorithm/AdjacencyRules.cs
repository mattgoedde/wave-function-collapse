namespace WaveFunctionCollapse.Core.Algorithm;

using WaveFunctionCollapse.Core.Models;

/// <summary>
/// Hardcoded tile adjacency rules for Phase 4+ implementation.
/// Enforces realistic terrain constraints:
/// - Grass: Can be adjacent to [Grass, Beach, Mountain]
/// - Mountain: Can be adjacent to [Grass, Mountain] (no Beach or Water)
/// - Beach: Can be adjacent to [Beach, Grass, Water] (Water only if it neighbors Grass)
/// - Water: Can be adjacent to [Water, Beach]
/// 
/// Special Rules (Phase 5 enhancement):
/// - Beach→Water adjacency requires that Water already has a Grass neighbor to prevent isolated beach islands.
/// </summary>
public class HardcodedRuleProvider : ITileRuleProvider
{
    private readonly Dictionary<TileType, HashSet<TileType>> _adjacencyMap;
    private readonly Dictionary<(TileType, TileType), double> _adjacencyWeights;

    public HardcodedRuleProvider()
    {
        _adjacencyMap = new Dictionary<TileType, HashSet<TileType>>();
        _adjacencyWeights = new Dictionary<(TileType, TileType), double>();
        InitializeRules();
        InitializeWeights();
    }

    private void InitializeRules()
    {
        _adjacencyMap[TileType.Grass] = new HashSet<TileType> 
        { 
            TileType.Grass, 
            TileType.Beach, 
            TileType.Mountain 
        };

        _adjacencyMap[TileType.Mountain] = new HashSet<TileType> 
        { 
            TileType.Grass, 
            TileType.Mountain 
        };

        _adjacencyMap[TileType.Beach] = new HashSet<TileType> 
        { 
            TileType.Beach, 
            TileType.Grass, 
            TileType.Water 
        };

        _adjacencyMap[TileType.Water] = new HashSet<TileType> 
        { 
            TileType.Water, 
            TileType.Beach 
        };
    }

    private void InitializeWeights()
    {
        // Initialize all weights to neutral (1.0)
        foreach (var tile1 in Enum.GetValues(typeof(TileType)).Cast<TileType>())
        {
            foreach (var tile2 in Enum.GetValues(typeof(TileType)).Cast<TileType>())
            {
                _adjacencyWeights[(tile1, tile2)] = 1.0;
            }
        }

        // Same-type adjacencies are heavily preferred (clustering)
        _adjacencyWeights[(TileType.Grass, TileType.Grass)] = 3.0;
        _adjacencyWeights[(TileType.Water, TileType.Water)] = 3.0;
        _adjacencyWeights[(TileType.Mountain, TileType.Mountain)] = 3.0;
        _adjacencyWeights[(TileType.Beach, TileType.Beach)] = 2.0;

        // Type-specific preferences for terrain formation
        // Grass prefers adjacent to mountains (foothills) and beaches (transition)
        _adjacencyWeights[(TileType.Grass, TileType.Mountain)] = 1.3;
        _adjacencyWeights[(TileType.Grass, TileType.Beach)] = 1.2;

        // Mountain prefers grass around it (natural elevation boundaries)
        _adjacencyWeights[(TileType.Mountain, TileType.Grass)] = 1.2;

        // Beach prefers water and grass equally (natural coastline)
        _adjacencyWeights[(TileType.Beach, TileType.Water)] = 1.5;
        _adjacencyWeights[(TileType.Beach, TileType.Grass)] = 1.3;

        // Water prefers beach nearby (realistic coastlines)
        _adjacencyWeights[(TileType.Water, TileType.Beach)] = 1.5;
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

    public double GetAdjacencyWeight(TileType tileType, TileType neighborType)
    {
        return _adjacencyWeights.TryGetValue((tileType, neighborType), out var weight) 
            ? weight 
            : 1.0;
    }

    /// <summary>
    /// Context-aware adjacency check for Beach→Water constraint.
    /// Beach can only neighbor Water if the Water tile already neighbors Grass.
    /// This prevents isolated beach islands in the middle of water.
    /// </summary>
    public bool CanBeAdjacentWithContext(TileType tile1, TileType tile2, IEnumerable<TileType> neighborAdjacentTypes)
    {
        // Standard adjacency check first
        if (!CanBeAdjacent(tile1, tile2))
            return false;

        // Special rule: Beach (tile1) → Water (tile2) only if Water neighbors Grass
        if (tile1 == TileType.Beach && tile2 == TileType.Water)
        {
            return neighborAdjacentTypes.Contains(TileType.Grass);
        }

        // All other adjacencies follow standard rules
        return true;
    }
}

/// <summary>
/// Permissive rule provider for testing - allows all tiles to be adjacent.
/// </summary>
public class PermissiveRuleProvider : ITileRuleProvider
{
    public bool CanBeAdjacent(TileType tile1, TileType tile2)
    {
        return true;
    }

    public IEnumerable<TileType> GetValidNeighbors(TileType tileType)
    {
        return Enum.GetValues(typeof(TileType)).Cast<TileType>();
    }

    public double GetAdjacencyWeight(TileType tileType, TileType neighborType)
    {
        return 1.0; // No preferences in permissive mode
    }

    public bool CanBeAdjacentWithContext(TileType tile1, TileType tile2, IEnumerable<TileType> neighborAdjacentTypes)
    {
        return true; // Permissive mode allows anything
    }
}
