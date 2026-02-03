# Scene Handler Module

씬 전환, 로딩 화면, 그리고 씬 간 플레이어 위치 이동을 관리하는 모듈입니다.

## 핵심 컴포넌트

### `SceneController` (싱글톤)
씬 전환을 총괄하는 매니저입니다.
*   비동기 씬 로딩(`LoadSceneAsync`)을 처리합니다.
*   페이딩 UI(`ScreenFader`)를 제어합니다.
*   전환 중 입력 잠금(`ControlRoomInput` 사용)을 관리합니다.
*   **이벤트**: `processBeforeUnloadingCurrentScene`, `processAfterLoadingNextScene`를 통해 전환 전후 처리를 지원합니다.

### `TransitionPoint`
전환을 시작하는 트리거(예: 문, 포털)에 부착하는 스크립트입니다.
*   **TransitionType**: `DifferentScene`(다른 씬), `SameScene`(같은 씬) 등 전환 타입을 설정합니다.
*   **DestinationTag**: 다음 씬에서 플레이어가 스폰될 위치의 태그를 지정합니다.

### `SceneTransitionDestination`
씬 내의 스폰 포인트(빈 GameObject)에 부착하는 스크립트입니다.
*   **DestinationTag**: `TransitionPoint`의 태그와 매칭되어 입/출구를 연결합니다.

## 사용 방법

1.  **설정**: 초기화 씬에 `SceneController`와 `ScreenFader`가 존재하는지 확인합니다(또는 프리팹을 통해 인스턴스화).
2.  **입구 추가**: 씬에 빈 GameObject를 배치하고 `SceneTransitionDestination`을 부착합니다. `Tag`를 설정합니다 (예: "FromVillage").
3.  **출구 추가**: 트리거 오브젝트(예: 문)를 배치하고 `TransitionPoint`를 부착합니다.
    *   `New Scene Name`에 이동할 씬 이름을 입력합니다.
    *   `Transition Destination Tag`에 대상 씬의 목적지 태그를 입력합니다 (예: "FromDungeon").
