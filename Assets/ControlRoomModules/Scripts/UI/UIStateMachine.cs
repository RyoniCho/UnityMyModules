using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom.UI
{
    public abstract class UIState 
    {
        public abstract string Name { get; }
        protected UIStateMachine stateMachine;
        protected IUIBackend uiBackend;
        
        // 연결 정보 저장
        protected List<Connection> connections = new List<Connection>();

        public virtual void Init(UIStateMachine uiStateMachine, IUIBackend backend)
        {
            this.stateMachine = uiStateMachine;
            this.uiBackend = backend;
            AddOutputConnections(this.connections);
        }

        public virtual void OnEnter()
        {
            Debug.Log($"[UI] Enter State: {Name}");
        }

        public virtual void OnExit()
        {
            Debug.Log($"[UI] Exit State: {Name}");
        }

        public virtual void OnUpdate() { }

        // 입력 처리 (StateMachine에서 호출해줌)
        public virtual void HandleInput(UIInputType type, string category, string name)
        {
            foreach (var connection in connections)
            {
                if (connection is ButtonConnection btnConn)
                {
                    if (btnConn.InputType == type && 
                        btnConn.ButtonCategory == category && 
                        btnConn.ButtonName == name)
                    {
                        TransitionTo(btnConn);
                        return;
                    }
                }
            }
        }

        public virtual void HandleGameEvent(string eventName)
        {
            foreach (var connection in connections)
            {
                if (connection is GameEventConnection evtConn)
                {
                    if (evtConn.EventName == eventName)
                    {
                        TransitionTo(evtConn);
                        return;
                    }
                }
            }
        }

        protected void TransitionTo(Connection connection)
        {
            if (connection.IsPush)
            {
                stateMachine.PushState(connection.NextStateName);
            }
            else if (connection.IsPop)
            {
                stateMachine.PopState();
            }
            else
            {
                stateMachine.SetState(connection.NextStateName);
            }
        }

        protected abstract void AddOutputConnections(List<Connection> listConnections);

        // --- Connection Classes ---
        public class Connection
        {
            public string NextStateName;
            public bool IsPush = false; // 스택에 추가?
            public bool IsPop = false;  // 스택에서 제거? (Back)
        }

        public class ButtonConnection : Connection
        {
            public UIInputType InputType { get; }
            public string ButtonCategory { get; }
            public string ButtonName { get; }

            public ButtonConnection(UIInputType type, string category, string name, string nextState, bool isPush = false, bool isPop = false)
            {
                InputType = type;
                ButtonCategory = category;
                ButtonName = name;
                NextStateName = nextState;
                IsPush = isPush;
                IsPop = isPop;
            }
        }

        public class GameEventConnection : Connection
        {
            public string EventName { get; }

            public GameEventConnection(string eventName, string nextState, bool isPush = false, bool isPop = false)
            {
                EventName = eventName;
                NextStateName = nextState;
                IsPush = isPush;
                IsPop = isPop;
            }
        }
    }

    public class UIStateMachine
    {
        private Dictionary<string, UIState> states = new Dictionary<string, UIState>();
        private Stack<UIState> stateStack = new Stack<UIState>(); // 팝업 처리를 위한 스택
        
        private UIState currentState;
        private UIState nextState;
        
        private MonoBehaviour coroutineRunner;
        private IUIBackend uiBackend;
        private string startStateStr = string.Empty;

        public UIState CurrentState => currentState;

        public UIStateMachine(MonoBehaviour runner, IUIBackend backend)
        {
            this.coroutineRunner = runner;
            this.uiBackend = backend;

            // 백엔드 이벤트 구독
            this.uiBackend.OnUIInputReceived += OnUIInput;
            this.uiBackend.OnGameEventReceived += OnGameEvent;
        }

        ~UIStateMachine()
        {
            if (this.uiBackend != null)
            {
                this.uiBackend.OnUIInputReceived -= OnUIInput;
                this.uiBackend.OnGameEventReceived -= OnGameEvent;
            }
        }

        public void RegistState(UIState uiState, bool isStartState = false)
        {
            if (!states.TryAdd(uiState.Name, uiState))
            {
                Debug.LogError($"UI State Error : duplicate state {uiState.Name}");
                return;
            }

            uiState.Init(this, uiBackend);

            if (isStartState)
            {
                this.startStateStr = uiState.Name;
                // 초기 상태 설정 시 currentState도 바로 설정해줘야 함
                // SetState는 nextState만 설정하고 코루틴에서 전환하므로,
                // StartStateMachine 호출 전까지 currentState는 null일 수 있음.
                // 하지만 StartStateMachine에서 currentState.OnEnter()를 호출하려면
                // 여기서 currentState를 설정해두거나, StartStateMachine에서 처리해야 함.
                
                // 기존 로직 복원:
                this.currentState = uiState;
                this.nextState = uiState;
                
                // 스택 초기화
                stateStack.Clear();
                stateStack.Push(uiState);
            }
        }

        public void StartStateMachine()
        {
            if (string.IsNullOrEmpty(this.startStateStr))
            {
                Debug.LogError("Not Exist Start State");
                return;
            }
            
            // currentState가 null이면 startState로 설정
            if (currentState == null && states.TryGetValue(startStateStr, out var startState))
            {
                currentState = startState;
                nextState = startState;
                stateStack.Clear();
                stateStack.Push(startState);
            }

            if (currentState != null)
            {
                currentState.OnEnter();
                if (coroutineRunner != null)
                    coroutineRunner.StartCoroutine(UpdateState());
            }
        }

        public void StopStateMachine()
        {
            if (coroutineRunner != null)
                coroutineRunner.StopCoroutine(UpdateState());
        }
        
        public void ResetToDefaultState()
        {
            if (currentState != null)
                currentState.OnExit();
        
            if (!string.IsNullOrEmpty(this.startStateStr))
            {
                this.SetState(this.startStateStr);
            }
        }

        // 일반적인 상태 전환 (교체)
        public void SetState(string stateName)
        {
            if (states.TryGetValue(stateName, out var newState))
            {
                // 스택 초기화 후 전환 (루트 변경)
                stateStack.Clear();
                stateStack.Push(newState);
                ChangeState(newState);
            }
            else
            {
                Debug.LogError($"Not exist UI state : {stateName}");
            }
        }

        // 팝업 띄우기 (현재 상태 유지하고 위에 쌓음)
        public void PushState(string stateName)
        {
            if (states.TryGetValue(stateName, out var newState))
            {
                // 현재 상태는 Exit 호출 안 함 (Pause 개념이 필요하면 추가 구현)
                stateStack.Push(newState);
                ChangeState(newState);
            }
        }

        // 팝업 닫기 (이전 상태로 복귀)
        public void PopState()
        {
            if (stateStack.Count > 1)
            {
                stateStack.Pop(); // 현재 상태 제거
                var prevState = stateStack.Peek();
                ChangeState(prevState);
            }
            else
            {
                Debug.LogWarning("Cannot Pop State: Stack is empty or only root remains.");
            }
        }

        private void ChangeState(UIState newState)
        {
            nextState = newState;
        }

        private void OnUIInput(UIInputType type, string category, string name)
        {
            currentState?.HandleInput(type, category, name);
        }

        private void OnGameEvent(string eventName)
        {
            currentState?.HandleGameEvent(eventName);
        }

        IEnumerator UpdateState()
        {
            while (true)
            {
                if (currentState != nextState && nextState != null)
                {
                    currentState?.OnExit();
                    currentState = nextState;
                    currentState.OnEnter();
                }
                
                currentState?.OnUpdate();
                yield return null;
            }
        }
    }
}
