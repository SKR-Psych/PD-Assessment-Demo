# Design Document - Sorting Board Game

## Overview

This design document outlines the technical architecture for a Unity 2022 LTS-based 3D sorting board game. The system implements mouse-driven ball manipulation with physics simulation, real-time performance tracking, and rich audio-visual feedback. The architecture prioritizes modularity, performance, and extensibility for future development phases.

## Architecture

### High-Level System Architecture

```
Game Manager
├── Scene Management
├── Level Controller
├── Statistics Manager
└── Audio Manager

Gameplay Systems
├── Ball Spawner
├── Ball Controller
├── Hole Manager
├── Mouse Interaction System
└── Physics Manager

UI Systems
├── HUD Manager
├── End Level UI
└── Settings UI

Data Systems
├── Performance Tracker
├── Data Logger
└── Configuration Manager
```

### Core Design Patterns

- **Singleton Pattern**: GameManager, AudioManager for global access
- **Observer Pattern**: Event system for ball placement, UI updates
- **Object Pooling**: Ball instances for performance optimization
- **State Machine**: Ball states (spawning, idle, dragged, placed)
- **Component-Based Architecture**: Unity's GameObject/Component system

## Components and Interfaces

### 1. Game Management Layer

**GameManager (Singleton)**
- Orchestrates overall game flow and state
- Manages scene transitions and level progression
- Coordinates between all major systems
- Handles application lifecycle (pause, quit, restart)

**LevelController**
- Manages Level 1 specific configuration (20 balls, spawn timing)
- Controls level start/end conditions
- Interfaces with StatisticsManager for level completion data

**ConfigurationManager**
- Stores game constants (ball sizes, hole positions, spawn rates)
- Manages build-specific settings (resolution, performance targets)
- Provides centralized access to tunable parameters

### 2. Gameplay Systems

**BallSpawner**
- Spawns balls at 2-second intervals using Unity Coroutines
- Manages ball object pool for performance
- Assigns random colors (red, blue, yellow) to spawned balls
- Handles spawn positioning and initial physics impulse

**BallController (MonoBehaviour)**
- Manages individual ball behavior and state machine
- States: Spawning, Idle, Hovered, Dragged, Placed, Failed
- Handles visual effects (glow, trail) based on state
- Integrates with Rigidbody for physics simulation

**MouseInteractionSystem**
- Performs raycasting from camera to detect ball selection
- Manages drag operations with screen-to-world coordinate conversion
- Handles ball pickup, movement, and release logic
- Provides hover detection for visual feedback

**HoleManager**
- Manages the 3 holes (160px, 140px, 120px diameters)
- Handles collision detection for ball placement validation
- Triggers success/failure feedback based on color/size matching
- Manages hole highlighting when balls are nearby

**PhysicsManager**
- Configures Unity physics settings for optimal ball behavior
- Manages collision layers and physics materials
- Handles ball-to-hole snapping mechanics
- Ensures consistent 60+ FPS physics simulation

### 3. Audio-Visual Systems

**AudioManager (Singleton)**
- Manages background music loop with mute option
- Plays positional sound effects (pop, buzz, roll)
- Handles audio mixing and volume control
- Implements audio object pooling for performance

**VisualEffectsManager**
- Manages particle systems for success feedback with color matching ball color
- Handles ball glow effects during hover/drag
- Implements trail renderer for ball movement
- Controls hole highlighting animations
- Displays numeric/bar placement error indicator after each ball placement

**CameraController**
- Maintains fixed 45° angled view with 45° FOV
- Positioned to frame 1.0m × 0.6m board at y = 0.9m optimally
- No player control - completely static positioning
- Optimized for 1920×1080 16:9 aspect ratio

### 4. UI and Data Systems

**HUDManager**
- Real-time display of accuracy percentage (updates live per ball)
- Shows average placement error and completion time (updates live per ball)
- Updates objects placed per minute counter (updates live per ball)
- Implements efficient UI update patterns to maintain 60+ FPS

**StatisticsManager**
- Tracks all required metrics per ball placement
- Calculates real-time averages and aggregates
- Provides data to HUD and end-level summary
- Interfaces with DataLogger for persistence

**DataLogger**
- Exports trial data to both CSV and JSON format in Documents/SortingBoard/Logs/{date}/session_xxx
- Records: session_id, trial_id, spawn_time, grasp_time, release_time, completion_time, ball_color, ball_size, target_color, target_size, placement_error_px, outcome
- Handles file I/O operations without blocking gameplay
- Implements data validation and error handling

## Data Models

### Ball Data Structure
```csharp
public class BallData
{
    public string SessionId { get; set; }
    public int TrialId { get; set; }
    public float SpawnTime { get; set; }
    public float GraspTime { get; set; }
    public float ReleaseTime { get; set; }
    public float CompletionTime { get; set; }
    public BallColor BallColor { get; set; }
    public float BallSize { get; set; } = 120f; // 120px diameter
    public BallColor TargetColor { get; set; }
    public float TargetSize { get; set; }
    public float PlacementErrorPx { get; set; }
    public PlacementOutcome Outcome { get; set; }
}
```

