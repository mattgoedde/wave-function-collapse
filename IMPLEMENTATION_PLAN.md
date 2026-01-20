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
│   ├── WaveFunctionCollapse.Core/       (Algorithm logic and models)
│   └── WaveFunctionCollapse.Console/    (CLI visualization)
├── tests/
│   └── WaveFunctionCollapse.Tests/      (Unit tests)
└── WaveFunctionCollapse.slnx            (Solution file)
```

**Key Components:**
- **Core**: WFC algorithm logic, tile models, constraint propagation
- **Console**: Spectre.Console CLI application for visualization
- **Tests**: Unit tests for algorithm correctness

## Implementation Phases

### Phase 0: Initial Project Setup
- [x] Create WaveFunctionCollapse.Core class library (.NET 10)
- [x] Create WaveFunctionCollapse.Console console application (.NET 10)
- [x] Create WaveFunctionCollapse.Tests unit test project (xUnit)
- [x] Set up WaveFunctionCollapse.slnx with all three projects
- [x] Add Spectre.Console NuGet package to Console project
- [x] Add xUnit NuGet packages to Tests project
- [x] Configure Console project to reference Core library
- [x] Configure Tests project to reference Core library
- [x] Verify solution builds without errors
- [x] Create basic project folder structure (src/, tests/, Models/, Algorithm/, Rendering/)

### Phase 1: Core Data Models & Basic Setup
- [x] Create TileType enum (Grass, Water, Mountain, Beach)
- [x] Create Tile class to represent grid cells
- [x] Create Grid class to manage 64x64 tile state
- [x] Set up project structure with Core and Console assemblies
- [x] Add Spectre.Console NuGet dependency

### Phase 2: Naive WFC Algorithm Implementation
- [x] Implement Wave class to track superposition states
- [x] Implement tile observation (collapse) logic
- [x] Implement basic constraint propagation
- [x] Create simple adjacency rules (all tiles can be adjacent initially)
- [x] Add entropy calculation for tile selection
- [x] Handle backtracking for contradiction resolution

### Phase 3: Visualization & Testing
- [x] Create MapRenderer using Spectre.Console
- [x] Generate and display a 64x64 map
- [x] Write unit tests for algorithm correctness
- [x] Test edge cases (contradictions, grid boundaries)
- [x] Verify output generation rate

### Phase 4: Type-Specific Adjacency Rules (Hardcoded)
- [ ] Implement tile adjacency lookup table (hardcoded rules)
- [ ] Update constraint propagation to enforce type-specific rules
- [ ] Create AdjacencyRules class to manage valid tile combinations
- [ ] Test adjacency enforcement for all tile types
- [ ] Verify Mountains are only adjacent to Grass and other Mountains
- [ ] Verify Water and Beach form coastal boundaries
- [ ] Write unit tests for each tile type's allowed adjacencies
- [ ] Write integration tests for map generation with new rules
- [ ] Verify no contradictions arise from the new rule set
- [ ] Test edge cases (islands, peninsulas, inland lakes)

### Phase 5: Configurable Rule Sets (Future)
- [ ] Develop configuration format for adjacency rules
- [ ] Implement tile compatibility matrix from config file
- [ ] Add support for learned patterns from sample images

### Phase 6: Performance Optimization with LINQ
- [ ] Profile current generation performance (identify bottlenecks)
- [ ] Replace manual loops with LINQ queries for tile filtering
- [ ] Optimize entropy calculation using LINQ (Min/Where/Select chains)
- [ ] Use LINQ to optimize constraint propagation (querying affected neighbors)
- [ ] Implement LINQ-based adjacency checking (parallel Where/All/Any)
- [ ] Optimize wave state queries using LINQ (collapsed/uncollapsed tile queries)
- [ ] Consider PLINQ for parallel processing of independent tiles
- [ ] Benchmark generation time before and after optimizations
- [ ] Target: Generate 64x64 maps in <500ms consistently
- [ ] Write performance regression tests

## Success Criteria

- [ ] Algorithm correctly generates patterns following input statistics  
- [ ] Generates valid outputs 95%+ of attempts (low contradiction rate)  
- [ ] Supports variable output sizes  
- [x] CLI tool is easy to use  
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

## Tile Adjacency Rules (Phase 4 & Beyond)

### Hardcoded Adjacency Rules (Phase 4)
Each tile type has specific constraints on which other tile types can be adjacent (in any direction):

- **Grass**: Can be adjacent to [Grass, Beach, Mountain]
- **Mountain**: Can be adjacent to [Grass, Mountain] (no Beach or Water)
- **Beach**: Can be adjacent to [Beach, Grass, Water]
- **Water**: Can be adjacent to [Water, Beach]

**Design Rationale:**
- Mountains form interior landmasses and cannot border water or beaches directly
- Beaches provide natural transitions between land (Grass, Mountain) and water
- Water and Grass can only meet through Beach tiles
- This creates realistic coastal/terrain geography in generated maps

### Future: Configurable Rules (Phase 5)
Will transition to configuration-based rules (JSON/YAML) to support:
- Custom tile types
- Learned patterns from sample images
- Multiple rule sets for different biomes

## Notes & Observations

- **Naive Approach**: This prototype uses a simple WFC without learning from input samples. All tiles start with equal probability, and adjacency is permissive initially.
- **Deterministic Seeding**: All random number generation uses a seeded Random instance to ensure reproducible maps. The same seed will always produce the same map layout.
- **Phase 4 Focus**: Implementing hardcoded type-specific adjacency rules to create more realistic map generation with logical terrain constraints.
- **Future Enhancement**: As the prototype matures, we'll develop explicit rule sets (JSON/configuration-based) defining which tiles can be adjacent.
- **Backtracking Strategy**: If a contradiction occurs during generation, the algorithm will backtrack to a previous valid state and try an alternative path.
- **Unity Transfer**: The Core library is designed to be platform-agnostic for easy porting to Unity. The Console app demonstrates the concept but won't be part of the game.
- **Performance Target**: 64x64 maps should generate in <2 seconds on typical hardware to support potential game use cases.
