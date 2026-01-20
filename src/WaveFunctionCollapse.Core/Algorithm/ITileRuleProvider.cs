namespace WaveFunctionCollapse.Core.Algorithm;

using WaveFunctionCollapse.Core.Models;

/// <summary>
/// Defines the contract for tile adjacency rules.
/// This interface allows for flexible implementation of rules,
/// from hardcoded to configuration-based providers.
/// </summary>
public interface ITileRuleProvider
{
    /// <summary>
    /// Determines if two tile types can be adjacent to each other.
    /// </summary>
    bool CanBeAdjacent(TileType tile1, TileType tile2);

    /// <summary>
    /// Gets all valid tile types that can be adjacent to the given tile type.
    /// </summary>
    IEnumerable<TileType> GetValidNeighbors(TileType tileType);

    /// <summary>
    /// Gets the adjacency weight (preference multiplier) for a tile type given its neighbor's type.
    /// Weight > 1.0 means this tile type is preferred when adjacent to that neighbor.
    /// Weight = 1.0 means neutral (no preference).
    /// This enables clustering behavior (e.g., mountains prefer mountains).
    /// </summary>
    double GetAdjacencyWeight(TileType tileType, TileType neighborType);
}
