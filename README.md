# VERTEX GAME - Prototype

A vertical physics-puzzle sandbox game where players must build towers upward while managing ground stability through underground reinforcement.

## 🎮 Game Overview

**Core Concept**: Build a tower upwards while simultaneously digging downwards to reinforce the ground, managing the load of the tower to prevent collapse.

**Target Platform**: PC (Steam)  
**Engine**: Unity  
**Genre**: Physics-Puzzle Sandbox  

## 🎯 Core Objective

The prototype answers a single question: *"Is the core gameplay loop (Build → Face Limitation → Reinforce Underground → Retry) inherently fun and intellectually satisfying?"*

## 🎮 Controls

| Key | Action |
|-----|---------|
| **B** | Toggle Build Mode |
| **1/2/3** | Select Material (Wood/Stone/Steel) |
| **Click** | Build block or Select unit |
| **Shift+Click** | Dig/Remove block |
| **Ctrl+Click** | Multi-select units |
| **V** | Toggle Structural View |
| **WASD/Arrow Keys** | Move camera |
| **Mouse Wheel** | Zoom in/out |
| **ESC** | Pause/Resume |
| **R** | Restart game |
| **F1** | Show help |

## 🏗️ Core Features

### Physics System
- **Event-Driven Calculations**: Physics updates only when structures change
- **Top-Down Load Distribution**: Each block bears weight from all blocks above
- **Material Properties**: Different materials have unique mass and support values
- **Collapse Chain Reactions**: Failed supports trigger cascading collapses

### Materials & Resources

| Material | Mass | Support | Tier | Source |
|----------|------|---------|------|--------|
| Wood | 5 | 100 | 1 | Surface |
| Stone | 20 | 500 | 2 | Underground |
| Steel | 15 | 1500 | 2 | Crafted (Coal + Iron) |
| Dirt | 10 | 50 | - | Underground |
| Coal | - | - | 2 | Underground |
| Iron | - | - | 2 | Underground |

### Ground Stability
- **Foundation System**: Surface tiles act as foundations
- **Load Calculation**: Total ground load vs. maximum ground support
- **Sinkhole Events**: Excessive load causes foundation collapse

### Construction System
- **Block Placement**: Build individual blocks or pillars
- **Unit Commands**: Move, Dig, Build actions
- **Resource Management**: Consume materials for construction
- **Structural Requirements**: Blocks need support below them

## 🎨 Visual Features

### Structural View (Press V)
- **Color-coded Load Status**:
  - 🟢 Green: Safe (< 50% load)
  - 🟡 Yellow: Moderate (50-75% load)
  - 🟠 Orange: Stressed (75-100% load)
  - 🔴 Red: Critical (> 100% load, will collapse)

### Materials Visualization
- **Wood**: Brown blocks
- **Stone**: Gray blocks
- **Steel**: Light gray blocks
- **Dirt**: Dark brown blocks
- **Coal**: Very dark gray blocks
- **Iron**: Rust-colored blocks

## 🏭 Crafting System

### Facilities
- **Workbench**: Basic crafting for wood processing
- **Furnace**: Metal smelting and advanced crafting

### Recipes
- **Steel**: 1 Coal + 2 Iron → 1 Steel
- **Wood Planks**: 2 Wood → 3 Wood (efficiency improvement)

## 🏗️ Architecture

### Modular Design
The codebase is structured for easy expansion to a beta version:

```
Assets/Scripts/
├── Core/
│   ├── Tile.cs                 # Basic tile data structure
│   └── MaterialType.cs         # Material definitions
├── Systems/
│   ├── PhysicsCalculationSystem.cs  # Event-driven physics
│   ├── ResourceManager.cs           # Resource tracking
│   ├── ConstructionSystem.cs        # Building mechanics
│   ├── GameManager.cs              # Main game controller
│   └── CameraController.cs         # Camera management
├── World/
│   ├── WorldGrid.cs            # 2D tile-based world
│   └── WorldRenderer.cs        # Visual representation
├── Units/
│   ├── Unit.cs                 # Individual unit behavior
│   └── UnitManager.cs          # Unit selection/commands
├── UI/
│   └── UIManager.cs            # User interface
├── Visualization/
│   └── StructuralVisualization.cs   # Load status display
└── Crafting/
    ├── CraftingSystem.cs       # Recipe management
    └── CraftingFacility.cs     # Workbench/Furnace
```

## 🎯 Gameplay Loop

1. **Build Phase**: Construct towers using available materials
2. **Physics Response**: System calculates loads and stability
3. **Limitation Discovery**: Reach structural limits or ground capacity
4. **Underground Reinforcement**: Dig and build underground supports
5. **Retry**: Return to building with improved foundation

## 🚀 Setup Instructions

1. **Open in Unity**: Load the project in Unity 2022.3 LTS or later
2. **Load Scene**: Open the main scene in `Assets/Scenes/`
3. **Play**: Press the Play button to start the prototype
4. **Learn**: Press F1 in-game for control help

## 🎮 Prototype Scope

### ✅ Included Features
- Basic unit control (Move, Dig, Build)
- 2D tile-based world with vertical scrolling
- Core physics simulation (load, support, collapse)
- Resource system (Wood, Stone, Coal, Iron)
- Basic construction (blocks, pillars)
- Minimalist UI and structural visualization
- Ground stability mechanics

### ❌ Excluded Features
- Complex simulations (gas, liquid, temperature, electrical)
- Complex unit AI (stress, hygiene, skills, diseases)
- Combat and external threats
- Story and quests
- Economic systems
- Advanced art and sound

## 🔧 Technical Features

### Event-Driven Physics
- Calculations trigger only on structural changes
- Top-down load propagation
- Efficient update system for large structures

### Expandable Architecture
- Modular system design
- Clear separation of concerns
- Easy to add new features for beta version

### Performance Optimized
- Minimal UI for prototype testing
- Efficient rendering system
- Optimized physics calculations

## 📝 Development Notes

This prototype focuses solely on validating the core gameplay mechanics. All systems are designed to be easily expandable for the full beta version while maintaining clean, modular architecture.

The game uses Unity's 2D physics system for simple collision detection and leverages a custom physics calculation system for the load-bearing mechanics that are central to the gameplay.

---

**Project Status**: Prototype Complete  
**Target**: Core Gameplay Validation  
**Next Phase**: Beta Development (Extended Features)