namespace WaveFunctionCollapse.Core.Algorithm;

using WaveFunctionCollapse.Core.Models;

public class Wave
{
    private readonly TileGrid _grid;
    private readonly ITileRuleProvider _ruleProvider;
    private readonly Random _random;
    private Stack<TileGrid> _backtrackStack;
    private bool _contradiction;

    public Wave(TileGrid grid, int seed, ITileRuleProvider? ruleProvider = null)
    {
        _grid = grid;
        _ruleProvider = ruleProvider ?? new HardcodedRuleProvider();
        _random = new Random(seed);
        _backtrackStack = new Stack<TileGrid>();
        _contradiction = false;
    }

    public bool Generate()
    {
        _backtrackStack.Clear();
        _contradiction = false;

        while (!_grid.IsFullyCollapsed && !_contradiction)
        {
            if (!ObserveAndCollapse())
            {
                _contradiction = true;
                break;
            }

            if (!PropagatConstraints())
            {
                if (!Backtrack())
                {
                    return false;
                }
            }
        }

        return !_contradiction && _grid.IsFullyCollapsed;
    }

    private bool ObserveAndCollapse()
    {
        var uncollapsedTiles = _grid.GetUncollapsedTiles().ToList();
        if (uncollapsedTiles.Count == 0)
            return true;

        var tileToCollapse = SelectTileByEntropy(uncollapsedTiles);
        if (tileToCollapse == null)
            return false;

        SaveState();

        var possibleType = SelectRandomType(tileToCollapse.PossibleTypes, tileToCollapse.ProbabilityWeights, tileToCollapse);
        tileToCollapse.Collapse(possibleType);

        return true;
    }

    private Tile? SelectTileByEntropy(List<Tile> uncollapsedTiles)
    {
        var minEntropy = uncollapsedTiles.Min(t => t.Entropy);
        if (minEntropy == 0)
            return null;

        var candidateTiles = uncollapsedTiles
            .Where(t => t.Entropy == minEntropy)
            .ToList();

        return candidateTiles.Count > 0 
            ? candidateTiles[_random.Next(candidateTiles.Count)] 
            : null;
    }

    private TileType SelectRandomType(HashSet<TileType> possibleTypes, Dictionary<TileType, double> probabilityWeights, Tile tileToCollapse)
    {
        // Get collapsed neighbors to calculate adjacency preferences
        var collapsedNeighbors = GetNeighbors(tileToCollapse).Where(n => n.IsCollapsed).ToList();

        // Filter to only possible types and calculate their relative weights
        var validWeights = possibleTypes
            .Where(t => probabilityWeights.ContainsKey(t))
            .ToDictionary(t => t, t => {
                // Start with frequency bias probability
                double baseWeight = probabilityWeights[t];
                
                // Apply adjacency preference multiplier based on collapsed neighbors
                double adjacencyMultiplier = tileToCollapse.CalculateNeighborAffinityWeight(t, collapsedNeighbors, _ruleProvider);
                
                // Combine: base probability weighted by adjacency preference
                return baseWeight * adjacencyMultiplier;
            });

        if (validWeights.Count == 0)
            return possibleTypes.First();

        var totalWeight = validWeights.Values.Sum();
        var randomValue = _random.NextDouble() * totalWeight;
        var cumulativeWeight = 0.0;

        foreach (var kvp in validWeights)
        {
            cumulativeWeight += kvp.Value;
            if (randomValue <= cumulativeWeight)
                return kvp.Key;
        }

        return validWeights.Keys.Last();
    }

    private bool PropagatConstraints()
    {
        var propagationQueue = new Queue<Tile>();

        var recentlyCollapsed = _grid.GetAllTiles()
            .Where(t => t.IsCollapsed)
            .ToList();

        foreach (var tile in recentlyCollapsed)
        {
            propagationQueue.Enqueue(tile);
        }

        while (propagationQueue.Count > 0)
        {
            var tile = propagationQueue.Dequeue();
            if (!tile.IsCollapsed)
                continue;

            var neighbors = GetNeighbors(tile);
            foreach (var neighbor in neighbors)
            {
                if (neighbor.IsCollapsed)
                    continue;

                var validTypes = GetValidTypesForNeighbor(tile.Type!.Value);
                var initialCount = neighbor.PossibleTypes.Count;

                neighbor.PossibleTypes.IntersectWith(validTypes);

                if (neighbor.PossibleTypes.Count == 0)
                    return false;

                if (neighbor.PossibleTypes.Count < initialCount)
                {
                    propagationQueue.Enqueue(neighbor);
                }
            }
        }

        return true;
    }

    private List<Tile> GetNeighbors(Tile tile)
    {
        var neighbors = new List<Tile>();
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            int nx = tile.X + dx[i];
            int ny = tile.Y + dy[i];

            if (nx >= 0 && nx < _grid.Width && ny >= 0 && ny < _grid.Height)
            {
                neighbors.Add(_grid.GetTile(nx, ny));
            }
        }

        return neighbors;
    }

    private HashSet<TileType> GetValidTypesForNeighbor(TileType collapsedType)
    {
        var validTypes = new HashSet<TileType>();
        foreach (var tileType in Enum.GetValues(typeof(TileType)).Cast<TileType>())
        {
            if (_ruleProvider.CanBeAdjacent(collapsedType, tileType))
            {
                validTypes.Add(tileType);
            }
        }
        return validTypes;
    }

    private void SaveState()
    {
        var stateCopy = new TileGrid(_grid.Width, _grid.Height, _grid.Distribution);
        foreach (var tile in _grid.GetAllTiles())
        {
            var tileCopy = new Tile(tile.X, tile.Y, _grid.Distribution);
            if (tile.IsCollapsed)
            {
                tileCopy.Collapse(tile.Type!.Value);
            }
            else
            {
                tileCopy.PossibleTypes = new HashSet<TileType>(tile.PossibleTypes);
            }
            stateCopy.SetTile(tile.X, tile.Y, tileCopy);
        }
        _backtrackStack.Push(stateCopy);
    }

    private bool Backtrack()
    {
        if (_backtrackStack.Count == 0)
            return false;

        var previousState = _backtrackStack.Pop();

        foreach (var tile in previousState.GetAllTiles())
        {
            var gridTile = _grid.GetTile(tile.X, tile.Y);
            gridTile.Type = tile.Type;
            gridTile.PossibleTypes = new HashSet<TileType>(tile.PossibleTypes);
        }

        _contradiction = false;
        return true;
    }
}
