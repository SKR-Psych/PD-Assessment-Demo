# Sorting Board Game - Unity Implementation

## Overview
This is a Unity implementation of a sorting board game where players drag and drop colored balls into matching holes. The game tracks performance metrics and exports data for analysis.

## Quick Start

### Method 1: Using GameInitializer (Recommended)
1. Create an empty GameObject in your scene
2. Add the `GameInitializer` component to it
3. Configure the settings in the inspector
4. Play the scene - the game will initialize automatically

### Method 2: Using AutoSetup
1. Create an empty GameObject in your scene
2. Add the `AutoSetup` component to it
3. Play the scene - all game components will be created automatically

### Method 3: Manual Setup
1. Add the individual manager components to GameObjects in your scene:
   - GameManager
   - AudioManager
   - StatisticsManager
   - DataLogger
   - BallSpawner
   - HoleManager
   - MouseInteractionSystem
   - LevelController

## Game Components

### Core Managers (Singletons)
- **GameManager**: Main game state management and coordination
- **AudioManager**: Handles all audio effects and background music
- **StatisticsManager**: Tracks real-time performance statistics
- **DataLogger**: Exports game data to CSV/JSON files
- **PerformanceManager**: Monitors and optimizes game performance

### Gameplay Components
- **BallSpawner**: Spawns balls at 2-second intervals (20 balls total)
- **HoleManager**: Manages the 3 colored holes (red, blue, yellow)
- **MouseInteractionSystem**: Handles mouse input for ball dragging
- **BallInteractionHandler**: Processes ball placement and validation
- **LevelController**: Controls level flow and completion

### UI Components
- **HUDManager**: Real-time statistics display
- **EndLevelUI**: Level completion summary screen
- **PlacementErrorIndicator**: Visual feedback for placement accuracy

### Data Classes
- **GameConfig**: Configuration settings (ScriptableObject)
- **BallData**: Individual ball performance data
- **HoleConfig**: Hole configuration data

## Game Specifications

### Level 1 Requirements
- **Total Balls**: 20 balls
- **Spawn Interval**: 2 seconds
- **Ball Diameter**: 120 pixels
- **Board Size**: 1.0m × 0.6m at 0.9m height
- **Hole Diameters**: 160px (red), 140px (blue), 120px (yellow)
- **Camera**: 45° angle, 45° FOV, static position
- **Performance**: ≥60 FPS on mid-range Windows laptops

### Controls
- **Mouse**: Drag and drop balls into holes
- **Escape**: Pause/resume or restart level
- **Space/P**: Pause/resume game
- **F1**: Performance test
- **F2**: Gameplay test
- **F3**: Full system test
- **F5**: Reinitialize game
- **F6**: Log system status
- **F7**: Performance test with full load

### Data Export
Game data is automatically exported to:
- **Location**: `Documents/SortingBoard/Logs/YYYY-MM-DD/`
- **Formats**: CSV and JSON
- **Data**: Session ID, trial data, placement accuracy, completion times

### Statistics Tracked
- **Accuracy**: Percentage of successful placements
- **Average Error**: Mean placement error in pixels
- **Average Time**: Mean completion time per ball
- **Throughput**: Balls placed per minute
- **Success Rate**: Successful vs. total placements

## Testing

### Automated Testing
The game includes comprehensive testing systems:
- **CompilationTest**: Verifies all classes compile correctly
- **GameTester**: Tests all game systems and performance
- **PerformanceManager**: Monitors FPS and optimizes performance

### Manual Testing
1. Use the GameInitializer with testing enabled
2. Press F1-F3 for different test types
3. Check console logs for test results
4. Verify data export in Documents folder

## Performance Optimization

The game includes several performance optimizations:
- **Object Pooling**: For balls and audio sources
- **Automatic Quality Adjustment**: Reduces effects if FPS drops
- **Memory Management**: Automatic garbage collection
- **Particle Optimization**: Limits particle count during low FPS

## Troubleshooting

### Common Issues
1. **No balls spawning**: Check that BallSpawner is present and started
2. **No audio**: Verify AudioManager is initialized
3. **Low FPS**: Enable PerformanceManager for automatic optimization
4. **Data not exporting**: Check DataLogger initialization and file permissions

### Debug Tools
- Enable debug logs in GameInitializer
- Use F6 to check system status
- Check Unity Console for error messages
- Use GameTester for comprehensive system validation

## File Structure
```
Assets/Game/
├── Scripts/
│   ├── Data/           # Data classes and configurations
│   ├── Managers/       # Core game managers
│   ├── Gameplay/       # Game logic components
│   ├── UI/            # User interface components
│   ├── Setup/         # Initialization and setup scripts
│   ├── AutoSetup.cs   # Automatic game setup
│   ├── GameInitializer.cs  # Main initialization script
│   └── CompilationTest.cs  # Compilation verification
├── Scenes/
│   └── MainGame.unity # Main game scene
├── Art/               # Materials and prefabs
├── Audio/             # Audio clips (optional)
└── UI/                # UI prefabs (optional)
```

## Development Notes

### Code Architecture
- Uses namespace organization for clean separation
- Singleton pattern for core managers
- Event-driven communication between systems
- Object pooling for performance
- ScriptableObject for configuration

### Unity Version
- Developed for Unity 2021.3 LTS or later
- Uses standard Unity components (no external packages required)
- Compatible with Windows builds

### Performance Targets
- Maintains ≥60 FPS with 20 active balls
- Memory usage optimized with object pooling
- Automatic quality scaling for lower-end hardware