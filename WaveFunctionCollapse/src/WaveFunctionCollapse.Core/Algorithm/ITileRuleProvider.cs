using WaveFunctionCollapse.Core.Models;

namespace WaveFunctionCollapse.Core.Algorithm;

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

    /// <summary>
    /// Determines if two tile types can be adjacent with consideration of the neighbor's existing adjacencies.
    /// This enables context-aware rules like "Beach can only neighbor Water if Water already neighbors Grass".
    /// </summary>
    /// <param name="tile1">The first tile type</param>
    /// <param name="tile2">The second tile type (potential neighbor)</param>
    /// <param name="neighborAdjacentTypes">The tile types already adjacent to tile2</param>
    /// <returns>True if the tiles can be adjacent given the context</returns>
    bool CanBeAdjacentWithContext(TileType tile1, TileType tile2, IEnumerable<TileType> neighborAdjacentTypes);
}
