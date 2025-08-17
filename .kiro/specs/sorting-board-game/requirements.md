# Requirements Document

## Introduction

This document outlines the requirements for Phase 1 of a 3D sorting board game built in Unity 2022 LTS for Windows desktop. The game features a colorful, playful experience where players use mouse controls to pick up and sort colored balls into matching holes on a board. This initial phase focuses on delivering a single level with core gameplay mechanics, visual feedback, and performance tracking to establish the foundation for future development phases. The deliverable includes both a Unity project and a Windows .exe build targeting 1920×1080 resolution at 16:9 aspect ratio.

## Requirements

### Requirement 1: Core Game Board Setup

**User Story:** As a player, I want to see a clear 3D game board with colored holes, so that I understand where to place the balls.

#### Acceptance Criteria

1. WHEN the game starts THEN the system SHALL display a rectangular 3D board measuring 1.0m × 0.6m in Unity units at waist height (y = 0.9m)
2. WHEN the board is displayed THEN the system SHALL show exactly 3 circular holes with diameters of 160px, 140px, and 120px respectively
3. WHEN holes are created THEN the system SHALL color them red, blue, and yellow respectively
4. WHEN the camera is positioned THEN the system SHALL provide a fixed 45-degree angled view with FOV ~45° and no player control
5. WHEN the environment loads THEN the system SHALL use a neutral colored board with clean background and soft lighting

### Requirement 2: Ball Spawning and Physics

**User Story:** As a player, I want balls to appear naturally on the board with realistic movement, so that the game feels engaging and believable.

#### Acceptance Criteria

1. WHEN a level begins THEN the system SHALL spawn one ball at a time onto the board at a rate of 1 ball every 2 seconds
2. WHEN a ball spawns THEN the system SHALL assign it a color (red, blue, or yellow) and diameter of 120px for Level 1
3. WHEN a ball appears THEN the system SHALL make it roll gently onto the board using semi-realistic physics
4. WHEN balls move THEN the system SHALL apply natural rolling physics without mass differences
5. WHEN 20 balls have been placed THEN the system SHALL complete the level and stop spawning new balls

### Requirement 3: Mouse Interaction Controls

**User Story:** As a player, I want to pick up and move balls with my mouse, so that I can sort them into the correct holes.

#### Acceptance Criteria

1. WHEN the player hovers over a ball THEN the system SHALL make the ball glow slightly
2. WHEN the player left-clicks on a ball THEN the system SHALL allow the ball to be picked up
3. WHEN the player drags a ball THEN the system SHALL move the ball following the mouse cursor
4. WHEN the player drags a ball THEN the system SHALL display a trail effect behind the moving ball
5. WHEN the player releases the mouse button THEN the system SHALL drop the ball at the current position
6. WHEN the player presses Escape THEN the system SHALL reset or exit the current level

### Requirement 4: Hole Targeting and Feedback

**User Story:** As a player, I want clear visual feedback when I'm near the correct hole, so that I know where to place each ball.

#### Acceptance Criteria

1. WHEN a ball is dragged near a hole THEN the system SHALL highlight the hole if it matches the ball's color and size
2. WHEN a ball is dropped into the correct hole THEN the system SHALL snap the ball into position
3. WHEN a successful placement occurs THEN the system SHALL display a particle burst effect matching the ball's color
4. WHEN a successful placement occurs THEN the system SHALL play a "pop" sound effect
5. WHEN any ball is placed THEN the system SHALL display a placement error indicator showing how close the placement was to the hole center
6. WHEN a ball is dropped incorrectly THEN the system SHALL bounce the ball off with dull coloring
7. WHEN an incorrect placement occurs THEN the system SHALL play a "buzz" sound effect

### Requirement 5: Performance Tracking and Statistics

**User Story:** As a player, I want to see my performance statistics in real-time and after completing the level, so that I can track my improvement.

#### Acceptance Criteria

1. WHEN the game is active THEN the system SHALL display real-time HUD showing accuracy percentage
2. WHEN the game is active THEN the system SHALL display average placement error in the HUD
3. WHEN the game is active THEN the system SHALL display average completion time in the HUD
4. WHEN the game is active THEN the system SHALL display objects placed per minute in the HUD
5. WHEN a ball is placed THEN the system SHALL record ball_id, spawn_time, grasp_time, release_time, completion_time, ball_color, ball_size, target_color, target_size, placement_error_px, and outcome
6. WHEN a level is completed THEN the system SHALL display a summary with accuracy %, mean placement error, mean completion time, and total throughput
7. WHEN a level is completed THEN the system SHALL save all trial data to a CSV/JSON file with the recorded data fields

### Requirement 6: Audio System

**User Story:** As a player, I want pleasant audio feedback that enhances the gameplay experience, so that the game feels polished and engaging.

#### Acceptance Criteria

1. WHEN the game starts THEN the system SHALL play a light, playful background music loop
2. WHEN background music plays THEN the system SHALL provide an option to mute it
3. WHEN balls roll on the board THEN the system SHALL play subtle rolling sound effects
4. WHEN successful placements occur THEN the system SHALL play clear "pop" sounds
5. WHEN failed placements occur THEN the system SHALL play dull "buzz" sounds
6. WHEN audio plays THEN the system SHALL use simple, fun, non-distracting cartoony style sounds

### Requirement 7: Level 1 Specific Configuration

**User Story:** As a player starting Level 1, I want an appropriately challenging but accessible first experience, so that I can learn the game mechanics without frustration.

#### Acceptance Criteria

1. WHEN Level 1 starts THEN the system SHALL use only 120px diameter balls
2. WHEN Level 1 starts THEN the system SHALL use holes with diameters 160px, 140px, and 120px to accommodate the balls
3. WHEN Level 1 runs THEN the system SHALL spawn balls at 1 ball every 2 seconds
4. WHEN Level 1 is active THEN the system SHALL spawn exactly 20 balls total
5. WHEN Level 1 is active THEN the system SHALL ensure ball colors always match available hole colors
6. WHEN Level 1 completes THEN the system SHALL display completion statistics and provide option to restart

### Requirement 8: Visual Style and Polish

**User Story:** As a player, I want the game to have a colorful, playful appearance that feels inviting and fun, so that I enjoy the visual experience.

#### Acceptance Criteria

1. WHEN visual elements are rendered THEN the system SHALL use bright primary colors for balls and holes
2. WHEN balls are displayed THEN the system SHALL apply glossy textures that look appealing
3. WHEN the overall style is applied THEN the system SHALL maintain a colorful, playful, slightly cartoonish aesthetic
4. WHEN lighting is configured THEN the system SHALL use soft, pleasant lighting that enhances the 3D environment
5. WHEN the game runs THEN the system SHALL maintain smooth visual performance suitable for desktop gameplay
### 
Requirement 9: Performance and Technical Standards

**User Story:** As a player, I want the game to run smoothly without lag or stuttering, so that I can focus on gameplay without technical distractions.

#### Acceptance Criteria

1. WHEN the game runs on a mid-range laptop THEN the system SHALL maintain ≥60 FPS consistently
2. WHEN balls spawn or HUD updates occur THEN the system SHALL not cause frame drops or stuttering
3. WHEN the game is built THEN the system SHALL target Unity 2022 LTS as the development platform
4. WHEN the game is distributed THEN the system SHALL provide a Windows .exe build for 1920×1080 resolution at 16:9 aspect ratio
5. WHEN performance is measured THEN the system SHALL maintain smooth gameplay during all interactive elements