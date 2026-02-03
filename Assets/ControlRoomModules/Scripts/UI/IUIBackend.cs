using System;

namespace ControlRoom.UI
{
    /// <summary>
    /// UI 입력 타입 (클릭, 롱클릭 등) - DoozyUI의 BehaviorType과 매핑됨
    /// </summary>
    public enum UIInputType
    {
        OnClick,
        OnDoubleClick,
        OnLongClick,
        OnPointerEnter,
        OnPointerExit,
        OnPointerDown,
        OnPointerUp,
        Custom
    }

    /// <summary>
    /// 실제 UI 라이브러리(Doozy, UGUI 등)와 통신하는 어댑터 인터페이스
    /// </summary>
    public interface IUIBackend
    {
        // 뷰 제어
        void ShowView(string category, string name);
        void HideView(string category, string name);
        
        // 이벤트 전송 (UI -> Game)
        void SendGameEvent(string eventName);
        
        // 이벤트 리스너 (Game/Input -> UI)
        event Action<UIInputType, string, string> OnUIInputReceived; // type, category, name
        event Action<string> OnGameEventReceived; // eventName
    }
}
