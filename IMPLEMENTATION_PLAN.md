# Wave Function Collapse - Implementation Plan

## Project Overview

A .NET 10 prototype implementation of the Wave Function Collapse (WFC) algorithm for procedural tile-based map generation. The prototype will serve as a proof-of-concept that can be transferred to Unity for a 2D top-down game.

**Goals:**
- Implement a naive WFC algorithm that generates 64x64 tile-based maps
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
│   ├── WaveFunctionCollapse.Core/
│   │   ├── Models/
│   │   │   ├── Tile.cs
│   │   │   ├── TileType.cs
│   │   │   └── Grid.cs
│   │   ├── Algorithm/
│   │   │   ├── WaveFunction.cs
│   │   │   ├── Propagator.cs
│   │   │   └── Constraint.cs
│   │   └── WaveFunctionCollapse.Core.csproj
│   ├── WaveFunctionCollapse.Console/
│   │   ├── Program.cs
│   │   ├── Rendering/
│   │   │   └── MapRenderer.cs
│   │   └── WaveFunctionCollapse.Console.csproj
│
├── tests/
│   └── WaveFunctionCollapse.Tests/
│       ├── AlgorithmTests.cs
│       ├── GridTests.cs
│       └── WaveFunctionCollapse.Tests.csproj
│
├── IMPLEMENTATION_PLAN.md
└── WaveFunctionCollapse.slnx
```

**Key Components:**
- **Core**: WFC algorithm logic, tile models, constraint propagation
- **Console**: Spectre.Console application for visualization
- **Tests**: Unit tests for algorithm correctness

## Implementation Phases

### Phase 1: Core Data Models & Basic Setup
- [ ] Create TileType enum (Grass, Water, Mountain, Beach)
- [ ] Create Tile class to represent grid cells
- [ ] Create Grid class to manage 64x64 tile state
- [ ] Set up project structure with Core and Console assemblies
- [ ] Add Spectre.Console NuGet dependency

### Phase 2: Naive WFC Algorithm Implementation
- [ ] Implement Wave class to track superposition states
- [ ] Implement tile observation (collapse) logic
- [ ] Implement basic constraint propagation
- [ ] Create simple adjacency rules (all tiles can be adjacent initially)
- [ ] Add entropy calculation for tile selection
- [ ] Handle backtracking for contradiction resolution

### Phase 3: Visualization & Testing
- [ ] Create MapRenderer using Spectre.Console
- [ ] Generate and display a 64x64 map
- [ ] Write unit tests for algorithm correctness
- [ ] Test edge cases (contradictions, grid boundaries)
- [ ] Verify output generation rate

### Phase 4: Rule Sets & Refinement (Future)
- [ ] Develop configuration format for adjacency rules
- [ ] Implement tile compatibility matrix
- [ ] Add support for learned patterns from sample images
- [ ] Optimize performance for larger maps

## Success Criteria

- [ ] Algorithm correctly generates patterns following input statistics  
- [ ] Generates valid outputs 95%+ of attempts (low contradiction rate)  
- [ ] Supports variable output sizes  
- [ ] CLI tool is easy to use  
- [ ] Performance acceptable for interactive use (<5s for 512x512)  
- [ ] Code is well-tested and documented  

**Prototype-Specific Criteria:**
- [ ] Successfully generates 64x64 tile maps without contradictions 90%+ of the time
- [ ] All four tile types appear in generated maps
- [ ] Map generation completes in <2 seconds
- [ ] Visual output displays correctly in Spectre.Console with proper colors
- [ ] No unhandled exceptions during generation or rendering
- [ ] Code compiles without warnings in .NET 10
- [ ] **Deterministic generation: same seed produces identical maps every time**

## Notes & Observations

- **Naive Approach**: This prototype uses a simple WFC without learning from input samples. All tiles start with equal probability, and adjacency is permissive initially.
- **Deterministic Seeding**: All random number generation uses a seeded Random instance to ensure reproducible maps. The same seed will always produce the same map layout.
- **Future Enhancement**: As the prototype matures, we'll develop explicit rule sets (JSON/configuration-based) defining which tiles can be adjacent.
- **Backtracking Strategy**: If a contradiction occurs during generation, the algorithm will backtrack to a previous valid state and try an alternative path.
- **Unity Transfer**: The Core library is designed to be platform-agnostic for easy porting to Unity. The Console app demonstrates the concept but won't be part of the game.
- **Performance Target**: 64x64 maps should generate in <2 seconds on typical hardware to support potential game use cases.
