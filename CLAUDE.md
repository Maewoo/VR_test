# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a **Unity VR game project** targeting **Android XR/Meta Quest** using **OpenXR**. The game is a ball-throwing mechanic where players hit targets within a time limit.

**Unity Version**: 6000.0.75f1 (LTS)
**Target Platform**: Android XR (Meta Quest)

## Development Setup

### Opening the Project
1. Use Unity Hub to open the project with Unity 6000.0.75f1 (LTS)
2. The project uses **Universal Render Pipeline (URP)** and **XR Interaction Toolkit**
3. XR device simulator available for testing in Editor (accessed via XR Device Simulator menu)

### Building and Running
- **For Meta Quest (Android XR)**:
  - Build Settings: Android → Switch to Android build target
  - Device: Select your Quest headset via `adb devices`
  - Build and Run from File → Build and Run
- **For Editor Testing**: Use XR Device Simulator (Shift + O) to simulate hand/controller input without a headset

### Testing in Editor
- XR Interaction Simulator provides mock XR input (controllers, hands)
- Mock Runtime in OpenXR allows testing XR features without hardware
- Console shows game debug logs including target counts and shot tracking

## Architecture

### Game Loop & Singleton Pattern
The game uses two main singleton controllers:
- **GameManager** (`Assets/Scripts/GameManager.cs`): Main game controller managing timer, targets, win/lose conditions
  - Spawns a 30-second game loop by default
  - Subscribes to ShotTracker events (TargetLanded, ShotEnded)
  - Emits TimeChanged and GameEnded events for UI to consume
  - Win condition: all targets destroyed before time expires
  - Lose condition: time expires or shots run out without hitting all targets

- **ShotTracker** (`Assets/Scripts/ShotTracker.cs`): Tracks individual throw attempts
  - Manages shots remaining counter
  - Raises TargetLanded and ShotEnded events
  - Monitors ball physics to detect completion

### Event System
- **GameEvents** (`Assets/Scripts/GameEvents.cs`): Static event dispatcher for game-wide events
  - Currently exposes `TargetHit` event with score parameter
  - Add new static events here for cross-system communication

### UI Layer
- **ScoreUI** and **TimerUI**: Subscribe to GameManager events and update displays
- Decoupled from game logic via event subscriptions (no direct calls)

### XR Integration
- **XR Interaction Toolkit 3.4.1**: Hand/controller interaction system
- **XR Hands 1.7.3**: Hand tracking with pinch gestures and grab detection
- **OpenXR 1.16.1** with Meta OpenXR 2.5.0 for Quest compatibility
- Input System 1.19.0 for input handling (maps controller/hand input to game actions)

### Asset Organization
```
Assets/
  Scripts/           - Custom game logic
  Scenes/            - Game scenes
  VRTemplateAssets/  - VR UI/interaction components
  XR/                - XR-specific configs (XR Loader, Features)
  Samples/           - XR Toolkit sample scenes (reference only)
```

## Key Development Patterns

### Singletons
Use the pattern from GameManager:
```csharp
public static GameManager Instance { get; private set; }

private void Awake()
{
    if (Instance != null && Instance != this) { Destroy(gameObject); return; }
    Instance = this;
}

private void OnDestroy()
{
    if (Instance == this) Instance = null;
}
```

### Events
Use static events in GameEvents class or instance events on MonoBehaviours. Always unsubscribe in OnDestroy to prevent memory leaks.

### Finding Objects
Use `FindObjectsByType<T>()` (not deprecated `FindObjectsOfType`). Example from GameManager:
```csharp
targets = FindObjectsByType<Target>(FindObjectsSortMode.None);
```

## Important Settings & Configs

### XR Plugin Management
- OpenXR with Meta OpenXR provider configured in Project Settings → XR Plug-in Management
- Android XR feature enabled for Quest support
- Hand Interaction Profile enabled for hand tracking

### Input Actions
- Input actions defined in XRI Default Input Actions asset
- Maps controller triggers, grips, thumbstick to game input
- Hand gestures (pinch, grab) recognized by XR Hands package

### Scene Setup
- Main scene includes XR Rig (player controller), XR Loader, and interaction system
- Physics: Continuous collision detection for thrown objects to not miss targets

## Common Tasks

### Adding a New Target Type
1. Inherit from `Target` class
2. Override interaction methods if needed
3. Register in scene; GameManager auto-discovers via FindObjectsByType

### Adding a UI Element
1. Create in Canvas
2. Subscribe to GameManager events (TimeChanged, GameEnded) in Start()
3. Unsubscribe in OnDestroy()

### Testing VR Interactions
1. Open scene in Editor
2. Press Shift + O to open XR Device Simulator
3. Use mouse + keyboard to simulate controller/hand input (see on-screen guide)
4. Alternatively, use Meta Quest Link with actual headset connected

## Dependencies & Versions
- **Unity 6000.0.75f1** (LTS)
- **URP 17.0.4** (for Android XR compatibility)
- **XR Interaction Toolkit 3.4.1**
- **XR Hands 1.7.3**
- **OpenXR 1.16.1**
- **Meta OpenXR 2.5.0**
- **Input System 1.19.0**
