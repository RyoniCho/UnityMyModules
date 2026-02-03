# Save System Module

유니티 프로젝트에서 게임 로직과 저장 구현(PlayerPrefs, JSON 파일, EasySave 등)을 분리하여 유연하게 사용할 수 있는 모듈입니다.

## 핵심 컴포넌트

### 1. `ISaveSystem` (인터페이스)
모든 저장 방식이 준수해야 할 계약을 정의합니다.
```csharp
public interface ISaveSystem
{
    void Save<T>(string key, T data, string fileName = null);
    T Load<T>(string key, string fileName = null, T defaultValue = default);
    bool Exists(string key, string fileName = null);
    void Delete(string key, string fileName = null);
}
```

### 2. `SaveManager` (싱글톤)
현재 활성화된 `ISaveSystem`을 관리하고 `IStayableData` 객체(상태 저장이 필요한 객체)들을 처리합니다.

*   **접근:** `SaveManager.Instance`
*   **기본값:** 별도 설정이 없으면 `PlayerPrefsSaveSystem`을 사용합니다.

## 기본 제공 구현체

*   **`PlayerPrefsSaveSystem`**: 유니티의 `PlayerPrefs`에 JSON 문자열 형태로 데이터를 저장합니다. 간단하며 모든 플랫폼에서 동작하지만 저장 용량에 제한이 있을 수 있습니다.
*   **`JsonFileSaveSystem`**: `Application.persistentDataPath`에 JSON 파일로 데이터를 저장합니다. `fileName` 매개변수를 통해 여러 파일로 분산 저장이 가능합니다.

## 사용 예시

### 1. 저장 시스템 변경
런타임(예: 게임 초기화 단계)에 저장 방식을 교체할 수 있습니다.

```csharp
void Awake()
{
    // JSON 파일 저장 방식으로 변경
    SaveManager.Instance.SetSaveSystem(new JsonFileSaveSystem());
}
```

### 2. EasySave(ES3) 래퍼 구현 예시
프로젝트에서 EasySave 에셋을 사용한다면, 아래와 같이 래퍼 클래스를 만들어 매니저에 등록하세요.

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

// 등록
SaveManager.Instance.SetSaveSystem(new EasySaveSystem());
```

### 3. 데이터 직접 저장/로드
```csharp
// 저장
SaveManager.Instance.SaveToStorage("PlayerGold", 100);

// 로드
int gold = SaveManager.Instance.LoadFromStorage<int>("PlayerGold", defaultValue: 0);
```

### 4. `IStayableData` 사용 (자동 저장/로드)
플레이어 컨트롤러처럼 스스로 상태를 관리해야 하는 객체에 사용합니다.

1.  `IStayableData` 인터페이스 구현 (Utils에 정의됨).
2.  `SaveManager`에 등록.

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
    
    // ... 인터페이스 메서드 구현 (SaveData, LoadData 등)
}
```
