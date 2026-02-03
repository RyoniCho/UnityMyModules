# Input System Module

Provides an abstraction layer for input handling, allowing `ControlRoomModules` to function without direct dependencies on specific input implementations (like the legacy Input Manager, the new Input System package, or 3rd party assets like Doozy).

## Core Components

### `IInputHelper`
Interface that defines the contract for input control. Currently supports disabling input globally.

```csharp
public interface IInputHelper
{
    bool DisableInput { get; set; }
}
```

### `ControlRoomInput`
Static accessor used by Control Room modules (like `SceneHandler`) to check input state.

## Integration

In your main project's Input Manager script:

1.  Implement `IInputHelper`.
2.  Assign your script to `ControlRoomInput.InputHelper` in `Awake`.

```csharp
using ControlRoom;

public class MyInputManager : MonoBehaviour, IInputHelper
{
    public bool DisableInput { get; set; }

    void Awake()
    {
        ControlRoomInput.InputHelper = this;
    }
}
```
