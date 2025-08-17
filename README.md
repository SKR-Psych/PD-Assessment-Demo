# Sorting Board Game

A 3D sorting board game built in Unity 2022 LTS for Windows desktop. Players use mouse controls to pick up colored balls and place them into matching holes on a board.

## Features

- **3D Gameplay**: Interactive 3D environment with physics-based ball movement
- **Mouse Controls**: Click and drag balls with visual feedback (glow, trails)
- **Visual Feedback**: Particle effects, hole highlighting, placement error indicators
- **Audio System**: Background music, success/failure sounds, rolling ball audio
- **Real-time Statistics**: Live HUD showing accuracy, error, time, and throughput
- **Data Logging**: Automatic export of trial data to CSV and JSON formats
- **Performance Optimized**: Maintains ≥60 FPS on mid-range Windows laptops

## Level 1 Specifications

- **Board**: 1.0m × 0.6m at waist height (y = 0.9m)
- **Holes**: 3 holes with diameters 160px, 140px, 120px (red, blue, yellow)
- **Balls**: 20 balls total, 120px diameter, spawned every 2 seconds
- **Camera**: Fixed 45° angle, 45° FOV, optimized for 1920×1080 resolution

## System Requirements

- **OS**: Windows 10/11 (64-bit)
- **Resolution**: 1920×1080 (16:9 aspect ratio)
- **Performance**: Mid-range laptop capable of 60+ FPS
- **Storage**: ~100MB for game + data logs

## Installation & Setup

### Option 1: Play Pre-built Game (Recommended)
1. Download the latest release from the Builds folder
2. Extract `SortingBoardGame.exe` to your desired location
3. Run `SortingBoardGame.exe`
4. Game data will be saved to `Documents/SortingBoard/Logs/`

### Option 2: Build from Source
1. **Install Unity 2022 LTS**
   - Download Unity Hub from [unity.com](https://unity.com)
   - Install Unity 2022.3 LTS (free)

2. **Open Project**
   - Open Unity Hub
   - Click "Add" and select this project folder
   - Open the project in Unity

3. **Build Game**
   - In Unity, go to `Sorting Board Game > Build Windows Executable`
   - Or use `File > Build Settings`, select Windows, and click Build
   - Choose output location (recommended: `Builds/` folder)

## How to Play

1. **Objective**: Sort colored balls into matching holes
2. **Controls**:
   - **Mouse**: Hover over balls to see them glow
   - **Left Click + Drag**: Pick up and move balls
   - **Release**: Drop balls into holes
   - **Escape**: Pause/restart level
   - **P/Space**: Pause/resume game

3. **Scoring**:
   - **Success**: Ball matches hole color and size
   - **Failure**: Wrong color, size, or missed hole
   - **Accuracy**: Percentage of successful placements
   - **Error**: Distance from hole center (in pixels)

4. **Level Complete**: Place all 20 balls to see final statistics

## Data Export

Game automatically saves detailed trial data:

- **Location**: `Documents/SortingBoard/Logs/{date}/session_{timestamp}`
- **Formats**: Both CSV and JSON
- **Data Fields**: 
  - `session_id`, `trial_id`
  - `spawn_time`, `grasp_time`, `release_time`, `completion_time`
  - `ball_color`, `ball_size`, `target_color`, `target_size`
  - `placement_error_px`, `outcome`

## Performance Features

- **Object Pooling**: Efficient ball and audio source management
- **Automatic Optimization**: Reduces particle effects if FPS drops
- **Memory Management**: Garbage collection optimization
- **Quality Settings**: Optimized for consistent 60+ FPS

## Testing & Debugging

### Manual Testing Hotkeys
- **F1**: Performance test (monitors FPS with full load)
- **F2**: Gameplay systems test
- **F3**: Full system test (all components)

### Performance Monitoring
- Real-time FPS monitoring
- Automatic optimization when FPS drops below 48
- Memory usage tracking
- System health monitoring

## Project Structure

```
Assets/Game/
├── Scenes/
│   └── MainGame.unity          # Main game scene
├── Scripts/
│   ├── Managers/               # Core system managers
│   ├── Gameplay/               # Ball, hole, interaction logic
│   ├── UI/                     # HUD and menu systems
│   └── Data/                   # Configuration and data structures
├── Art/
│   ├── Materials/              # Visual materials
│   └── Prefabs/                # Game object prefabs
└── Audio/
    ├── Music/                  # Background music
    └── SFX/                    # Sound effects
```

## Development Notes

### Architecture
- **Singleton Managers**: GameManager, AudioManager, StatisticsManager, DataLogger
- **Event-Driven**: Loose coupling between systems via events
- **Component-Based**: Unity's GameObject/Component architecture
- **Object Pooling**: Performance optimization for balls and audio

### Key Systems
1. **GameManager**: Overall game state and coordination
2. **BallSpawner**: Object pooling and ball lifecycle
3. **MouseInteractionSystem**: Raycast-based ball selection
4. **HoleManager**: Collision detection and validation
5. **StatisticsManager**: Real-time performance tracking
6. **DataLogger**: Trial data export and session management

## Troubleshooting

### Common Issues
- **Low FPS**: Game automatically optimizes, but check system requirements
- **No Audio**: Check Windows audio settings and game volume controls
- **Data Not Saving**: Ensure Documents folder has write permissions
- **Mouse Not Working**: Check if other applications are capturing mouse input

### Debug Information
- Console logs provide detailed system status
- Press F3 for full system diagnostic
- Check `Documents/SortingBoard/Logs/` for data export confirmation

## Future Development

This Phase 1 implementation provides the foundation for:
- Additional levels with increased difficulty
- VR adaptation (Unity project structure supports this)
- Advanced analytics and machine learning integration
- Multiplayer capabilities
- Custom level editor

## License

This project is developed for research and educational purposes.

## Support

For technical issues or questions:
1. Check console logs for error messages
2. Verify system requirements
3. Test with F1/F2/F3 diagnostic keys
4. Check data export in Documents folder

---

**Version**: 1.0.0  
**Unity Version**: 2022.3 LTS  
**Target Platform**: Windows Desktop  
**Build Date**: 2025