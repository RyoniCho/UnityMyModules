#if dUI_MANAGER
using System;
using Doozy.Engine;
using Doozy.Engine.UI;
using UnityEngine;

namespace ControlRoom.UI
{
    public class DoozyUIBackend : IUIBackend
    {
        public event Action<UIInputType, string, string> OnUIInputReceived;
        public event Action<string> OnGameEventReceived;

        public DoozyUIBackend()
        {
            // DoozyUI 메시지 리스너 등록
            Message.AddListener<UIButtonMessage>(OnButtonMessage);
            Message.AddListener<GameEventMessage>(OnGameEventMessage);
        }

        ~DoozyUIBackend()
        {
            Message.RemoveListener<UIButtonMessage>(OnButtonMessage);
            Message.RemoveListener<GameEventMessage>(OnGameEventMessage);
        }

        public void ShowView(string category, string name)
        {
            UIView.ShowView(category, name);
        }

        public void HideView(string category, string name)
        {
            UIView.HideView(category, name);
        }

        public void SendGameEvent(string eventName)
        {
            GameEventMessage.SendEvent(eventName);
        }

        private void OnButtonMessage(UIButtonMessage message)
        {
            if (OnUIInputReceived == null) return;

            UIInputType inputType = ConvertToInputType(message.Type);
            string category = message.Button != null ? message.Button.ButtonCategory : "General"; // 기본값 처리
            string name = message.Button != null ? message.Button.ButtonName : message.ButtonName;

            OnUIInputReceived.Invoke(inputType, category, name);
        }

        private void OnGameEventMessage(GameEventMessage message)
        {
            OnGameEventReceived?.Invoke(message.EventName);
        }

        private UIInputType ConvertToInputType(UIButtonBehaviorType doozyType)
        {
            switch (doozyType)
            {
                case UIButtonBehaviorType.OnClick: return UIInputType.OnClick;
                case UIButtonBehaviorType.OnDoubleClick: return UIInputType.OnDoubleClick;
                case UIButtonBehaviorType.OnLongClick: return UIInputType.OnLongClick;
                case UIButtonBehaviorType.OnPointerEnter: return UIInputType.OnPointerEnter;
                case UIButtonBehaviorType.OnPointerExit: return UIInputType.OnPointerExit;
                case UIButtonBehaviorType.OnPointerDown: return UIInputType.OnPointerDown;
                case UIButtonBehaviorType.OnPointerUp: return UIInputType.OnPointerUp;
                default: return UIInputType.Custom;
            }
        }
    }
}
#endif
