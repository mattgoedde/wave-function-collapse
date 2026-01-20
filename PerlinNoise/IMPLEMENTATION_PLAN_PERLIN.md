# Wave Function Collapse - Implementation Plan

## Project Overview

A .NET 10 prototype implementation of Perlin Noise for procedural tile-based map generation. The prototype will serve as a proof-of-concept that can be transferred to Unity for a 2D top-down game. 

**Goals:**
- Implement a Perlin Noise algorithm to create tile maps
- Display output using Spectre.Console with colored pixels
- Establish foundation for future rule-set development and optimization
- Verify algorithm generates valid maps without contradictions
- **Ensure deterministic output: identical seeds produce identical maps**

**Tile Types:**
- Grass (green pixel) - #00AA00
- Water (blue pixel) - #0000FF
- Mountain (grey pixel) - #808080
- Beach (yellow pixel) - #FFFF00

## Project Structure

```
PerlinNoise/
├── src/
|   ├── PerlinNoise.Core/ (Logic to generate map layouts)
│   └── PerlinNoise.Console/    (CLI visualization)
├── tests/
│   └── PerlinNoise.Tests/      (Unit tests)
└── PerlinNoise.slnx            (Solution file)
```

**Key Components:**
- **Core**: Perlin Noise algorithm logic, tile models, constraint propagation
- **Console**: Spectre.Console CLI application for visualization
- **Tests**: Unit tests for algorithm correctness

## Success Criteria

### Architecture & Extensibility
- [ ] Noise algorithm abstracted behind an `INoiseGenerator` interface (or similar)
- [ ] Dependency Injection container configured to allow swapping noise implementations
- [ ] Clean separation of concerns: Core logic, abstractions, and console UI

### Deterministic Output
- [ ] Given the same seed, the map is tile-for-tile identical across multiple runs
- [ ] Unit tests verify and document this repeatability

### Tile Generation
- [ ] Map generates with all 4 tile types (Grass, Water, Mountain, Beach)
- [ ] No neighbor constraints enforced (any tile can be adjacent to any tile)
- [ ] Configurable map dimensions (default: 64x64)
- [ ] Noise values (0-1 range) mapped to tiles using quartiles:
  - 0.00-0.25 → Water (#0000FF)
  - 0.25-0.50 → Beach (#FFFF00)
  - 0.50-0.75 → Grass (#00AA00)
  - 0.75-1.00 → Mountain (#808080)

### Visualization
- [ ] Console displays generated map using Spectre.Console
- [ ] Each tile rendered as a colored pixel with correct hex colors
- [ ] 2D top-down perspective representation

### Testing
- [ ] Unit tests verify deterministic output (same seed produces identical maps)
- [ ] No performance benchmarks required at this stage

## Implementation Phases

### Phase 1: Project Setup
**Goal:** Create solution structure and project references
- [x] Create `PerlinNoise.Core` class library project
- [x] Create `PerlinNoise.Console` console app project
- [x] Create `PerlinNoise.Tests` unit test project
- [x] Add project references (Console → Core, Tests → Core)
- [x] Verify all projects compile successfully

### Phase 2: Core Models & Tile Types
**Goal:** Define tile system and data models
- [x] Create `Tile` model class with tile type and color properties
- [x] Create enum for `TileType` (Grass, Water, Mountain, Beach)
- [x] Create `TileColor` constants with hex color values
- [x] Create `MapDimensions` or `MapSize` model (width, height)

### Phase 3: Abstractions & Dependency Injection
**Goal:** Set up interfaces and DI container
- [x] Create `INoiseGenerator` interface with method: `float[,] GenerateNoise(int width, int height, int seed)`
- [x] Create `IMapGenerator` interface for map generation logic
- [x] Set up dependency injection container (Microsoft.Extensions.DependencyInjection)
- [x] Configure DI registrations for interfaces

### Phase 4: Noise Algorithm Implementation
**Goal:** Implement a simple deterministic noise generator returning 2D noise field
- [ ] Implement `SimpleNoiseGenerator : INoiseGenerator` (or Perlin if preferred)
- [ ] Method returns `float[,]` with dimensions matching (width, height)
- [ ] All values in 0-1 range
- [ ] Ensure seeding produces deterministic output

### Phase 5: Noise-to-Tile Mapping
**Goal:** Convert 2D noise field to tile types
- [ ] Create `INoiseToTileMapper` interface with method: `TileType MapNoiseToTile(float noiseValue)`
- [ ] Create `NoiseToTileMapper : INoiseToTileMapper` class
- [ ] Implement quartile mapping logic (0-0.25→Water, 0.25-0.50→Beach, etc.)

### Phase 6: Map Generator Implementation
**Goal:** Orchestrate map generation using noise field and tile mapping
- [ ] Implement `MapGenerator : IMapGenerator`
- [ ] Accept seed, dimensions, INoiseGenerator, and INoiseToTileMapper as dependencies
- [ ] Call INoiseGenerator to get 2D noise field (float[,])
- [ ] Iterate through noise field and use INoiseToTileMapper to convert each float to TileType
- [ ] Return 2D Tile array (Tile[,])

### Phase 7: Console Visualization
**Goal:** Display generated maps in console
- [ ] Integrate Spectre.Console NuGet package
- [ ] Create `MapRenderer` class to format tile map for console
- [ ] Implement colored pixel/character rendering using tile colors
- [ ] Create console application entry point to generate and display a map

### Phase 8: Deterministic Output Testing
**Goal:** Verify map reproducibility
- [ ] Create unit tests for `MapGenerator` with known seeds
- [ ] Verify identical seeds produce identical maps
- [ ] Test with multiple map dimensions
- [ ] Document test expectations

## Notes & Observations

### Architectural Decisions
- **2D Noise Field**: INoiseGenerator returns a complete `float[,]` noise field rather than sampling individual coordinates. This is cleaner and more efficient.
- **Multidimensional Arrays**: Using `float[,]` and `Tile[,]` for 2D data (more intuitive for grid representation than jagged arrays).
- **Mapper Interface**: INoiseToTileMapper is injected into MapGenerator for flexibility and testability.
