using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StateMachine<Tbehaviour,TeState> where Tbehaviour:MonoBehaviour where TeState :System.Enum
{

    State<Tbehaviour,TeState> currentState;
    State<Tbehaviour,TeState> nextState;
    int currentStateNum;
    int nextStateNum;
    bool stateLock = false;
  

    Tbehaviour behaviour;
    float stateElapseTime = 0f;

    public int CurrentState => currentStateNum;

    public float StateElapseTime => stateElapseTime;
    

    public StateMachine(Tbehaviour _behaviour) 
    {
        this.behaviour = _behaviour;
    }
        
    Dictionary<int, State<Tbehaviour,TeState>> states = new Dictionary<int, State<Tbehaviour,TeState>>();

    public void RegistState(TeState eState, State<Tbehaviour,TeState> stateMachine) 
    {
        
        var stateNum= (int)(System.IConvertible)eState;

        if (!states.ContainsKey(stateNum))
        {
            states.Add(stateNum, stateMachine);
            stateMachine.InitialState(this.behaviour,this);
           
        }
          
    }

    public void StartStateMachine(TeState firstState)
    {
        State<Tbehaviour,TeState> initialState;
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

        stateLock = false;


    }

    public void StopStateMachine()
    {
        this.behaviour.StopCoroutine(UpdateStateMachine());
       
    }

    public void SetState(TeState eState,bool isLock=false)
    {
      
        var stateNum = (int)(System.IConvertible)eState;

        if (stateLock)
            return;
       
        
        if(states.ContainsKey(stateNum))
        {
            var next = states[stateNum];
            if(next!=currentState)
            {
                this.nextState = next;
                this.nextStateNum = stateNum;

                stateLock = isLock;
            }
        }
    }

   
    public void SetStateForce(TeState eState,bool withReleaseLock=false)
    {
        var stateNum = (int)(System.IConvertible)eState;


        if (states.ContainsKey(stateNum))
        {
            var next = states[stateNum];
            if (next != currentState)
            {
                this.nextState = next;
                this.nextStateNum = stateNum;
                if (withReleaseLock)
                    stateLock = false;
            }
        }
    }


    public bool CheckElapseTimeToFinsishState(float maxTime)
    {
       
        if (maxTime < this.stateElapseTime)
            return true;

        return false;
    }


    IEnumerator UpdateStateMachine()
    {
        while(true)
        {
            this.stateElapseTime += Time.deltaTime;

         

            if(nextState!=currentState)
            {
                currentState.OnExitState();
                nextState.OnEnterState();

                currentState = nextState;
                currentStateNum = nextStateNum;
                this.stateElapseTime = 0f;
            }
            else
            {
                currentState.OnUpdateState();
            }

            yield return null;

            
        }
       
    }

    public TeState GetRandomState() 
    {
        int randNum = 0;

        while (true)
        {
            randNum = UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(TeState)).Length);
            if (randNum != currentStateNum)
                break;

        }

        return (TeState)(System.IConvertible)randNum;

    }
}

public abstract class State<TBehaviour,TeState> where TBehaviour :MonoBehaviour where TeState :System.Enum
{
    protected TBehaviour behaviour;
    protected StateMachine<TBehaviour,TeState> stateMachine;
    private List<Transition<TeState>> transitions;
    public void InitialState(TBehaviour behaviour, StateMachine<TBehaviour,TeState> stateMachine)
    {
        this.behaviour = behaviour;
        this.stateMachine = stateMachine;
    }

    public void AddTransition(Func<bool> condition, TeState targetState)
    {
        transitions.Add(new Transition<TeState>(condition,targetState));
    }

    public void CheckTransitions()
    {
        if (transitions.Count <= 0)
        {
            Debug.LogError($"State Transition is Empty! Check it out");
            return;
        }

        foreach (var tr in transitions)
        {
            if (tr.Condition())
            {
                this.stateMachine.SetState(tr.TargetState);
                break;
            }
        }
          
        
    }
    
    public abstract void OnEnterState();

    public abstract void OnUpdateState();

    public abstract void OnExitState();

}

public class Transition<TeState> where TeState : System.Enum
{
    public Func<bool> Condition { get; }
    public TeState TargetState { get; }
    
    public Transition(Func<bool> condition, TeState targetState)
    {
        Condition = condition;
        TargetState = targetState;
    }

}

