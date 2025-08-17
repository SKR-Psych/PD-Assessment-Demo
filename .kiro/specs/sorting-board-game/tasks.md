# Implementation Plan

- [x] 1. Set up Unity project structure and core configuration

  - Create new Unity 2022 LTS project with organized folder structure under Assets/Game/
  - Configure project settings for Windows desktop build targeting 1920×1080 resolution
  - Set up basic scene with camera positioned at 45° angle, FOV 45°, targeting board area
  - _Requirements: 9.3, 9.4_

- [x] 2. Implement game board and hole system

  - Create 3D board GameObject with 1.0m × 0.6m dimensions positioned at y = 0.9m
  - Implement 3 hole GameObjects with diameters 160px, 140px, 120px and colors red, blue, yellow
  - Write HoleManager script to handle hole configuration and collision detection
  - Create hole highlighting system for visual feedback when balls are nearby
  - _Requirements: 1.1, 1.2, 1.3, 4.1_

- [x] 3. Create ball physics and spawning system

  - Implement Ball prefab with 120px diameter, Rigidbody, and Collider components
  - Write BallController script with state machine (Spawning, Idle, Hovered, Dragged, Placed, Failed)
  - Create BallSpawner script that spawns balls every 2 seconds with random colors
  - Implement object pooling system for ball instances to optimize performance
  - Add semi-realistic rolling physics with appropriate physics materials
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_

- [x] 4. Implement mouse interaction system

  - Write MouseInteractionSystem script using raycasting for ball selection
  - Implement click and drag functionality with screen-to-world coordinate conversion
  - Add ball hover detection with glow effect visual feedback
  - Create trail effect system that follows balls during drag operations
  - Handle ball pickup, movement, and release with proper state transitions
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6_

- [x] 5. Create ball placement validation and feedback system

  - Implement collision detection between balls and holes for placement validation
  - Write logic to check color and size matching for successful placements
  - Create ball snapping mechanism for correct placements

  - Implement particle effect system with color-matched effects for successful placements
  - Add placement error calculation (distance from hole center in pixels)
  - Create placement error indicator display (numeric or bar) shown after each placement
  - _Requirements: 4.2, 4.3, 4.4, 4.5, 4.6, 4.7_

- [x] 6. Implement audio system



  - Create AudioManager singleton for centralized audio control
  - Add background music loop with mute option functionality
  - Implement "pop" sound effect for successful ball placements
  - Add "buzz" sound effect for failed placements
  - Create subtle rolling sound effects for ball movement on board
  - Configure audio mixing and 3D positional audio for immersive experience
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6_



- [ ] 7. Create statistics tracking and HUD system

  - Write StatisticsManager script to track all required ball placement metrics
  - Implement real-time calculation of accuracy percentage, average placement error, completion time, and throughput
  - Create HUDManager script with UI elements that update live after each ball placement
  - Display accuracy %, average placement error, average completion time, and objects per minute


  - Ensure HUD updates maintain 60+ FPS performance during gameplay
  - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5_

- [ ] 8. Implement data logging system

  - Create DataLogger script to record all trial data with required fields
  - Implement session_id and trial_id generation for unique identification
  - Add timestamp recording for spawn_time, grasp_time, release_time, completion_time


  - Write CSV and JSON export functionality to Documents/SortingBoard/Logs/{date}/session_xxx
  - Implement file I/O operations that don't block gameplay performance
  - Add data validation and error handling for logging operations
  - _Requirements: 5.6, 5.7_

- [ ] 9. Create level management and completion system



  - Write LevelController script to manage Level 1 configuration (20 balls total)
  - Implement level start/end conditions and ball counting logic
  - Create end-level summary UI displaying final statistics
  - Add level restart functionality and option to exit
  - Ensure level completion triggers data export and statistics summary
  - _Requirements: 7.4, 7.5, 7.6_



- [ ] 10. Implement game state management

  - Create GameManager singleton to orchestrate overall game flow
  - Add pause/resume functionality and Escape key handling for level reset/exit
  - Implement proper scene management and application lifecycle handling
  - Connect all systems through event-driven architecture for loose coupling
  - Add error recovery mechanisms for edge cases (stuck balls, input failures)



  - _Requirements: 3.6, 8.5_

- [ ] 11. Performance optimization and testing

  - Profile game performance to ensure ≥60 FPS on mid-range Windows laptops
  - Test with full 20-ball scenarios to verify no frame drops during gameplay
  - Optimize object pooling, collision detection, and UI update efficiency
  - Implement memory management best practices and garbage collection optimization
  - Add performance monitoring and automatic quality adjustment if needed
  - _Requirements: 9.1, 9.2, 9.5_

- [ ] 12. Final integration and build preparation
  - Integrate all systems and test complete gameplay flow from start to finish
  - Verify all visual and audio feedback works correctly with proper timing
  - Test data logging functionality with complete trial sessions
  - Create Windows .exe build with proper resolution and performance settings
  - Validate build deployment and data export functionality on target platform
  - _Requirements: 8.1, 8.2, 8.3, 9.4_
