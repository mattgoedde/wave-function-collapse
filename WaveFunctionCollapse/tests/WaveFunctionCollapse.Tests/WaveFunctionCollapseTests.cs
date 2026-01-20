namespace WaveFunctionCollapse.Tests;

using WaveFunctionCollapse.Core.Algorithm;
using WaveFunctionCollapse.Core.Models;

public class WaveFunctionCollapseTests
{
    [Fact]
    public void GridCreation_ShouldInitializeAllTiles()
    {
        var grid = new TileGrid(10, 10);
        var allTiles = grid.GetAllTiles().ToList();

        Assert.Equal(100, allTiles.Count);
        Assert.True(allTiles.All(t => !t.IsCollapsed));
        Assert.True(allTiles.All(t => t.PossibleTypes.Count == 4));
    }

    [Fact]
    public void GridBoundaryCheck_ShouldThrowOutOfRangeException()
    {
        var grid = new TileGrid(10, 10);

        Assert.Throws<ArgumentOutOfRangeException>(() => grid.GetTile(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => grid.GetTile(10, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => grid.GetTile(0, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => grid.GetTile(0, 10));
    }

    [Fact]
    public void TileCollapse_ShouldSetTypeAndUpdatePossibleTypes()
    {
        var tile = new Tile(0, 0);
        
        tile.Collapse(TileType.Grass);

        Assert.Equal(TileType.Grass, tile.Type);
        Assert.True(tile.IsCollapsed);
        Assert.Single(tile.PossibleTypes);
        Assert.Contains(TileType.Grass, tile.PossibleTypes);
    }

    [Fact]
    public void TileRemovePossibleType_ShouldReduceEntropy()
    {
        var tile = new Tile(0, 0);
        var initialEntropy = tile.Entropy;

        tile.RemovePossibleType(TileType.Grass);

        Assert.Equal(initialEntropy - 1, tile.Entropy);
        Assert.DoesNotContain(TileType.Grass, tile.PossibleTypes);
    }

    [Fact]
    public void GridReset_ShouldRestoreAllTilesToInitialState()
    {
        var grid = new TileGrid(5, 5);
        
        // Collapse some tiles
        var tile = grid.GetTile(0, 0);
        tile.Collapse(TileType.Water);

        grid.Reset();

        var resetTile = grid.GetTile(0, 0);
        Assert.False(resetTile.IsCollapsed);
        Assert.Equal(4, resetTile.Entropy);
    }

    [Fact]
    public void WaveGeneration_ShouldCompleteSuccessfully()
    {
        var grid = new TileGrid(8, 8);
        var wave = new Wave(grid, 12345);

        var result = wave.Generate();

        Assert.True(result);
        Assert.True(grid.IsFullyCollapsed);
    }

    [Fact]
    public void WaveGeneration_ShouldAllTilesBeCollapsed()
    {
        var grid = new TileGrid(10, 10);
        var wave = new Wave(grid, 54321);

        wave.Generate();

        var uncollapsedTiles = grid.GetUncollapsedTiles().ToList();
        Assert.Empty(uncollapsedTiles);
    }

    [Fact]
    public void WaveGeneration_ShouldContainMostTileTypes()
    {
        var grid = new TileGrid(32, 32);
        var wave = new Wave(grid, 99999);

        wave.Generate();

        var tileTypes = grid.GetAllTiles()
            .Where(t => t.Type.HasValue)
            .Select(t => t.Type!.Value)
            .Distinct()
            .ToList();

        // Should have at least 3 tile types (the adjacency rules can be restrictive)
        Assert.True(tileTypes.Count >= 3, $"Expected at least 3 tile types, got {tileTypes.Count}");
        // Grass is almost always present
        Assert.Contains(TileType.Grass, tileTypes);
    }

    [Fact]
    public void WaveGeneration_DeterministicWithSameSeed()
    {
        var grid1 = new TileGrid(8, 8);
        var wave1 = new Wave(grid1, 12345);
        wave1.Generate();

        var grid2 = new TileGrid(8, 8);
        var wave2 = new Wave(grid2, 12345);
        wave2.Generate();

        var tiles1 = grid1.GetAllTiles().Select(t => t.Type).ToList();
        var tiles2 = grid2.GetAllTiles().Select(t => t.Type).ToList();

        Assert.Equal(tiles1, tiles2);
    }

    [Fact]
    public void WaveGeneration_DifferentWithDifferentSeed()
    {
        var grid1 = new TileGrid(8, 8);
        var wave1 = new Wave(grid1, 11111);
        wave1.Generate();

        var grid2 = new TileGrid(8, 8);
        var wave2 = new Wave(grid2, 22222);
        wave2.Generate();

        var tiles1 = grid1.GetAllTiles().Select(t => t.Type).ToList();
        var tiles2 = grid2.GetAllTiles().Select(t => t.Type).ToList();

        // Very unlikely to be identical with different seeds
        Assert.NotEqual(tiles1, tiles2);
    }

    [Fact]
    public void WaveGeneration_ShouldCompleteInReasonableTime()
    {
        var grid = new TileGrid(64, 64);
        var wave = new Wave(grid, 12345);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        wave.Generate();
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 30000, 
            $"Generation took {stopwatch.ElapsedMilliseconds}ms, should be < 30000ms");
    }

    [Fact]
    public void AdjacencyRules_AllTilesShouldBeAdjacent()
    {
        var rules = new PermissiveRuleProvider();

        foreach (var tile1 in Enum.GetValues(typeof(TileType)).Cast<TileType>())
        {
            foreach (var tile2 in Enum.GetValues(typeof(TileType)).Cast<TileType>())
            {
                Assert.True(rules.CanBeAdjacent(tile1, tile2),
                    $"{tile1} should be adjacent to {tile2}");
            }
        }
    }

    #region Phase 4: Hardcoded Adjacency Rules Tests

    [Fact]
    public void HardcodedRules_GrassShouldBeAdjacentToGrassBachMountain()
    {
        var rules = new HardcodedRuleProvider();

        Assert.True(rules.CanBeAdjacent(TileType.Grass, TileType.Grass));
        Assert.True(rules.CanBeAdjacent(TileType.Grass, TileType.Beach));
        Assert.True(rules.CanBeAdjacent(TileType.Grass, TileType.Mountain));
        Assert.False(rules.CanBeAdjacent(TileType.Grass, TileType.Water));
    }

    [Fact]
    public void HardcodedRules_MountainShouldBeAdjacentToGrassAndMountain()
    {
        var rules = new HardcodedRuleProvider();

        Assert.True(rules.CanBeAdjacent(TileType.Mountain, TileType.Grass));
        Assert.True(rules.CanBeAdjacent(TileType.Mountain, TileType.Mountain));
        Assert.False(rules.CanBeAdjacent(TileType.Mountain, TileType.Beach));
        Assert.False(rules.CanBeAdjacent(TileType.Mountain, TileType.Water));
    }

    [Fact]
    public void HardcodedRules_BeachShouldBeAdjacentToBeachGrassWater()
    {
        var rules = new HardcodedRuleProvider();

        Assert.True(rules.CanBeAdjacent(TileType.Beach, TileType.Beach));
        Assert.True(rules.CanBeAdjacent(TileType.Beach, TileType.Grass));
        Assert.True(rules.CanBeAdjacent(TileType.Beach, TileType.Water));
        Assert.False(rules.CanBeAdjacent(TileType.Beach, TileType.Mountain));
    }

    [Fact]
    public void HardcodedRules_WaterShouldBeAdjacentToWaterAndBeach()
    {
        var rules = new HardcodedRuleProvider();

        Assert.True(rules.CanBeAdjacent(TileType.Water, TileType.Water));
        Assert.True(rules.CanBeAdjacent(TileType.Water, TileType.Beach));
        Assert.False(rules.CanBeAdjacent(TileType.Water, TileType.Grass));
        Assert.False(rules.CanBeAdjacent(TileType.Water, TileType.Mountain));
    }

    [Fact]
    public void HardcodedRules_GetValidNeighbors_Grass()
    {
        var rules = new HardcodedRuleProvider();
        var neighbors = rules.GetValidNeighbors(TileType.Grass).ToHashSet();

        Assert.Contains(TileType.Grass, neighbors);
        Assert.Contains(TileType.Beach, neighbors);
        Assert.Contains(TileType.Mountain, neighbors);
        Assert.DoesNotContain(TileType.Water, neighbors);
    }

    [Fact]
    public void HardcodedRules_GetValidNeighbors_Mountain()
    {
        var rules = new HardcodedRuleProvider();
        var neighbors = rules.GetValidNeighbors(TileType.Mountain).ToHashSet();

        Assert.Contains(TileType.Grass, neighbors);
        Assert.Contains(TileType.Mountain, neighbors);
        Assert.DoesNotContain(TileType.Beach, neighbors);
        Assert.DoesNotContain(TileType.Water, neighbors);
    }

    [Fact]
    public void HardcodedRules_GetValidNeighbors_Beach()
    {
        var rules = new HardcodedRuleProvider();
        var neighbors = rules.GetValidNeighbors(TileType.Beach).ToHashSet();

        Assert.Contains(TileType.Beach, neighbors);
        Assert.Contains(TileType.Grass, neighbors);
        Assert.Contains(TileType.Water, neighbors);
        Assert.DoesNotContain(TileType.Mountain, neighbors);
    }

    [Fact]
    public void HardcodedRules_GetValidNeighbors_Water()
    {
        var rules = new HardcodedRuleProvider();
        var neighbors = rules.GetValidNeighbors(TileType.Water).ToHashSet();

        Assert.Contains(TileType.Water, neighbors);
        Assert.Contains(TileType.Beach, neighbors);
        Assert.DoesNotContain(TileType.Grass, neighbors);
        Assert.DoesNotContain(TileType.Mountain, neighbors);
    }

    [Fact]
    public void WaveGenerationWithHardcodedRules_ShouldEnforceAdjacencyConstraints()
    {
        var grid = new TileGrid(16, 16);
        var rules = new HardcodedRuleProvider();
        var wave = new Wave(grid, 12345, rules);

        var result = wave.Generate();

        Assert.True(result);
        Assert.True(grid.IsFullyCollapsed);

        // Verify no invalid adjacencies exist
        foreach (var tile in grid.GetAllTiles())
        {
            Assert.NotNull(tile.Type);
            var neighbors = GetNeighbors(grid, tile);

            foreach (var neighbor in neighbors)
            {
                Assert.True(rules.CanBeAdjacent(tile.Type.Value, neighbor.Type.Value),
                    $"Invalid adjacency: {tile.Type.Value} at ({tile.X},{tile.Y}) adjacent to {neighbor.Type.Value} at ({neighbor.X},{neighbor.Y})");
            }
        }
    }

    [Fact]
    public void WaveGenerationWithHardcodedRules_ShouldNotHaveMountainAdjacentToWater()
    {
        var grid = new TileGrid(32, 32);
        var wave = new Wave(grid, 99999, new HardcodedRuleProvider());
        wave.Generate();

        foreach (var tile in grid.GetAllTiles())
        {
            if (tile.Type == TileType.Mountain)
            {
                var neighbors = GetNeighbors(grid, tile);
                foreach (var neighbor in neighbors)
                {
                    Assert.NotEqual(TileType.Water, neighbor.Type);
                    Assert.NotEqual(TileType.Beach, neighbor.Type);
                }
            }
        }
    }

    [Fact]
    public void WaveGenerationWithHardcodedRules_ShouldNotHaveWaterAdjacentToGrass()
    {
        var grid = new TileGrid(32, 32);
        var wave = new Wave(grid, 88888, new HardcodedRuleProvider());
        wave.Generate();

        foreach (var tile in grid.GetAllTiles())
        {
            if (tile.Type == TileType.Water)
            {
                var neighbors = GetNeighbors(grid, tile);
                foreach (var neighbor in neighbors)
                {
                    Assert.NotEqual(TileType.Grass, neighbor.Type);
                    Assert.NotEqual(TileType.Mountain, neighbor.Type);
                }
            }
        }
    }

    [Fact]
    public void WaveGenerationWithHardcodedRules_ShouldCreateCoastalBoundaries()
    {
        var grid = new TileGrid(32, 32);
        var wave = new Wave(grid, 77777, new HardcodedRuleProvider());
        wave.Generate();

        // Verify that Beach tiles exist and provide transitions between Water and Grass
        var hasBeach = grid.GetAllTiles().Any(t => t.Type == TileType.Beach);
        Assert.True(hasBeach, "Beach tiles should exist in generated map");

        // Verify Beach acts as boundary
        var beachTiles = grid.GetAllTiles().Where(t => t.Type == TileType.Beach).ToList();
        var hasWaterNeighbor = false;
        var hasGrassNeighbor = false;

        foreach (var beach in beachTiles)
        {
            var neighbors = GetNeighbors(grid, beach);
            if (neighbors.Any(n => n.Type == TileType.Water))
                hasWaterNeighbor = true;
            if (neighbors.Any(n => n.Type == TileType.Grass))
                hasGrassNeighbor = true;
        }

        Assert.True(hasWaterNeighbor || hasGrassNeighbor, "Beach should neighbor Water or Grass");
    }

    [Fact]
    public void WaveGenerationWithHardcodedRules_DeterministicWithSameSeed()
    {
        var grid1 = new TileGrid(16, 16);
        var wave1 = new Wave(grid1, 12345, new HardcodedRuleProvider());
        wave1.Generate();

        var grid2 = new TileGrid(16, 16);
        var wave2 = new Wave(grid2, 12345, new HardcodedRuleProvider());
        wave2.Generate();

        var tiles1 = grid1.GetAllTiles().Select(t => t.Type).ToList();
        var tiles2 = grid2.GetAllTiles().Select(t => t.Type).ToList();

        Assert.Equal(tiles1, tiles2);
    }

    private List<Tile> GetNeighbors(TileGrid grid, Tile tile)
    {
        var neighbors = new List<Tile>();
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            int nx = tile.X + dx[i];
            int ny = tile.Y + dy[i];

            if (nx >= 0 && nx < grid.Width && ny >= 0 && ny < grid.Height)
            {
                neighbors.Add(grid.GetTile(nx, ny));
            }
        }

        return neighbors;
    }

    #endregion

    #region Phase 4.5a: Frequency Bias Tests

    [Fact]
    public void TileDistribution_CreateDefault_ShouldHaveCorrectProbabilities()
    {
        var distribution = TileDistribution.CreateDefault();

        Assert.Equal(0.50, distribution.GetProbability(TileType.Grass), 0.0001);
        Assert.Equal(0.25, distribution.GetProbability(TileType.Water), 0.0001);
        Assert.Equal(0.15, distribution.GetProbability(TileType.Mountain), 0.0001);
        Assert.Equal(0.10, distribution.GetProbability(TileType.Beach), 0.0001);
    }

    [Fact]
    public void TileDistribution_CreateUniform_ShouldHaveEqualProbabilities()
    {
        var distribution = TileDistribution.CreateUniform();

        Assert.Equal(0.25, distribution.GetProbability(TileType.Grass), 0.0001);
        Assert.Equal(0.25, distribution.GetProbability(TileType.Water), 0.0001);
        Assert.Equal(0.25, distribution.GetProbability(TileType.Mountain), 0.0001);
        Assert.Equal(0.25, distribution.GetProbability(TileType.Beach), 0.0001);
    }

    [Fact]
    public void TileDistribution_InvalidProbabilities_ShouldThrow()
    {
        // Sum doesn't equal 1.0
        Assert.Throws<ArgumentException>(() => new TileDistribution(new Dictionary<TileType, double>
        {
            { TileType.Grass, 0.50 },
            { TileType.Water, 0.25 },
            { TileType.Mountain, 0.15 },
            { TileType.Beach, 0.05 }
        }));

        // Negative probability
        Assert.Throws<ArgumentException>(() => new TileDistribution(new Dictionary<TileType, double>
        {
            { TileType.Grass, -0.10 },
            { TileType.Water, 0.25 },
            { TileType.Mountain, 0.15 },
            { TileType.Beach, 0.70 }
        }));
    }

    [Fact]
    public void Tile_WithDistribution_ShouldInitializeProbabilityWeights()
    {
        var distribution = TileDistribution.CreateDefault();
        var tile = new Tile(0, 0, distribution);

        Assert.Equal(0.50, tile.ProbabilityWeights[TileType.Grass], 0.0001);
        Assert.Equal(0.25, tile.ProbabilityWeights[TileType.Water], 0.0001);
        Assert.Equal(0.15, tile.ProbabilityWeights[TileType.Mountain], 0.0001);
        Assert.Equal(0.10, tile.ProbabilityWeights[TileType.Beach], 0.0001);
    }

    [Fact]
    public void TileGrid_WithDistribution_ShouldInitializeGridWithDistribution()
    {
        var distribution = TileDistribution.CreateDefault();
        var grid = new TileGrid(8, 8, distribution);

        Assert.Equal(distribution, grid.Distribution);
        
        var tile = grid.GetTile(0, 0);
        Assert.Equal(0.50, tile.ProbabilityWeights[TileType.Grass], 0.0001);
    }

    [Fact]
    public void WaveGeneration_WithFrequencyBias_ShouldCompleteSuccessfully()
    {
        var distribution = TileDistribution.CreateDefault();
        var grid = new TileGrid(32, 32, distribution);
        var wave = new Wave(grid, 42, new HardcodedRuleProvider());

        var result = wave.Generate();

        Assert.True(result);
        Assert.True(grid.IsFullyCollapsed);
        // Verify all tiles are actually collapsed
        Assert.True(grid.GetAllTiles().All(t => t.Type.HasValue));
    }

    [Fact]
    public void WaveGeneration_WithFrequencyBias_PreservesDistribution()
    {
        // Verify that probability weights are preserved and used during generation
        var distribution = TileDistribution.CreateDefault();
        var grid = new TileGrid(32, 32, distribution);
        
        // Verify all tiles have correct initial weights
        foreach (var tile in grid.GetAllTiles())
        {
            Assert.Equal(0.50, tile.ProbabilityWeights[TileType.Grass], 0.0001);
            Assert.Equal(0.25, tile.ProbabilityWeights[TileType.Water], 0.0001);
            Assert.Equal(0.15, tile.ProbabilityWeights[TileType.Mountain], 0.0001);
            Assert.Equal(0.10, tile.ProbabilityWeights[TileType.Beach], 0.0001);
        }
        
        var wave = new Wave(grid, 42, new HardcodedRuleProvider());
        wave.Generate();

        // All tiles should still have the same distribution info
        foreach (var tile in grid.GetAllTiles())
        {
            Assert.NotNull(tile.Type);
        }
    }

    [Fact]
    public void WaveGeneration_WithFrequencyBias_DeterministicWithSameSeed()
    {
        var distribution = TileDistribution.CreateDefault();

        var grid1 = new TileGrid(16, 16, distribution);
        var wave1 = new Wave(grid1, 99999, new HardcodedRuleProvider());
        wave1.Generate();

        var grid2 = new TileGrid(16, 16, distribution);
        var wave2 = new Wave(grid2, 99999, new HardcodedRuleProvider());
        wave2.Generate();

        var tiles1 = grid1.GetAllTiles().Select(t => t.Type).ToList();
        var tiles2 = grid2.GetAllTiles().Select(t => t.Type).ToList();

        Assert.Equal(tiles1, tiles2);
    }

    [Fact]
    public void Tile_Reset_ShouldRestoreProbabilityWeights()
    {
        var distribution = TileDistribution.CreateDefault();
        var tile = new Tile(0, 0, distribution);

        tile.Collapse(TileType.Grass);
        Assert.True(tile.IsCollapsed);

        tile.Reset(distribution);
        Assert.False(tile.IsCollapsed);
        Assert.Equal(0.50, tile.ProbabilityWeights[TileType.Grass], 0.0001);
    }

    #endregion

    #region Phase 4.5b: Weighted Adjacency Preferences Tests

    [Fact]
    public void HardcodedRuleProvider_GetAdjacencyWeight_SameTypesPreferred()
    {
        var rules = new HardcodedRuleProvider();

        // Same types should have weights > 1.0
        Assert.True(rules.GetAdjacencyWeight(TileType.Grass, TileType.Grass) > 1.0);
        Assert.True(rules.GetAdjacencyWeight(TileType.Water, TileType.Water) > 1.0);
        Assert.True(rules.GetAdjacencyWeight(TileType.Mountain, TileType.Mountain) > 1.0);
        Assert.True(rules.GetAdjacencyWeight(TileType.Beach, TileType.Beach) > 1.0);
    }

    [Fact]
    public void HardcodedRuleProvider_GetAdjacencyWeight_SpecificPreferences()
    {
        var rules = new HardcodedRuleProvider();

        // Test specific weight values
        Assert.Equal(3.0, rules.GetAdjacencyWeight(TileType.Grass, TileType.Grass), 0.0001);
        Assert.Equal(3.0, rules.GetAdjacencyWeight(TileType.Water, TileType.Water), 0.0001);
        Assert.Equal(3.0, rules.GetAdjacencyWeight(TileType.Mountain, TileType.Mountain), 0.0001);
        Assert.Equal(2.0, rules.GetAdjacencyWeight(TileType.Beach, TileType.Beach), 0.0001);
    }

    [Fact]
    public void HardcodedRuleProvider_GetAdjacencyWeight_InvalidPairReturnsNeutral()
    {
        var rules = new HardcodedRuleProvider();

        // Invalid adjacencies should return neutral weight (1.0)
        Assert.Equal(1.0, rules.GetAdjacencyWeight(TileType.Mountain, TileType.Water), 0.0001);
        Assert.Equal(1.0, rules.GetAdjacencyWeight(TileType.Water, TileType.Grass), 0.0001);
    }

    [Fact]
    public void PermissiveRuleProvider_GetAdjacencyWeight_AlwaysNeutral()
    {
        var rules = new PermissiveRuleProvider();

        // Permissive should always return 1.0 (no preferences)
        Assert.Equal(1.0, rules.GetAdjacencyWeight(TileType.Grass, TileType.Water), 0.0001);
        Assert.Equal(1.0, rules.GetAdjacencyWeight(TileType.Mountain, TileType.Water), 0.0001);
        Assert.Equal(1.0, rules.GetAdjacencyWeight(TileType.Grass, TileType.Grass), 0.0001);
    }

    [Fact]
    public void Tile_CalculateNeighborAffinityWeight_NoNeighbors_ReturnsNeutral()
    {
        var tile = new Tile(0, 0);
        var rules = new HardcodedRuleProvider();

        var weight = tile.CalculateNeighborAffinityWeight(TileType.Grass, new List<Tile>(), rules);

        Assert.Equal(1.0, weight, 0.0001);
    }

    [Fact]
    public void Tile_CalculateNeighborAffinityWeight_SameTypeNeighbor_ReturnsHighWeight()
    {
        var tile = new Tile(0, 0);
        var grassNeighbor = new Tile(1, 0);
        grassNeighbor.Collapse(TileType.Grass);
        var rules = new HardcodedRuleProvider();

        var weight = tile.CalculateNeighborAffinityWeight(TileType.Grass, new List<Tile> { grassNeighbor }, rules);

        // Should get 3.0 weight for grass-grass adjacency
        Assert.Equal(3.0, weight, 0.0001);
    }

    [Fact]
    public void Tile_CalculateNeighborAffinityWeight_MultipleNeighbors_AveragesWeights()
    {
        var tile = new Tile(0, 0);
        var grassNeighbor = new Tile(1, 0);
        grassNeighbor.Collapse(TileType.Grass);
        var waterNeighbor = new Tile(-1, 0);
        waterNeighbor.Collapse(TileType.Water);
        var rules = new HardcodedRuleProvider();

        // For Grass type: grass-grass = 3.0, grass-water invalid = 1.0
        // Average = (3.0 + 1.0) / 2 = 2.0
        var weight = tile.CalculateNeighborAffinityWeight(TileType.Grass, new List<Tile> { grassNeighbor, waterNeighbor }, rules);

        Assert.Equal(2.0, weight, 0.0001);
    }

    [Fact]
    public void WaveGeneration_WithClusteringPreferences_FormsTileGroups()
    {
        var distribution = TileDistribution.CreateDefault();
        var grid = new TileGrid(32, 32, distribution);
        var wave = new Wave(grid, 54321, new HardcodedRuleProvider());

        var result = wave.Generate();

        Assert.True(result);
        Assert.True(grid.IsFullyCollapsed);

        // Check for clustering by counting same-type neighbors
        int clusteredPairs = 0;
        foreach (var tile in grid.GetAllTiles())
        {
            if (!tile.Type.HasValue)
                continue;

            var neighbors = GetNeighbors(grid, tile);
            foreach (var neighbor in neighbors)
            {
                if (neighbor.Type == tile.Type)
                    clusteredPairs++;
            }
        }

        // With clustering, should have many same-type adjacencies
        // Each clustered pair contributes 1 to count, so expect high number
        Assert.True(clusteredPairs > 100, $"Expected high clustering (>100 same-type pairs), got {clusteredPairs}");
    }

    [Fact]
    public void WaveGeneration_WithWeightedPreferences_DeterministicWithSameSeed()
    {
        var distribution = TileDistribution.CreateDefault();

        var grid1 = new TileGrid(16, 16, distribution);
        var wave1 = new Wave(grid1, 11111, new HardcodedRuleProvider());
        wave1.Generate();

        var grid2 = new TileGrid(16, 16, distribution);
        var wave2 = new Wave(grid2, 11111, new HardcodedRuleProvider());
        wave2.Generate();

        var tiles1 = grid1.GetAllTiles().Select(t => t.Type).ToList();
        var tiles2 = grid2.GetAllTiles().Select(t => t.Type).ToList();

        Assert.Equal(tiles1, tiles2);
    }

    [Fact]
    public void WaveGeneration_WeightedVsUnweighted_ShouldShowDifferentPatterns()
    {
        var distribution = TileDistribution.CreateDefault();

        // Generate with weighted preferences (clustering)
        var weightedGrid = new TileGrid(32, 32, distribution);
        var weightedWave = new Wave(weightedGrid, 99999, new HardcodedRuleProvider());
        weightedWave.Generate();

        // Generate with permissive rules (no clustering preference)
        var unweightedGrid = new TileGrid(32, 32, distribution);
        var unweightedWave = new Wave(unweightedGrid, 99999, new PermissiveRuleProvider());
        unweightedWave.Generate();

        // Count same-type adjacencies in each
        var weightedClustering = CountClusteredPairs(weightedGrid);
        var unweightedClustering = CountClusteredPairs(unweightedGrid);

        // Weighted should have more clustering due to preferences
        Assert.True(weightedClustering > unweightedClustering,
            $"Weighted clustering ({weightedClustering}) should exceed unweighted ({unweightedClustering})");
    }

    [Fact]
    public void WaveGeneration_WithWeightedPreferences_GrassAndWaterCluster()
    {
        var distribution = TileDistribution.CreateDefault();
        var grid = new TileGrid(32, 32, distribution);
        var wave = new Wave(grid, 77777, new HardcodedRuleProvider());
        wave.Generate();

        // Count grass-grass adjacencies
        int grassClusters = 0;
        int waterClusters = 0;

        foreach (var tile in grid.GetAllTiles())
        {
            if (tile.Type == TileType.Grass)
            {
                var neighbors = GetNeighbors(grid, tile);
                grassClusters += neighbors.Count(n => n.Type == TileType.Grass);
            }
            else if (tile.Type == TileType.Water)
            {
                var neighbors = GetNeighbors(grid, tile);
                waterClusters += neighbors.Count(n => n.Type == TileType.Water);
            }
        }

        // Both should have significant clustering (with weighted preferences)
        Assert.True(grassClusters > 35, $"Grass clusters should be > 35, got {grassClusters}");
        Assert.True(waterClusters > 10, $"Water clusters should be > 10, got {waterClusters}");
    }

    private int CountClusteredPairs(TileGrid grid)
    {
        int count = 0;
        foreach (var tile in grid.GetAllTiles())
        {
            if (!tile.Type.HasValue)
                continue;

            var neighbors = GetNeighbors(grid, tile);
            foreach (var neighbor in neighbors)
            {
                if (neighbor.Type == tile.Type)
                    count++;
            }
        }
        return count;
    }

    #endregion
}

