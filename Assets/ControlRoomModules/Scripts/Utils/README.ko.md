# Utils Module

Control Room 모듈 전반에서 사용되는 공용 유틸리티 클래스, 인터페이스, 헬퍼 함수 모음입니다.

## 내용

### `Utils`
정적(static) 헬퍼 클래스입니다.
*   `CheckLayerMask(LayerMask layerMask, int layer)`: 특정 레이어가 LayerMask에 포함되어 있는지 확인합니다.

### 인터페이스
게임 엔티티를 위한 공통 인터페이스입니다.

*   **`IStayableData`**: `SaveSystem`을 통해 데이터를 저장/로드해야 하는 객체를 위한 인터페이스입니다.
    *   `GetDataSetting()`
    *   `SaveData()`
    *   `LoadData(Data data)`
*   **`IStateMachine`**: `StateMachine` 모듈을 사용하는 객체를 위한 인터페이스입니다.
*   **`IGetHit`**: 피격 가능한(데미지를 입는) 오브젝트의 표준 인터페이스입니다.
*   **`ITalkObject`**: 상호작용 가능한 NPC나 오브젝트를 위한 인터페이스입니다.
*   **`IActionEvent`**: 일반적인 액션 트리거를 위한 인터페이스입니다.

### 데이터 클래스
*   **`Data`, `Data<T>`**: 직렬화 가능한 데이터를 감싸는 제네릭 래퍼입니다.
*   **`DataSetting`**: 저장된 데이터의 설정(권한, 태그)을 관리합니다.