### Hole Configuration
```csharp
public class HoleConfig
{
    public Vector3 Position { get; set; }
    public float Diameter { get; set; } // 160px, 140px, or 120px
    public BallColor Color { get; set; } // Red, Blue, or Yellow
    public Material HoleMaterial { get; set; }
}
```

### Level Configuration
```csharp
public class LevelConfig
{
    public int TotalBalls { get; set; } = 20; // Exactly 20 balls for Level 1
    public float SpawnInterval { get; set; } = 2.0f; // 2 second intervals
    public float BallDiameter { get; set; } = 120f; // 120px diameter
    public Vector3 BoardSize { get; set; } = new Vector3(1.0f, 0.0f, 0.6f); // 1.0m × 0.6m
    public float BoardHeight { get; set; } = 0.9f; // y = 0.9m waist height
    public List<HoleConfig> Holes { get; set; } // 160px, 140px, 120px holes
}
```

## Error Handling

### Physics Error Recovery
- Ball stuck detection with automatic reset
- Collision system fallbacks for edge cases
- Physics timestep management for consistent behavior
- Boundary detection to prevent balls leaving play area

### Input Error Handling
- Mouse raycast failure fallbacks
- Drag operation interruption recovery
- Multi-touch prevention on Windows desktop
- Input validation for edge cases

### Performance Error Mitigation
- Frame rate monitoring with automatic quality adjustment
- Memory management for object pools
- Garbage collection optimization
- Audio system fallbacks for missing sound files

### Data Persistence Errors
- File I/O error handling with user notification
- Data validation before export
- Backup data storage in memory
- Graceful degradation if logging fails

## Testing Strategy

### Unit Testing
- Ball state machine transitions
- Statistics calculation accuracy
- Data logging functionality
- Configuration management
- Audio system integration

### Integration Testing
- Mouse interaction with physics system
- Ball spawning and placement workflow
- HUD updates with statistics system
- Audio-visual feedback coordination
- Level completion flow

### Performance Testing
- ≥60 FPS maintenance on mid-range Windows laptops
- Test with full 20 balls per level to verify no frame drops
- Memory usage profiling during extended gameplay
- Physics simulation stability with multiple balls
- UI update efficiency with live HUD updates
- Audio system performance impact

### User Experience Testing
- Ball manipulation responsiveness
- Visual feedback clarity and timing
- Audio feedback appropriateness
- HUD readability and accuracy
- Level completion satisfaction

### Platform Testing
- Windows desktop compatibility
- 1920×1080 resolution optimization
- Mid-range laptop performance validation
- Input device compatibility
- Build deployment verification

## Technical Implementation Notes

### Unity-Specific Considerations
- Use Unity 2022 LTS features for stability
- Implement proper GameObject lifecycle management
- Utilize Unity's built-in physics for ball behavior
- Leverage Unity's audio system for 3D positional sound
- Use Unity's UI system (Canvas) for HUD implementation

### Performance Optimization
- Object pooling for balls to reduce instantiation overhead
- Efficient collision detection using appropriate collider types
- Optimized particle systems with limited particle counts
- Texture atlasing for UI elements
- Audio compression for sound effects

### Extensibility Design
- Modular level configuration system for future levels
- Pluggable difficulty scaling mechanisms
- Expandable statistics tracking system
- Configurable visual effects system
- Scalable audio management for additional sounds
## Projec
t Structure and Deliverables

### Unity Project Organization
```
Assets/
├── Game/
│   ├── Scenes/
│   │   ├── MainGame.unity
│   │   └── MainMenu.unity
│   ├── Scripts/
│   │   ├── Managers/
│   │   ├── Gameplay/
│   │   ├── UI/
│   │   └── Data/
│   ├── UI/
│   │   ├── Prefabs/
│   │   ├── Materials/
│   │   └── Fonts/
│   ├── Audio/
│   │   ├── Music/
│   │   ├── SFX/
│   │   └── AudioMixers/
│   └── Art/
│       ├── Materials/
│       ├── Textures/
│       ├── Models/
│       └── Particles/
```

### Build Deliverables
- **Unity 2022 LTS Project**: Complete source project with organized asset structure
- **Windows .exe Build**: Standalone executable targeting 1920×1080 resolution
- **Data Output**: Automatic creation of Documents/SortingBoard/Logs/{date}/session_xxx files
- **Documentation**: Build instructions and deployment guide

### Data Export Structure
**File Location**: `Documents/SortingBoard/Logs/{YYYY-MM-DD}/session_{timestamp}.csv`
**File Location**: `Documents/SortingBoard/Logs/{YYYY-MM-DD}/session_{timestamp}.json`

**Data Fields**: session_id, trial_id, spawn_time, grasp_time, release_time, completion_time, ball_color, ball_size, target_color, target_size, placement_error_px, outcome