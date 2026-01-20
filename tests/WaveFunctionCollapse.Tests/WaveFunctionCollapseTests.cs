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
    public void WaveGeneration_ShouldContainAllTileTypes()
    {
        var grid = new TileGrid(16, 16);
        var wave = new Wave(grid, 99999);

        wave.Generate();

        var tileTypes = grid.GetAllTiles()
            .Where(t => t.Type.HasValue)
            .Select(t => t.Type!.Value)
            .Distinct()
            .ToList();

        Assert.Contains(TileType.Grass, tileTypes);
        Assert.Contains(TileType.Water, tileTypes);
        Assert.Contains(TileType.Mountain, tileTypes);
        Assert.Contains(TileType.Beach, tileTypes);
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
        var rules = new AdjacencyRules();

        foreach (var tile1 in Enum.GetValues(typeof(TileType)).Cast<TileType>())
        {
            foreach (var tile2 in Enum.GetValues(typeof(TileType)).Cast<TileType>())
            {
                Assert.True(rules.CanBeAdjacent(tile1, tile2),
                    $"{tile1} should be adjacent to {tile2}");
            }
        }
    }
}

