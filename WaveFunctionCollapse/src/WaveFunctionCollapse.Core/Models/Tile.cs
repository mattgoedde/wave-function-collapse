namespace WaveFunctionCollapse.Core.Models;

using WaveFunctionCollapse.Core.Algorithm;

public class Tile
{
    public int X { get; }
    public int Y { get; }
    public TileType? Type { get; set; }
    public HashSet<TileType> PossibleTypes { get; set; }
    public Dictionary<TileType, double> ProbabilityWeights { get; set; }

    public Tile(int x, int y, TileDistribution? distribution = null)
    {
        X = x;
        Y = y;
        Type = null;
        distribution ??= TileDistribution.CreateDefault();
        
        PossibleTypes = new HashSet<TileType> { TileType.Grass, TileType.Water, TileType.Mountain, TileType.Beach };
        
        // Initialize probability weights based on distribution
        ProbabilityWeights = new Dictionary<TileType, double>
        {
            { TileType.Grass, distribution.GetProbability(TileType.Grass) },
            { TileType.Water, distribution.GetProbability(TileType.Water) },
            { TileType.Mountain, distribution.GetProbability(TileType.Mountain) },
            { TileType.Beach, distribution.GetProbability(TileType.Beach) }
        };
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

    public void Reset(TileDistribution? distribution = null)
    {
        Type = null;
        distribution ??= TileDistribution.CreateDefault();
        
        PossibleTypes = new HashSet<TileType> { TileType.Grass, TileType.Water, TileType.Mountain, TileType.Beach };
        
        ProbabilityWeights = new Dictionary<TileType, double>
        {
            { TileType.Grass, distribution.GetProbability(TileType.Grass) },
            { TileType.Water, distribution.GetProbability(TileType.Water) },
            { TileType.Mountain, distribution.GetProbability(TileType.Mountain) },
            { TileType.Beach, distribution.GetProbability(TileType.Beach) }
        };
    }

    /// <summary>
    /// Calculates average adjacency weight based on collapsed neighbors.
    /// Returns a multiplier to apply to tile type preferences based on how well this type
    /// matches the types of already-collapsed neighbors.
    /// </summary>
    public double CalculateNeighborAffinityWeight(TileType candidateType, List<Tile> collapsedNeighbors, ITileRuleProvider ruleProvider)
    {
        if (collapsedNeighbors.Count == 0)
            return 1.0; // No neighbors, no affinity bonus

        double totalWeight = 0.0;
        foreach (var neighbor in collapsedNeighbors)
        {
            if (neighbor.Type.HasValue)
            {
                // Get weight for this candidate type adjacent to the neighbor's type
                totalWeight += ruleProvider.GetAdjacencyWeight(candidateType, neighbor.Type.Value);
            }
        }

        // Average the weights across all collapsed neighbors
        return totalWeight / collapsedNeighbors.Count;
    }
}
