# Pool Manager Module

GameObject를 재사용하여 가비지 컬렉션(GC) 부하를 최소화하는 간단한 오브젝트 풀링 시스템입니다.

## 핵심 컴포넌트

### `PoolManager` (싱글톤)
여러 오브젝트 풀을 관리하는 관리자입니다.

### `Pool`
특정 프리팹에 대한 풀(Pool)을 나타냅니다.

### `IPoolable`
풀링된 오브젝트가 스폰/디스폰 이벤트를 처리하기 위한 인터페이스입니다.
```csharp
public interface IPoolable
{
    void OnSpawn();    // 풀에서 오브젝트가 활성화된 직후 호출됨
    void OnDespawn();  // 오브젝트가 풀로 반환되기 직전에 호출됨
}
```

## 사용 방법

1.  **초기화**: `PoolManager.Instance.CreatePool(prefab, initialSize)`를 호출하여 풀을 생성합니다.
2.  **IPoolable 구현 (선택 사항)**: 오브젝트가 재사용될 때 상태를 초기화해야 한다면 `IPoolable`을 구현합니다.
    ```csharp
    public class Bullet : MonoBehaviour, IPoolable
    {
        public void OnSpawn() { /* 속도, 트레일 등을 초기화 */ }
        public void OnDespawn() { /* 필요 시 정리 로직 */ }
    }
    ```
3.  **스폰**: `Instantiate` 대신 `PoolManager.Instance.ReuseObject(prefab, position, rotation)`을 사용합니다.
3.  **반환(Despawn)**: 오브젝트를 비활성화(`gameObject.SetActive(false)`)하면 자동으로 풀로 반환된 것으로 간주됩니다. (참고: 이 모듈은 보통 오브젝트가 스스로 비활성화 상태를 관리한다고 가정합니다).
