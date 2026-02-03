# Google Sheet CSV Loader

구글 스프레드시트에서 내보낸 CSV 파일을 통해 게임 데이터(스탯, 대화, 아이템 등)를 로드하는 파이프라인입니다.

## 워크플로우

1.  **Google Sheets**: 구글 스프레드시트에서 게임 데이터를 관리합니다.
2.  **Export**: 시트를 `.csv` 파일로 내보냅니다.
3.  **Import**: `.csv` 파일을 `Resources/Data/CSV` (또는 설정된 경로)에 배치합니다.
4.  **Load**: 런타임에 모듈이 CSV를 파싱하여 C# 객체(`TableData`)로 변환합니다.

## 핵심 클래스

*   **`TableDataLoader`**: CSV 텍스트 에셋을 읽어들입니다.
*   **`TableManager`**: 로드된 모든 데이터 테이블에 접근하는 중앙 관리자입니다.
*   **`TableData<T>`**: 데이터의 한 행(Row)을 나타내는 기본 클래스입니다.

## 새로운 데이터 타입 추가

1.  `TableData`를 상속받는 데이터 클래스를 생성합니다 (예: `TEnemyData`).
2.  필요한 경우 매칭되는 로더/빌더 클래스를 생성합니다.
3.  `TableManager`에 새로운 타입을 등록합니다.
