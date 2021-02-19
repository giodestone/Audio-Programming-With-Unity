# Audio Programming with Unity
![GIF of walking around](https://raw.githubusercontent.com/giodestone/Audio-Programming-With-Unity/main/Images/GIF2.gif)

Game which integrates prerecorded & synthesized audio effects and area-aware music into the Unity audio system by providing reusable components. Made as coursework for a module in University.

## Running
[Download](https://github.com/giodestone/Audio-Programming-With-Unity/releases)

### Controls
* W, A, S, D - Walk around.
* Mouse - Look around
* Left Mouse Button - Interact
* Alt + F4 - Exit

### Interactions
![Cooker Image](https://raw.githubusercontent.com/giodestone/Audio-Programming-With-Unity/main/Images/Image2.jpg)

- Bed: Changes outside sound and plays snore sound.
- Drink glasses: Add/clear sound effects.
- Cooker: Knob and oven sound.
- Moving between inside and outside: Indoor the outside effects are muffled and different music tracks are paused/played.
- Fridge: Plays sounds randomly.
- Footsteps: Grass, wood, and path.

## Using Source Code
Only open in Unity 2020.1.x as 2020.2.x breaks the audio mixers and general audio playback.

## Implementation
![Image of House](https://raw.githubusercontent.com/giodestone/Audio-Programming-With-Unity/main/Images/Image1.jpg)

The game aimed to integrate with the Unity audio system as possible. For that reason it uses the Audio Source/Audio Listener components to emit/'listen' to audio. Audio Mixers are used to separate the different sounds into easily modifiable groups, on which Audio Snapshots (namely low/high pass, delay, pitch shift) are applied depending on the location of the player (inside/outside) and whether they are intoxicated (in which case a high pitched noise simulating tinnitus is procedurally generated inside the overridden`OnBufferRead()` method).

In addition, the project attempts to create reusable and easy to integrate into the project using the editor.

*For a more aesthetically pleasing rundown of these features, please see the [presentation](https://raw.githubusercontent.com/giodestone/Audio-Programming-With-Unity/main/Presentation.pdf).*

### Reactive Music
Depending on which audio zone the player is located in (which is changed by a trigger collider), the outdoor music is then cross faded with the indoor one. The outgoing loop is 'paused' (position in playback kept) then resumed when played again.

### Procedural Audio Generation
The `OnBufferRead()` method is used to generate the high pitched noise. This involves generating a sine wave and copying it across the dual audio channels (which are interweaved). Control of the sound is done via atomics (simple data types in C# are atomic by default).

### Recording & Editing Sound Effects
![Cabin at night](https://raw.githubusercontent.com/giodestone/Audio-Programming-With-Unity/main/Images/GIF1.gif)

Some sound effects (oven sound, cooker & oven knobs) were recorded by me. All the sound effects were then processed using Audacity, where they were:
1. Trimmed to size (with care taken to make sure the start/end of clip is crossing at zero to avoid pops).
2. Normalized to integrate with spatial audio better.
3. Faded in/out where applicable.

The oven sound also has a noise reduction profile reduced due to background noise at high gain levels.


## Credits
The project was largely coded by me using the HDRP template. The icon was sourced from Google Material Icons under Apache 2.0. The character movement code was sourced from [here](https://assetstore.unity.com/packages/tools/input-management/mini-first-person-controller-174710)

### Sound Effects
Footsteps Wood (own processing) - https://www.soundjay.com/footsteps-1.html
Footsteps Concrete - https://www.premiumbeat.com/blog/40-free-footstep-foley-sound-effects/
Footsteps Grass (own processing) - Footsteps-in-grass-moderate-A-www.fesliyanstudios.com

The snoring and fridge sounds were sourced from BBC's SFX archive, which are used under license. These sound effects are copyright BBC.

The outdoor day/night sounds were gotten from [here](https://www.freetousesounds.com/product/vietnam-sound-library/).

### Music
Music was provided by [Kevin Macleod at incompetech.com](https://incompetech.com/music/royalty-free/)

