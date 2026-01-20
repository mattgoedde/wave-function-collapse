namespace WaveFunctionCollapse.Core.Models;

/// <summary>
/// Defines the probability distribution for tile types in generated maps.
/// Allows customization of how frequently each tile type appears.
/// </summary>
public class TileDistribution
{
    private readonly Dictionary<TileType, double> _probabilities;

    /// <summary>
    /// Creates a custom tile distribution with specified probabilities.
    /// Probabilities should sum to 1.0.
    /// </summary>
    public TileDistribution(Dictionary<TileType, double> probabilities)
    {
        ValidateProbabilities(probabilities);
        _probabilities = new Dictionary<TileType, double>(probabilities);
    }

    /// <summary>
    /// Gets the probability for a specific tile type.
    /// </summary>
    public double GetProbability(TileType tileType)
    {
        return _probabilities.TryGetValue(tileType, out var probability) ? probability : 0.0;
    }

    /// <summary>
    /// Creates the default distribution: Grass 50%, Water 25%, Mountain 15%, Beach 10%.
    /// </summary>
    public static TileDistribution CreateDefault()
    {
        return new TileDistribution(new Dictionary<TileType, double>
        {
            { TileType.Grass, 0.50 },
            { TileType.Water, 0.25 },
            { TileType.Mountain, 0.15 },
            { TileType.Beach, 0.10 }
        });
    }

    /// <summary>
    /// Creates a uniform distribution where all tile types are equally likely.
    /// Used for testing and baseline comparison.
    /// </summary>
    public static TileDistribution CreateUniform()
    {
        return new TileDistribution(new Dictionary<TileType, double>
        {
            { TileType.Grass, 0.25 },
            { TileType.Water, 0.25 },
            { TileType.Mountain, 0.25 },
            { TileType.Beach, 0.25 }
        });
    }

    private static void ValidateProbabilities(Dictionary<TileType, double> probabilities)
    {
        if (probabilities == null || probabilities.Count != 4)
            throw new ArgumentException("TileDistribution must contain exactly 4 tile types");

        var sum = probabilities.Values.Sum();
        const double tolerance = 0.0001;
        if (Math.Abs(sum - 1.0) > tolerance)
            throw new ArgumentException($"Probabilities must sum to 1.0, but sum to {sum}");

        foreach (var kvp in probabilities)
        {
            if (kvp.Value < 0 || kvp.Value > 1)
                throw new ArgumentException($"Probability for {kvp.Key} must be between 0 and 1, got {kvp.Value}");
        }
    }
}
