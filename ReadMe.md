

<h1 align="center">FamiliAR</h1>

<p align="center">
  <a href="#introduction">Introduction</a> •
<a href="#features">Features</a> •
  <a href="#setup">Setup</a> •
  <a href="#build">Build</a> •
  <a href="#usage">Usage</a> •
  <a href="#acknowledgments">Acknowledgments</a> •
  <a href="#license">License</a>
</p>

## Introduction

FamiliAR introduces fundamental operation and interaction mechanisms in Augmented Reality (AR) on the Microsoft HoloLens 2. 
It originated in the scope of a research project, aiming to explore and improve mental accessibility of instructional content in AR on Head-mounted Displays (HMDs).
It was created for the purpose of familiarizing people, who have never experienced AR on HMDs, with the underlying technology. 

<p align="center">
  <img src="https://github.com/anjelomerte/FamiliAR/blob/main/Images/buttons_speech.png" width="32.5%" />
  <img src="https://github.com/anjelomerte/FamiliAR/blob/main/Images/cubes.png" width="32.5%" /> 
  <img src="https://github.com/anjelomerte/FamiliAR/blob/main/Images/piano.png" width="32.5%" />
</p>

## Features

Supported languages:
- German
- English

The following features are showcased:
- Hologram interaction
	- Triggering buttons
	- Manipulating objects
- Hand-tracking
	- Visualization of virtual hands
- Eye-tracking
	- Playback of video on gaze
- Spatial mapping
	- Visualization
	- Interplay between spatial mesh and objects
- Speech recognition
	- Global speech commands
 
## Setup

1. Clone this repository (or download and unpack .zip)
2. Add project to UnityHub and set specified Unity version (used: 2022.3.9f1) and UWP as target platform
3. Open project. This will take some time as necessary packages will be installed automatically

## Build

1. Open project build settings (File -> Build Settings)
2. Switch platform to Universal Windows if not already selected
3. Set Architecture to ARM 64-bit
4. Set Build and Run to Local Machine or Remote Device depending on your preference
5. Set Build Configuration to Master
6. Build (and run) project on HoloLens 2

## Usage

The user is greeted by a starting dialog which will stay in view of the user until the start button is tapped.
By default, German is set as the application language. To switch to English, raise left hand into view with palm facing towards user. A handmenu will show up for toggling language setting. 

After start is tapped, the demo environment is spawned in front of the user. It consists of 3 main stations: 
1. Piano (to the right)
2. Interactable cubes (center)
3. Buttons (left)
4. Speech recognition (far left)

For each station, a video demonstrates the basic underlying principle and is played on gaze. 
To reposition the AR environment in front of the user again, simply say "position".
Otherwise, have fun exploring:)

## Acknowledgments

This project utilized elements from different scenes included in the Unity MRTK3 Developer Template. For reference see: [MRTK Dev Template](https://github.com/MixedRealityToolkit/MixedRealityToolkit-Unity)

## License

This project is licensed under the MIT License. See [LICENSE.md](https://github.com/anjelomerte/FamiliAR/blob/main/LICENSE.md) for further details.
