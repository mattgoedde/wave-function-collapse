namespace WaveFunctionCollapse.Console.Rendering;

using Spectre.Console;
using WaveFunctionCollapse.Core.Models;

public class MapRenderer
{
    private static readonly Dictionary<TileType, string> TileSymbols = new()
    {
        { TileType.Grass, "🟩" },
        { TileType.Water, "🟦" },
        { TileType.Mountain, "⬛" },
        { TileType.Beach, "🟨" }
    };

    public void Render(TileGrid grid)
    {
        for (int y = 0; y < grid.Height; y++)
        {
            var row = string.Empty;
            for (int x = 0; x < grid.Width; x++)
            {
                var tile = grid.GetTile(x, y);
                if (tile.Type.HasValue)
                {
                    row += TileSymbols[tile.Type.Value];
                }
                else
                {
                    row += "⬛";
                }
            }
            AnsiConsole.WriteLine(row);
        }
    }

    public void RenderWithStats(TileGrid grid, TimeSpan generationTime)
    {
        AnsiConsole.MarkupLine("[bold cyan]Wave Function Collapse Map[/]");
        AnsiConsole.MarkupLine($"[dim]Size: {grid.Width}x{grid.Height}[/]");
        AnsiConsole.MarkupLine($"[dim]Generation time: {generationTime.TotalMilliseconds:F2}ms[/]");
        AnsiConsole.WriteLine();

        Render(grid);

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Legend:[/]");
        AnsiConsole.MarkupLine("🟩 Grass");
        AnsiConsole.MarkupLine("🟦 Water");
        AnsiConsole.MarkupLine("🟫 Mountain");
        AnsiConsole.MarkupLine("🟨 Beach");
    }
}
