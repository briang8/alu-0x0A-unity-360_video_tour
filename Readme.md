# Extended Immersive Tour

A Meta Quest 2 VR experience built in Unity 6 with the XR Interaction Toolkit. It extends the ALU Intranet 360 Video Tour into a three-scene application: a welcome menu, the original Intranet tour, and a custom campus tour built from original 360 photos.

## Contents

- [What it does](#what-it-does)
- [Scenes](#scenes)
- [Navigation flow](#navigation-flow)
- [How it is built](#how-it-is-built)
- [Scripts](#scripts)
- [Project structure](#project-structure)
- [VR comfort and readability](#vr-comfort-and-readability)
- [The 360 captures](#the-360-captures)
- [Credits](#credits)

## What it does

Users start at a welcome menu floating in front of them and select one of two experiences with a controller ray. The Intranet tour plays 360 videos of four rooms with hotspots, points of interest, and background audio. The campus tour presents three 360 photo stops, with forward and back navigation between locations. From either tour, users can jump to the other tour or return to the menu, and every scene or stop change fades through black.

## Scenes

| Scene | Build index | Purpose |
| --- | --- | --- |
| `MainMenuScene` | 0 | Entry point. Title panel with two buttons, placed in front of the user at start. |
| `IntranetTourScene` | 1 | The original Intranet project, renamed. 360 video rooms, hotspots, room switching, POIs, background audio, and fades. A nav bar was added for menu and campus navigation. |
| `CustomCampusTourScene` | 2 | Three original 360 photo stops, each on its own sphere, with forward and back hotspots and a nav bar. |

## Navigation flow

```
                 MainMenuScene
                  /         \
                 v           v
   IntranetTourScene <---> CustomCampusTourScene
                 \           /
                  \         /
                   v       v
                 MainMenuScene
```

- Main menu: Intranet Tour, Custom Campus Tour
- Intranet tour: Main Menu, Campus Tour
- Campus tour: Main Menu, Intranet Tour

Inside the campus tour, hotspots move between stops in walking order: Collaboration Space, Classroom, Study Room. The arrow ahead goes to the next stop, the arrow behind goes back to the previous one.

## How it is built

**360 photos as spheres.** The campus tour follows the same approach as the Intranet tour. Each stop is an inside out sphere (`Perfect360`) scaled to 100 with the photo applied through a URP Unlit material. Only one stop object is active at a time, so switching stops means enabling one sphere and its hotspots while disabling the others. This is the same idea as `RoomSwitcher` in the Intranet scene, where each room is a sphere with a video player.

**Hotspots.** Every hotspot is a small world space canvas holding a button, using the same sprite as the Intranet hotspots. They sit inside the room, about 1.5 m from the centre at roughly eye height, and are turned to face the viewer. Pointing at one scales it up, and pressing it fades to the target stop.

**Transitions.** `SceneFader` is a persistent object that survives scene loads. It renders a black canvas in front of the XR camera, so it also works in the headset, where a Screen Space Overlay canvas is not drawn. Scene changes fade out, load asynchronously, then fade in. Stop changes inside the campus tour use the same fade with the photo swapped at the darkest point.

**UI input in VR.** Controller rays only reach UI when the EventSystem uses the XR UI Input Module and each world space canvas has a Tracked Device Graphic Raycaster. `XRUISetup` enforces both at runtime, which is also what fixed the Intranet hotspots that could not be clicked.

**Panel placement.** The menu is positioned in front of the user once, shortly after the scene starts. Navigation bars use their authored scene positions. Nothing tracks the head while looking around, which keeps the UI stable and predictable.

## Scripts

Original Intranet scripts, in `Assets/scripts`:

| Script | Role |
| --- | --- |
| `RoomSwitcher` | Activates one video room at a time and fades between them. Also makes sure the XR UI input module and tracked raycasters are in place. |
| `HotspotButton` | Room change hotspot with hover scaling. |
| `InfoBoxToggle` | Fades a point of interest panel in and out. |

Extended tour scripts, in `Assets/ExtendedTour/Scripts`:

| Script | Role |
| --- | --- |
| `SceneFader` | Persistent fade for scene loads and stop changes. |
| `SceneLoadButton` | Put on a UI button to open a scene through the fader. |
| `PanoramaTour` | Holds the campus stops, switches the active one, updates the location label. |
| `PanoramaHotspot` | Hotspot that moves the tour to a target stop, with a gentle pulse and hover feedback. |
| `HeadAnchoredPanel` | Places a panel in front of the user once at scene start. |
| `XRUISetup` | Guarantees XR UI input module and tracked raycasters in the scene. |

## Project structure

```
Assets/
  ExtendedTour/
    Materials/    sphere and skybox materials
    Panoramas/    the three campus 360 photos and the menu background
    Scripts/      fader, navigation, campus tour, comfort UI
  Hotspots/       hotspot sprites shared by both tours
  Prefabs/        Perfect360 sphere
  Scenes/         MainMenuScene, IntranetTourScene, CustomCampusTourScene
  scripts/        original Intranet scripts
  Textures/       render textures and materials for the 360 videos
  Videos/         Intranet 360 videos (Git LFS)
Packages/
ProjectSettings/
```

Generated Unity caches, temporary folders, local editor settings, build outputs, crash reports, and IDE project files are excluded from version control. Media and project assets remain part of the source project, while platform packages, signing keys, and other local build artifacts are not tracked.

## VR comfort and readability

- Every transition fades to black and back, so nothing cuts abruptly
- The viewer never moves on their own, no artificial locomotion in the tours
- Panels are placed once and then stay still, so the world does not shift while looking around
- The nav bar sits below eye level and is tilted up, clear of the main view
- Large type with strong contrast, and buttons big enough to hit comfortably with a ray
- Stops run in walking order so the space stays understandable

## The 360 captures

All three campus panoramas use the campus 360 camera and are presented unedited at 6080 x 3040. The stops are a collaboration space upstairs by the stairs, a classroom, and a study room with a balcony.

## Credits

- 360 campus photos: captured with the ALU 360 camera
- Intranet 360 videos and hotspot artwork: provided by the course
- XR Interaction Toolkit, XR Hands and the Starter Assets sample: Unity Technologies
