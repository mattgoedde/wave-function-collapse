using Spectre.Console;
using WaveFunctionCollapse.Core.Algorithm;
using WaveFunctionCollapse.Core.Models;
using WaveFunctionCollapse.Console.Rendering;

const int GridSize = 64;
const int Seed = 12345;

var grid = new TileGrid(GridSize, GridSize);
var wave = new Wave(grid, Seed);

var stopwatch = System.Diagnostics.Stopwatch.StartNew();
bool success = wave.Generate();
stopwatch.Stop();

if (success)
{
    var renderer = new MapRenderer();
    renderer.RenderWithStats(grid, stopwatch.Elapsed);
    AnsiConsole.MarkupLine("[bold green]✓ Map generated successfully![/]");
}
else
{
    AnsiConsole.MarkupLine("[bold red]✗ Failed to generate map (contradiction reached).[/]");
}
