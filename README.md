# Hexa Sort

A Unity-based hexagonal color-sorting puzzle game built with clean architecture, service-oriented design, and the MVVM (Model–View–ViewModel) pattern.
The project uses VContainer (DI container) as its Composition Root for dependency injection, state management, and lifecycle control.

## 📋 Table of Contents

- [Overview](#overview)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [Core Systems](#core-systems)
- [Game Mechanics](#game-mechanics)
- [Configuration](#configuration)
- [Third-Party Libraries](#third-party-libraries)
- [Known Issues and Possible Improvements](#known-issues-and-possible-improvements)

---

## 🎮 Overview <a id="overview"></a>

Hexa Sort is a puzzle game where players sort colored hexagonal cells by dragging stacks onto a hexagonal grid. The game features:

- **Hexagonal Grid System**: Cells are arranged in a hexagonal grid pattern
- **Color-Based Sorting**: Cells have different colors and must be sorted according to specific rules
- **Stack Management**: Cells are organized in stacks that can be dragged and placed on the grid
- **Level Progression**: Multiple levels with increasing difficulty
- **Boosters**: Special power-ups to help players (Hammer, Shuffle)
- **Save System**: Persistent game progress

---

## 📁 Project Structure <a id="project-structure"></a>

```
Assets/_Project/Scripts/
│   ├── Runtime/
│   │   ├── Bootstrap/          # Application initialization and DI setup
│   │   │   ├── DI/             # Dependency injection configuration
│   │   │   └── Units/          # Loading units for bootstrap process
│   │   │ 
│   │   ├── Gameplay/           # Core game logic
│   │   │   ├── Core/           # Core models, interfaces, and shared types
│   │   │   │   ├── Interfaces/ # ICell, IStack, ISlot, IDraggable
│   │   │   │   └── Models/     # ColorType, HexCoordinates, HexMetrics, LevelData
│   │   │   │
│   │   │   ├── Domain/         # Business logic layer
│   │   │   │   ├── Boosters/   # Booster system (Hammer, Shuffle)
│   │   │   │   ├── Grid/       # Grid domain logic and services
│   │   │   │   ├── Level/      # Level management
│   │   │   │   └── Stack/      # Stack domain logic and services
│   │   │   │
│   │   │   ├── Infrastructure/ # Infrastructure layer
│   │   │   │   ├── DI/         # Dependency injection installers
│   │   │   │   ├── Factories/ # Object factories (HexGridFactory, HexStackFactory)
│   │   │   │   ├── Input/      # Input handling services
│   │   │   │   ├── State/      # Gameplay state management
│   │   │   │   └── UI/         # UI interfaces (IView, IViewModel)
│   │   │   │
│   │   │   ├── Presentation/  # Presentation layer (Unity-specific)
│   │   │   │   ├── Cell/       # HexCell, HexCellAnimator, HexCellMaterial
│   │   │   │   ├── Grid/       # Grid controllers and slots
│   │   │   │   └── Stack/      # Stack presentation components
│   │   │   │
│   │   │   ├── UI/             # UI layer (MVVM pattern)
│   │   │   │   ├── Boosters/   # Booster UI views and view models
│   │   │   │   ├── Game/       # Main game UI
│   │   │   │   ├── LevelComplete/ # Level completion screen
│   │   │   │   ├── LevelFailed/  # Level failure screen
│   │   │   │   ├── LevelProgression/ # Level progression UI
│   │   │   │   └── Settings/  # Settings UI
│   │   │   │
│   │   │   └── Config/         # ScriptableObject configurations
│   │   │
│   │   └── Utilities/          # Shared utilities
│   │       ├── Loading/        # Loading service
│   │       ├── Logger/          # Custom logging system
│   │       └── Persistence/    # Save/Load services
│   │
│   └── Editor/                 # Editor-only scripts and tools
│
├── Art/                        # Visual assets (sprites, materials, etc.)
├── Bundles/                    # Asset bundles
└── Scenes/                     # Unity scenes
    ├── 0.Bootstrap.unity       # Bootstrap scene
    └── 1.Gameplay.unity        # Main gameplay scene
```

---

## 🏗️ Architecture <a id="architecture"></a>

### Architecture Pattern

The project follows **Clean Architecture** principles with clear separation of concerns:

1. **Core Layer**: Pure C# models, interfaces, and enums (no Unity dependencies)
2. **Domain Layer**: Business logic and game rules (minimal Unity dependencies)
3. **Infrastructure Layer**: External concerns (DI, input, state management)
4. **Presentation Layer**: Unity-specific components (MonoBehaviours, visual representation)
   - **Gameplay Presentation**: Game world objects (Cell, Grid, Stack) - 3D/2D game entities
   - **UI Presentation**: Screen-space user interface (menus, HUD, popups) - MVVM pattern

### Dependency Injection

The project uses **VContainer** for dependency injection:

- **BootstrapScope**: Registers core services (LoadingService, SaveService, LoadService)
- **GameplayScope**: Registers gameplay-specific services, factories, and managers
- **Installers**: Modular installers for different systems (InputInstaller, ConfigInstaller, UIInstaller, BoosterInstaller)

### Service-Based Architecture

The project follows a **service-based architecture** where functionality is organized into discrete, reusable services:

- **Services**: Self-contained components that handle specific responsibilities (e.g., `SaveService`, `LoadService`, `InputService`, `StackMergeService`, `StackSortingService`)
- **Managers**: Coordinate multiple services and manage domain-specific logic (e.g., `LevelManager`, `BoosterManager`, `GameplayStateManager`)
- **Separation of Concerns**: Each service has a single, well-defined responsibility
- **Dependency Injection**: Services are injected where needed, promoting loose coupling and testability
- **Examples**: `PersistenceService`, `LoadingService`, `RaycastService`, `PositionCalculationService`, `GridCleanupService`

### Composition Root

The project implements the **Composition Root** pattern, where all object creation and dependency wiring happens in centralized locations:

- **BootstrapScope**: The root composition point for application-level services
- **GameplayScope**: The composition point for gameplay-specific dependencies
- All dependencies are configured at startup through VContainer's `LifetimeScope`
- This ensures a single point of control for dependency management and makes the system easier to test and maintain

### UI Architecture (MVVM)

The UI follows the **Model-View-ViewModel** pattern:

- **View**: Unity UI components (MonoBehaviours) - `*View.cs`
- **ViewModel**: Business logic and state for views - `*ViewModel.cs`
- **Model**: Data models and domain logic

Example:
- `GameView` + `GameViewModel`
- `LevelCompletedView` + `LevelCompletedViewModel`
- `BoosterView` + `BoosterViewModel`

## 🎯 Core Systems <a id="core-systems"></a>

### 1. Bootstrap System

**Location**: `Runtime/Bootstrap/`

- **BootstrapScope**: Initial DI container setup
- **BootstrapFlow**: Entry point that loads configuration and transitions to gameplay scene
- **ApplicationConfigurationLoadUnit**: Handles initial app configuration loading

**Flow**:
```
BootstrapScene → BootstrapScope → BootstrapFlow → Load Config → GameplayScene
```

### 2. Level Management

**Location**: `Runtime/Gameplay/Domain/Level/`

- **LevelManager**: Manages level progression, completion, and failure
- **LevelData**: Contains level configuration (grid size, colors, stack heights, cells to clear)
- **LevelProgressionConfig**: ScriptableObject defining level progression rules

**Features**:
- Reactive properties for level state (`CurrentLevel`, `CellsCleared`, `Progress`)
- Event streams for level events (`OnLevelStarted`, `OnLevelCompleted`, `OnLevelFailed`)
- Automatic progress calculation

### 3. Grid System

**Location**: `Runtime/Gameplay/Domain/Grid/` and `Runtime/Gameplay/Presentation/Grid/`

- **HexGridMapper**: Maps between grid coordinates, offset coordinates, and world positions
- **GridController**: Manages grid state, slot placement, and neighbor checking
- **HexSlot**: Individual slot in the hexagonal grid
- **HexCoordinates**: Hexagonal coordinate system (axial coordinates)
- **HexMetrics**: Constants for hexagonal grid calculations

**Features**:
- Hexagonal grid layout with offset coordinates
- Automatic centering of grid
- Neighbor detection (6 neighbors per hex)
- Stack placement and validation

### 4. Stack System

**Location**: `Runtime/Gameplay/Domain/Stack/` and `Runtime/Gameplay/Presentation/Stack/`

- **HexStack**: Container for hexagonal cells
- **StackMergeService**: Handles merging stacks together
- **StackSortingService**: Implements color-based sorting logic
- **StackStateAnalyzer**: Analyzes stack state (Empty, Pure, Mixed)
- **HexStackBoard**: Manages the board of draggable stacks

**Stack States**:
- **Empty**: No cells
- **Pure**: All cells have the same color
- **Mixed**: Cells have different colors

**Sorting Rules**:
- Pure → Pure: Merge stacks of the same color
- Mixed → Pure: Transfer matching cells from mixed to pure stack
- Cascading transfers: Multiple cells can transfer in sequence

### 5. Input System

**Location**: `Runtime/Gameplay/Infrastructure/Input/`

- **InputService**: Main input handling service
- **DragService**: Handles drag operations
- **DropService**: Handles drop operations
- **RaycastService**: Raycast-based input detection
- **PositionCalculationService**: Calculates positions for dragged objects
- **BoosterInputService**: Handles booster-specific input

**Features**:
- Unity Input System integration
- Drag and drop for stacks
- Touch and mouse support
- Position calculation for hexagonal grid

### 6. Booster System

**Location**: `Runtime/Gameplay/Domain/Boosters/`

- **BoosterManager**: Manages all boosters and their usage
- **IBooster**: Interface for booster implementations
- **HammerBooster**: Removes a single cell
- **ShuffleBooster**: Shuffles stacks on the board
- **BoosterUnlockConfig**: Defines when boosters unlock

**Features**:
- Booster registration and management
- Usage tracking per level
- Unlock system based on level progression
- Active booster state management

### 7. State Management

**Location**: `Runtime/Gameplay/Infrastructure/State/`

- **GameplayStateManager**: Manages gameplay state machine
- **GameplayState**: Enum defining game states (Playing, LevelCompleted, LevelFailed, etc.)

**States**:
- `Playing`: Normal gameplay
- `LevelCompleted`: Level completion screen
- `LevelFailed`: Level failure screen
- Additional states as needed

### 8. Persistence System

**Location**: `Runtime/Utilities/Persistence/`

- **SaveService**: Handles saving game data to disk
- **LoadService**: Handles loading game data from disk
- **GameSaveData**: Serializable data model for save files

**Features**:
- JSON-based save files
- Async save/load operations using UniTask
- File I/O on background threads
- Automatic directory creation
- Error handling and validation

### 9. Animation System

**Location**: `Runtime/Gameplay/Presentation/Cell/`

- **HexCellAnimator**: Handles cell animations
- **HexAnimationConfig**: ScriptableObject for animation settings
- Uses **DOTween** for tweening

**Animation Types**:
- Merge animations (cells moving between stacks)
- Flip animations during movement
- Staggered animations for multiple cells

---

## 🎮 Game Mechanics <a id="game-mechanics"></a>

### Core Gameplay

1. **Stack Dragging**: Players drag stacks from the stack board onto the hexagonal grid
2. **Color Sorting**: When stacks are placed, automatic sorting occurs based on color rules:
   - Pure stacks of the same color merge together
   - Mixed stacks transfer matching cells to pure stacks
   - Cascading transfers continue until no more matches
3. **Level Objectives**: Clear a certain number of cells by creating matches
4. **Level Failure**: Level fails when all grid slots are filled without completing the objective

### Hexagonal Grid

- Uses **axial coordinates** (X, Z) with Y = -X - Z
- **Offset coordinates** for grid layout (row/column style)
- **6 neighbors** per hex cell (East, Northeast, Northwest, West, Southwest, Southeast)
- Automatic centering based on grid dimensions

### Color System

- **8 Colors**: Red, Blue, Green, Yellow, Purple, Orange, Pink, Cyan
- Each cell has a `ColorType` property
- Colors are mapped to materials via `ColorMaterialConfig`

### Level Progression

- Levels are defined in `LevelProgressionConfig` ScriptableObject
- Each level specifies:
  - Grid dimensions (width × height)
  - Color range (min/max colors)
  - Stack height range
  - Available colors
  - Cells to clear (objective)
- Progress is saved automatically after level completion

## 🔧 Configuration <a id="configuration"></a>

The project uses **ScriptableObjects** for configuration:

- `LevelProgressionConfig` - Level definitions
- `ColorMaterialConfig` - Color to material mapping
- `HexAnimationConfig` - Animation settings
- `BoosterUnlockConfig` - Booster unlock levels
- `BoosterIconConfig` - Booster UI icons

These can be found in `Assets/_Project/Bundles/Configs/` and should be created as assets in the Unity project.

---

## 🛠️ Third-Party Libraries <a id="third-party-libraries"></a>

#### Dependency Injection
- **VContainer** (`jp.hadashikick.vcontainer` v1.17.0)
  - Lightweight DI framework for Unity
  - Used for service registration and dependency injection
  - Supports lifetime scopes (Singleton, Scoped, Transient)

#### Async/Await
- **UniTask** (`com.cysharp.unitask`)
  - Zero-allocation async/await for Unity
  - Used for async operations (save/load, animations)
  - Provides `UniTask` and `UniTask<T>` types

#### Reactive Programming
- **UniRx** (`com.neuecc.unirx`)
  - Reactive Extensions for Unity
  - Used for event streams and reactive properties
  - Provides `ReactiveProperty<T>`, `Subject<T>`, `IObservable<T>`

#### Animation
- **DOTween** (included in `Assets/Plugins/DOTween/`)
  - Tweening library for smooth animations
  - Used for cell animations, UI transitions
  - Configured via `HexAnimationConfig` ScriptableObject

---

## 🐛 Known Issues and Possible Improvements <a id="known-issues-and-possible-improvements"></a>

This section documents known issues and areas for improvement in the current implementation.

### 1. Object Pooling for Cells

**Issue**: 
Currently, hex cells are instantiated and destroyed frequently during gameplay. When cells are cleared or stacks are merged, `GameObject.Destroy()` is called, which causes:
- **Performance Impact**: Frequent garbage collection (GC) spikes
- **Memory Allocation**: Constant allocation/deallocation of GameObjects
- **Frame Drops**: Instantiation and destruction can cause frame rate stutters

**Current Implementation**:
- Cells are instantiated via `Object.Instantiate()` in `HexStackFactory`
- Cells are destroyed via `UnityEngine.Object.Destroy()` in `GridController` and `GridCleanupService`

**Recommended Solution**:
- Implement an object pool for `HexCell` prefabs
- Create a `CellPool` service that manages cell lifecycle
- Reuse cell instances instead of destroying them

---

### 2. Addressables Implementation

**Issue**: 
Currently, all prefabs and assets are referenced directly through ScriptableObjects and passed via the Unity Editor. This approach:

**Current Implementation**:
- All assets must be assigned in the Unity Editor

**Recommended Solution**:
- Migrate to Unity Addressables system

---

### 3. Missing Animations and Tweens

**Issue**: 
The game lacks crucial animations and tweens in both UI and core gameplay, making the experience feel less polished

**Current Implementation**:
- `HexCellAnimator` exists for cell merge animations using DOTween
- Some cell animations are implemented (merge, flip)
- UI views have no transition animations

**Recommended Solution**:
- **UI Animations**:
  - Add fade in/out transitions for screen changes
  - Implement scale/punch animations for button interactions
  - Add slide animations for popups and panels
  - Create smooth transitions between game states
- **Gameplay Animations**:
  - Add bounce/scale animation when stacks are placed
  - Add scale animation when cells get cleared

---

### 4. Missing Audio

**Issue**: 
The game currently has no audio system implemented. There are no sound effects for gameplay actions or background music

---