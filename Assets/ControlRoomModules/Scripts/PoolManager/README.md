# Pool Manager Module

A simple object pooling system to reuse GameObjects and minimize garbage collection overhead.

## Core Components

### `PoolManager` (Singleton)
Manages a collection of object pools.

### `Pool`
Represents a pool for a specific prefab.

### `IPoolable`
Interface for pooled objects to handle spawn/despawn events.
```csharp
public interface IPoolable
{
    void OnSpawn();    // Called immediately after the object is activated from the pool
    void OnDespawn();  // Called immediately before the object is returned to the pool
}
```

## Usage

1.  **Initialize**: Call `PoolManager.Instance.CreatePool(prefab, initialSize)` to set up a pool.
2.  **Implement IPoolable (Optional)**: If your object needs to reset its state when reused, implement `IPoolable`.
    ```csharp
    public class Bullet : MonoBehaviour, IPoolable
    {
        public void OnSpawn() { /* Reset velocity, trail, etc. */ }
        public void OnDespawn() { /* Clean up if needed */ }
    }
    ```
3.  **Spawn**: Use `PoolManager.Instance.ReuseObject(prefab, position, rotation)` instead of `Instantiate`.
3.  **Despawn**: Deactivate the object (`gameObject.SetActive(false)`) to return it to the pool. (Note: This module typically relies on the object managing its own deactivation state).
