# Scene Handler Module

Manages scene transitions, loading screens, and player positioning between scenes.

## Core Components

### `SceneController` (Singleton)
The central manager for scene transitions.
*   Handles asynchronous scene loading (`LoadSceneAsync`).
*   Manages the fading UI (`ScreenFader`).
*   Controls input locking during transitions (via `ControlRoomInput`).
*   **Events**: `processBeforeUnloadingCurrentScene`, `processAfterLoadingNextScene`.

### `TransitionPoint`
Script attached to triggers (e.g., doors, portals) that initiate a transition.
*   **TransitionType**: `DifferentScene`, `SameScene`, etc.
*   **DestinationTag**: Where the player should spawn in the next scene.

### `SceneTransitionDestination`
Script attached to spawn points (empty GameObjects) in a scene.
*   **DestinationTag**: Matches the tag in `TransitionPoint` to link entrances/exits.

## Usage

1.  **Setup**: Ensure `SceneController` and `ScreenFader` are present in your initialization scene (or instantiated via a prefab).
2.  **Add Entrance**: Place an empty GameObject in your scene and attach `SceneTransitionDestination`. Set its `Tag` (e.g., "FromVillage").
3.  **Add Exit**: Place a Trigger object (e.g., a Door) and attach `TransitionPoint`.
    *   Set `New Scene Name` to the target scene.
    *   Set `Transition Destination Tag` to the tag of the destination in that target scene (e.g., "FromDungeon").
