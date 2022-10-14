using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<Tbehaviour> where Tbehaviour:MonoBehaviour
{

    State<Tbehaviour> currentState;
    State<Tbehaviour> nextState;
    int currentStateNum;
    int nextStateNum;

    Tbehaviour behaviour;
    float stateElapseTime = 0f;

    public int CurrentState => currentStateNum;
    

    public StateMachine(Tbehaviour _behaviour) 
    {
        this.behaviour = _behaviour;
    }
        
    Dictionary<int, State<Tbehaviour>> states = new Dictionary<int, State<Tbehaviour>>();

    public void RegistState<TeState>(TeState eState, State<Tbehaviour> stateMachine) where TeState : System.Enum
    {
        
        var stateNum= (int)(System.IConvertible)eState;

        if (!states.ContainsKey(stateNum))
        {
            states.Add(stateNum, stateMachine);
            stateMachine.InitialState(this.behaviour,this);
           
        }
          
    }

    public void StartStateMachine<TeState>(TeState firstState) where TeState :System.Enum
    {
        State<Tbehaviour> initialState;
        var stateNum = (int)(System.IConvertible)firstState;

        if (states.TryGetValue(stateNum, out initialState))
        {
            currentState = initialState;
            nextState = currentState;
            currentStateNum = stateNum;
            currentState.OnEnterState();

            this.behaviour.StartCoroutine(UpdateStateMachine());
        }
        else
        {
            Debug.LogError($"Could not find {firstState.ToString()}. Register State first");
        }
       

    }

    public void StopStateMachine()
    {
        this.behaviour.StopCoroutine(UpdateStateMachine());
       
    }

    public void SetState<TeState>(TeState eState) where TeState : System.Enum
    {
        var stateNum = (int)(System.IConvertible)eState;

       
        if(states.ContainsKey(stateNum))
        {
            var next = states[stateNum];
            if(next!=currentState)
            {
                this.nextState = next;
                this.nextStateNum = stateNum;
               
            }
        }
    }

    public bool CheckStateElapseTime(float maxTime)
    {
       
        if (maxTime >= this.stateElapseTime)
            return true;

        return false;
    }


    IEnumerator UpdateStateMachine()
    {
        while(true)
        {
            this.stateElapseTime += Time.deltaTime;

            currentState.OnUpdateState();

            if(nextState!=currentState)
            {
                currentState.OnExitState();
                nextState.OnEnterState();

                currentState = nextState;
                currentStateNum = nextStateNum;
                this.stateElapseTime = 0f;
            }

            yield return null;

            
        }
       
    }

    public TeState GetRandomState<TeState>() where TeState : System.Enum
    {
        int randNum = 0;

        while (true)
        {
            randNum = Random.Range(0, System.Enum.GetValues(typeof(TeState)).Length);
            if (randNum != currentStateNum)
                break;

        }

        return (TeState)(System.IConvertible)randNum;

    }
}

public abstract class State<TBehaviour> where TBehaviour :MonoBehaviour
{
    protected TBehaviour behaviour;
    protected StateMachine<TBehaviour> stateMachine;
    public void InitialState(TBehaviour behaviour, StateMachine<TBehaviour> stateMachine)
    {
        this.behaviour = behaviour;
        this.stateMachine = stateMachine;
    }
    
    public abstract void OnEnterState();

    public abstract void OnUpdateState();

    public abstract void OnExitState();

}

