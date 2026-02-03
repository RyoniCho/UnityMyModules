# State Machine Module

캐릭터 컨트롤러나 로직 시스템을 위한 가벼운 유한 상태 머신(FSM) 구현체입니다.

## 핵심 컴포넌트

### `StateMachine`
상태와 전환을 관리하는 기본 클래스입니다.

### `StateMachineController`
상태 실행 순서를 관리하는 헬퍼 구조체입니다.

### `IStateMachine`
이 상태 머신을 사용하는 객체가 구현해야 하는 인터페이스입니다.

## 사용 방법

1.  특정 로직 클래스(예: `PlayerStateMachine`)에서 `StateMachine`을 상속받습니다.
2.  상태(Enum 또는 클래스)를 정의합니다.
3.  각 상태에 대한 `OnEnter`, `OnUpdate`, `OnExit` 로직을 구현합니다.
