# Save System Module

A flexible, modular save system for Unity that decouples game logic from specific storage implementations (like PlayerPrefs, JSON files, or 3rd party assets like EasySave).

## Core Components

### 1. `ISaveSystem` (Interface)
Defines the contract for any storage mechanism.
```csharp
public interface ISaveSystem
{
    void Save<T>(string key, T data, string fileName = null);
    T Load<T>(string key, string fileName = null, T defaultValue = default);
    bool Exists(string key, string fileName = null);
    void Delete(string key, string fileName = null);
}
```

### 2. `SaveManager` (Singleton)
Manages the active `ISaveSystem` and handles `IStayableData` objects (objects that need to save/load their state).

*   **Access:** `SaveManager.Instance`
*   **Default:** Uses `PlayerPrefsSaveSystem` by default.

## Built-in Implementations

*   **`PlayerPrefsSaveSystem`**: Saves data as JSON strings in Unity's `PlayerPrefs`. Simple, works everywhere, but limited size.
*   **`JsonFileSaveSystem`**: Saves data to JSON files in `Application.persistentDataPath`. Supports multiple "files" (fileName parameter).

## Usage Examples

### 1. Changing the Save System
You can switch the underlying storage method at runtime (e.g., in your Game Initializer).

```csharp
void Awake()
{
    // Switch to JSON File storage
    SaveManager.Instance.SetSaveSystem(new JsonFileSaveSystem());
}
```

### 2. Implementing a Custom Wrapper (e.g., EasySave)
If you use EasySave (ES3) in your project, create a wrapper class and assign it to the manager.

```csharp
using ControlRoom;

public class EasySaveSystem : ISaveSystem
{
    public void Save<T>(string key, T data, string fileName = null)
    {
        if (string.IsNullOrEmpty(fileName)) ES3.Save(key, data);
        else ES3.Save(key, data, fileName);
    }

    public T Load<T>(string key, string fileName = null, T defaultValue = default)
    {
        if (string.IsNullOrEmpty(fileName)) return ES3.Load(key, defaultValue);
        return ES3.Load(key, fileName, defaultValue);
    }

    public bool Exists(string key, string fileName = null)
    {
        if (string.IsNullOrEmpty(fileName)) return ES3.KeyExists(key);
        return ES3.KeyExists(key, fileName);
    }

    public void Delete(string key, string fileName = null)
    {
        if (string.IsNullOrEmpty(fileName)) ES3.DeleteKey(key);
        else ES3.DeleteKey(key, fileName);
    }
}

// Register it
SaveManager.Instance.SetSaveSystem(new EasySaveSystem());
```

### 3. Saving Game Data directly
```csharp
// Save
SaveManager.Instance.SaveToStorage("PlayerGold", 100);

// Load
int gold = SaveManager.Instance.LoadFromStorage<int>("PlayerGold", defaultValue: 0);
```

### 4. Using `IStayableData` (Auto-Save/Load)
For objects that manage their own state (like a PlayerController).

1.  Implement `IStayableData` (defined in `Utils`).
2.  Register with `SaveManager`.

```csharp
public class Player : MonoBehaviour, IStayableData
{
    private void Start()
    {
        SaveManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if(SaveManager.Instance != null)
            SaveManager.Instance.UnRegister(this);
    }
    
    // ... Implement Interface methods (SaveData, LoadData, etc.)
}
```
