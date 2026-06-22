# IGP Project

This is a 2D Unity tower-defense survival game with Arduino controller support. The player defends a central tower while waves of enemies move in from different spawn points. Enemies become stronger over time, bosses appear at timed stages, and defeating the final boss wins the game.

## How It Works

- Move the player around the screen and attack enemies when they enter the attack range.
- Enemies that reach the tower deal damage based on their type.
- Defeated enemies give gold, which can be spent in the shop.
- Shop upgrades improve tower health, attack speed, attack damage, lifesteal, and defensive ranken/spikes.
- Bosses spawn at fixed time milestones and unlock new enemy types after they are defeated.
- The Arduino joystick can control movement and shop navigation, while LEDs show stage progress.

## Controls

The game supports Unity input controls and an Arduino joystick. With the Arduino setup, joystick directions move the player, the button opens the shop or buys the selected upgrade, and directional input navigates the shop while it is open.

## Project Structure

- `Assets/Scripts` contains the gameplay logic.
- `Assets/Scenes` contains the Unity scenes.
- `Arduino/Arduino.ino` contains the Arduino joystick and LED code.
