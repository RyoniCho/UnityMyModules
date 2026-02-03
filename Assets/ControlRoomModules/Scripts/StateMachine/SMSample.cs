using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMSample : MonoBehaviour
{


    public enum STATE
    {
        IDLE,
        MOVE,
        ATTACK,
    }

    private StateMachine<SMSample,STATE> stateMachine;
    public STATE CurrentState => (STATE)this.stateMachine?.CurrentState;

    private void Awake()
    {
        this.stateMachine = new StateMachine<SMSample,STATE>(this);
        this.stateMachine.RegistState(STATE.IDLE, new IDLE());
        this.stateMachine.RegistState(STATE.MOVE, new MOVE());
        this.stateMachine.RegistState(STATE.ATTACK, new ATTACK());


        if (this.stateMachine != null)
            this.stateMachine.StartStateMachine(STATE.IDLE);

    }


    public class IDLE : State<SMSample,STATE>
    {
        public override void OnEnterState()
        {
            //On Enter Idle State 
        }

        public override void OnExitState()
        {
            //On Exit Idle State 
        }

        public override void OnUpdateState()
        {
            //On Update Idle State

            //Random Transition
            var nextState = this.stateMachine.GetRandomState();
            Debug.Log($"MOVE TO NEXT STATE: {nextState}");
            this.stateMachine.SetState(nextState);


        }
    }

    public class MOVE : State<SMSample,STATE>
    {
        private float moveSpeed = 4;
        public override void OnEnterState()
        {
           
        }

        public override void OnExitState()
        {
           
        }

        public override void OnUpdateState()
        {
            this.behaviour.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    public class ATTACK :State<SMSample,STATE>
    {
        private float maxStateTime = 1f;
        public override void OnEnterState()
        {
            
        }

        public override void OnExitState()
        {
            
        }

        public override void OnUpdateState()
        {
            if (this.stateMachine.CheckElapseTimeToFinsishState(maxStateTime))
            {
                this.stateMachine.SetState(STATE.IDLE);
            }

        }
    }
}
