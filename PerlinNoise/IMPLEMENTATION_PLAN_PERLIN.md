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
WaveFunctionCollapse/
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
