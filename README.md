# Incremental Tower Defense

A 2D Unity tower-defense survival game developed as a team project. The player protects a central tower against increasingly difficult enemy waves, earns currency from defeated enemies, and upgrades the tower through an in-game shop.

The project can be played with standard Unity controls and optionally supports an Arduino joystick with LED stage feedback.

## Gameplay

- Defend a central tower against enemies approaching from multiple spawn points.
- Defeat enemies to earn gold for upgrades.
- Improve tower health, attack speed, damage, lifesteal, spikes, and defensive rank.
- Face timed boss encounters that unlock additional enemy types.
- Defeat the final boss to win the run.


## Tech Stack

- Unity 6000.3.10f1
- C#
- Unity UI Toolkit / UIElements
- Unity Input System
- Universal Render Pipeline
- Arduino serial input with joystick and LED feedback

## Run Locally

1. Clone the repository.
2. Open the repository root in Unity Hub with Unity 6000.3.10f1.
3. Let Unity restore packages, then open a game scene from `Assets/Scenes`.
4. Press Play to run with the default Unity controls.

### Optional Arduino Controller

The Arduino controller is optional. Its source is located at `Arduino/Arduino.ino`.

1. Upload the sketch to a compatible Arduino board.
2. Connect the joystick and LEDs as defined in the sketch.
3. Update the configured serial port in Unity if necessary. The included scenes currently use `COM3`; macOS and Linux typically require a `/dev/cu.*` or `/dev/tty*` device path.

## Project Structure

- `Assets/Scripts` - game loop, enemies, tower systems, UI, and input
- `Assets/Scenes` - Unity scenes
- `Arduino/Arduino.ino` - Arduino joystick and LED controller
- `Packages` - Unity package configuration

## Status

Student team project and portfolio piece. The repository contains the Unity project source and optional Arduino integration.
