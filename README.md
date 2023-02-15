# Rotateris 

Rotateris is a puzzle game in which the player is challenged to rotate and move differently shaped blocks as they fall from the top of the playing field(**_s_**). The goal is to arrange the blocks to form complete rows, which will then disappear and score points. An interesting **_twist_** is that there can be multiple playing fields at once, which can be played simultaneously by rotating the camera between them.

# Features
An overview of the main features of the game with GIF previews:

- The __main menu__, which has settings that allow you to change resolution and game audio levels.

<p align="center">
  <img width="500" height="400" src="https://github.com/iGenode/rotateris/blob/gifs/settings.gif?raw=true">
</p>

- The __preview screen__ offers a variety of settings to customize your playing experience, allowing you to get the most out of your game.

<p align="center">
  <img width="500" height="400" src="https://github.com/iGenode/rotateris/blob/gifs/preview%20screen.gif?raw=true">
</p>

- The __game scene__ includes a subset of features:

  - __Rotating__ between playing fields

  <p align="center">
  <img width="500" height="400" src="https://github.com/iGenode/rotateris/blob/gifs/rotating%20around.gif?raw=true">
  </p>

  - __Holding__ shapes and being able to transfer them between fields

  <p align="center">
  <img width="500" height="400" src="https://github.com/iGenode/rotateris/blob/gifs/holding%20shapes.gif?raw=true">
  </p>

  - Changing input schemes on the fly

  <p align="center">
  <img width="500" height="400" src="https://github.com/iGenode/rotateris/blob/gifs/control%20scheme%20change.gif?raw=true">
  </p>

  - Pause menu with settings

  <p align="center">
  <img width="500" height="400" src="https://github.com/iGenode/rotateris/blob/gifs/pause%20menu.gif?raw=true">
  </p>

  - Game over screen with a brief summary of the game and a high score that is saved between sessions

  <p align="center">
  <img width="500" height="400" src="https://github.com/iGenode/rotateris/blob/gifs/game%20over.gif?raw=true">
  </p>
  
  GIFs might take a while to load, so a full video is also available [here](https://youtu.be/mW0oe_7NQMM)

# Tech stack

Here's a brief overview of the tech stack the rotateris uses:
- Input System with PlayerInput components set to Invoke Unity Events for player controls as well as control scheme swapping on Controls Changed Event. Control schemes are easily extendable using a custom ScriptableObject with fields for button sprites.
- PlayerPrefs used to store AudioMixer settings 
- System.IO used for writing and reading save data
- Object Pooling used to optimize playing audio clips on demand and prevent too many instances of AudioSources from being instantiated
- Splines package is used to create a path for the camera to follow when rotating around the playing fields

# Install

Download and install __latest release__ on repo's [release page](https://github.com/iGenode/rotateris/releases):
 - Installer: You can install the game using an installer, which allows you to select the installation location and create an uninstaller. 
 - Portable: The game is available in a ready-to-play format, packed in a zip file.

___

##### TODO's

- Change camera rotation logic to prevent bugs on high field counts
- Add a better previw of difficulty levels
- Generalize the creation of new shape prefabs
- Use shader graph for a glimmer shader on shapes
