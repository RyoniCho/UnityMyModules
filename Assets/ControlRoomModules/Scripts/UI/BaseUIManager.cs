using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom.UI
{
    /// <summary>
    /// UI 상태 머신을 관리하는 기본 UIManager 클래스입니다.
    /// 프로젝트별 UIManager는 이 클래스를 상속받아 구현하면 됩니다.
    /// </summary>
    public abstract class BaseUIManager<T> : SingletonBase<T> where T : Component
    {
        protected UIStateMachine uiStateMachine;
        protected IUIBackend uiBackend;

        protected override void Awake()
        {
            base.Awake();
            InitializeUI();
        }

        protected virtual void InitializeUI()
        {
            // 1. UI Backend 설정 (프로젝트 상황에 맞게 선택)
            uiBackend = CreateUIBackend();

            // 2. State Machine 생성
            uiStateMachine = new UIStateMachine(this, uiBackend);

            // 3. State 등록
            RegisterStates();

            // 4. State Machine 시작
            uiStateMachine.StartStateMachine();
        }

        /// <summary>
        /// 사용할 UI Backend를 생성하여 반환합니다.
        /// 예: return new DoozyUIBackend();
        /// </summary>
        protected abstract IUIBackend CreateUIBackend();

        /// <summary>
        /// 사용할 UI State들을 등록합니다.
        /// 예: uiStateMachine.RegistState(new TitleState(), true);
        /// </summary>
        protected abstract void RegisterStates();

        protected virtual void OnDisable()
        {
            uiStateMachine?.StopStateMachine();
        }
    }
}
