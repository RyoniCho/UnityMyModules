# ControlRoom UI Module

이 모듈은 특정 UI 라이브러리(DoozyUI, UGUI 등)에 종속되지 않는 **상태 기반(State-based) UI 프레임워크**입니다.

---

## 1. 기본 설정 (Setup)

### UIManager 생성
프로젝트의 UI를 총괄할 매니저를 생성합니다. `BaseUIManager<T>`를 상속받습니다.

```csharp
using ControlRoom.UI;

public class MyGameUIManager : BaseUIManager<MyGameUIManager>
{
    protected override IUIBackend CreateUIBackend()
    {
        // DoozyUI 사용 시
        #if dUI_MANAGER
        return new DoozyUIBackend();
        #else
        // UGUI 사용 시 직접 구현한 Backend 반환 (아래 IUIBackend 구현 참조)
        return new MyUGUIBackend(); 
        #endif
    }

    protected override void RegisterStates()
    {
        // 사용할 State 등록 (true는 시작 State)
        uiStateMachine.RegistState(new TitleState(), true);
        uiStateMachine.RegistState(new InGameState());
        uiStateMachine.RegistState(new InventoryState());
        uiStateMachine.RegistState(new PopupState());
    }
}
```

---

## 2. UI 화면 만들기 (State)

각 UI 화면(패널)은 하나의 `UIState`로 정의됩니다.

### 기본 화면 (View)
```csharp
public class TitleState : UIState
{
    public override string Name => "TitleMenu";

    public override void OnEnter()
    {
        base.OnEnter();
        // Backend에 "General" 카테고리의 "TitleView"를 보여달라고 요청
        uiBackend.ShowView("General", "TitleView");
    }

    public override void OnExit()
    {
        base.OnExit();
        uiBackend.HideView("General", "TitleView");
    }

    protected override void AddOutputConnections(List<Connection> listConnections)
    {
        // StartButton 클릭 -> InGame 상태로 전환
        listConnections.Add(new ButtonConnection(
            UIInputType.OnClick, "General", "StartButton", "InGame"
        ));
    }
}
```

### 팝업 (Popup) - Stack 구조
팝업은 현재 화면을 유지한 채 위에 쌓이는 구조입니다. `PushState`와 `PopState`를 사용합니다.

```csharp
// 1. 팝업을 띄우는 연결 (이전 State에서 설정)
listConnections.Add(new ButtonConnection(
    UIInputType.OnClick, "General", "OpenOption", "OptionPopup", 
    isPush: true // 중요: Stack에 Push함
));

// 2. 팝업 State 구현
public class OptionPopupState : UIState
{
    public override string Name => "OptionPopup";

    public override void OnEnter()
    {
        base.OnEnter();
        uiBackend.ShowView("Popup", "OptionWindow");
    }

    public override void OnExit()
    {
        base.OnExit();
        uiBackend.HideView("Popup", "OptionWindow");
    }

    protected override void AddOutputConnections(List<Connection> listConnections)
    {
        // 닫기 버튼 -> Stack에서 Pop (이전 화면으로 복귀)
        listConnections.Add(new ButtonConnection(
            UIInputType.OnClick, "Popup", "CloseBtn", "", 
            isPop: true // 중요: Stack에서 제거함
        ));
    }
}
```

---

## 3. 동적 리스트 만들기 (ScrollView)

인벤토리, 랭킹, 파일 목록 등 데이터에 따라 개수가 변하는 UI를 만들 때 사용합니다.

### 1단계: 데이터 클래스 정의
리스트 아이템에 전달할 데이터를 정의합니다.
```csharp
public class ItemData
{
    public string ItemName;
    public int Count;
    public Sprite Icon;
}
```

### 2단계: 리스트 아이템 스크립트 (BaseUIListItem 상속)
개별 아이템 프리팹에 붙을 스크립트입니다.
```csharp
using ControlRoom.UI;
using TMPro;
using UnityEngine.UI;

public class InventoryItem : BaseUIListItem<ItemData>
{
    public TextMeshProUGUI nameText;
    public Image iconImage;

    // 데이터가 변경될 때 UI 갱신 로직
    protected override void RefreshUI()
    {
        if (data == null) return;
        nameText.text = $"{data.ItemName} ({data.Count})";
        iconImage.sprite = data.Icon;
    }
}
```

### 3단계: 리스트 컨트롤러 스크립트 (BaseUIListController 상속)
스크롤뷰를 관리하는 스크립트입니다.
```csharp
using ControlRoom.UI;

public class InventoryController : BaseUIListController<InventoryItem, ItemData>
{
    // 인스펙터에서 itemPrefabName을 설정해야 함 (PoolManager에 등록된 이름)
    
    public void UpdateInventory(List<ItemData> myItems)
    {
        // 부모 클래스의 메서드 호출 -> 자동으로 풀링 및 아이템 생성
        SetListItems(myItems);
    }
}
```

### 4단계: State에서 사용
```csharp
public class InventoryState : UIState
{
    public override string Name => "Inventory";

    public override void OnEnter()
    {
        base.OnEnter();
        uiBackend.ShowView("General", "InventoryView");
        
        // 데이터 로드 및 리스트 갱신
        var data = DataManager.Instance.GetItems();
        // 씬에 있는 컨트롤러를 찾아 갱신 (싱글톤이나 참조로 접근 권장)
        GameObject.FindObjectOfType<InventoryController>().UpdateInventory(data);
    }
    ...
}
```

---

## 4. IUIBackend 커스텀 구현 (UGUI 예시)

DoozyUI 없이 순수 UGUI를 사용할 경우, `IUIBackend`를 구현하여 연결합니다.

```csharp
public class MyUGUIBackend : IUIBackend
{
    public event Action<UIInputType, string, string> OnUIInputReceived;
    public event Action<string> OnGameEventReceived;

    // 뷰 이름과 실제 프리팹/오브젝트 매핑 관리 필요
    private Dictionary<string, GameObject> viewCache = new Dictionary<string, GameObject>();
    private Transform canvasRoot;

    public void ShowView(string category, string name)
    {
        // 1. 이미 생성된 뷰가 있다면 활성화
        if (viewCache.TryGetValue(name, out var view))
        {
            view.SetActive(true);
            return;
        }

        // 2. 없다면 Resources/Addressables에서 로드하여 인스턴스화 (동적 생성)
        var prefab = Resources.Load<GameObject>($"UI/{category}/{name}");
        if (prefab != null)
        {
            var instance = GameObject.Instantiate(prefab, canvasRoot);
            viewCache.Add(name, instance);
            
            // 버튼 이벤트 자동 연결 로직 필요 (예: 컴포넌트 찾아서 OnClick에 연결)
            // instance.GetComponentInChildren<Button>().onClick.AddListener(() => 
            //     OnUIInputReceived?.Invoke(UIInputType.OnClick, category, name));
        }
    }

    public void HideView(string category, string name)
    {
        if (viewCache.TryGetValue(name, out var view))
        {
            view.SetActive(false);
        }
    }

    public void SendGameEvent(string eventName)
    {
        // 게임 내 이벤트 시스템으로 전달
    }
}
```
