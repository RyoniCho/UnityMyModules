# Utils Module

A collection of shared utility classes, interfaces, and helper functions used across Control Room Modules.

## Contents

### `Utils`
Static helper class.
*   `CheckLayerMask(LayerMask layerMask, int layer)`: Checks if a specific layer is included in a LayerMask.

### Interfaces
Common interfaces for game entities.

*   **`IStayableData`**: For objects that need to save/load data via the `SaveSystem`.
    *   `GetDataSetting()`
    *   `SaveData()`
    *   `LoadData(Data data)`
*   **`IStateMachine`**: For objects using the `StateMachine` module.
*   **`IGetHit`**: Standard interface for damageable objects.
*   **`ITalkObject`**: For interactable NPCs or objects.
*   **`IActionEvent`**: For triggering generic actions.

### Data Classes
*   **`Data`, `Data<T>`**: Generic wrappers for serializable data.
*   **`DataSetting`**: Configuration for saved data (Permission, Tag).
