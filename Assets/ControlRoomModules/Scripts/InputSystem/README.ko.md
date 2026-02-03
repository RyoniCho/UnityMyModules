# Input System Module

입력 처리의 추상화 계층을 제공하여, `ControlRoomModules`가 특정 입력 구현(레거시 Input Manager, 새로운 Input System 패키지, 또는 Doozy 같은 서드파티 에셋)에 직접 의존하지 않고 동작할 수 있게 합니다.

## 핵심 컴포넌트

### `IInputHelper`
입력 제어를 위한 규약을 정의하는 인터페이스입니다. 현재는 전역 입력 비활성화 기능을 지원합니다.

```csharp
public interface IInputHelper
{
    bool DisableInput { get; set; }
}
```

### `ControlRoomInput`
Control Room 모듈(예: `SceneHandler`)이 입력 상태를 확인하거나 제어할 때 사용하는 정적 접근자입니다.

## 통합 방법

메인 프로젝트의 Input Manager 스크립트에서 다음을 수행하세요:

1.  `IInputHelper` 인터페이스를 구현합니다.
2.  `Awake` 함수에서 자신의 인스턴스를 `ControlRoomInput.InputHelper`에 할당합니다.

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
