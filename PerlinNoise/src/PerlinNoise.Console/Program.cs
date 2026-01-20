using Microsoft.Extensions.DependencyInjection;
using PerlinNoise.Console;
using PerlinNoise.Core;
using PerlinNoise.Core.Abstractions;
using PerlinNoise.Core.DependencyInjection;

var services = new ServiceCollection();
services.AddPerlinNoiseCore();
var serviceProvider = services.BuildServiceProvider();

var mapGenerator = serviceProvider.GetRequiredService<IMapGenerator>();
var renderer = new MapRenderer();

const int width = 128;
const int height = 128;
const int seed = 42;

var tileMap = mapGenerator.GenerateMap(width, height, seed);
renderer.Render(tileMap);
