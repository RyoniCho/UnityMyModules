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

    private StateMachine<SMSample> stateMachine;
    public STATE CurrentState => (STATE)this.stateMachine?.CurrentState;

    private void Awake()
    {
        this.stateMachine = new StateMachine<SMSample>(this);
        this.stateMachine.RegistState<STATE>(STATE.IDLE, new IDLE());
        this.stateMachine.RegistState<STATE>(STATE.MOVE, new MOVE());
        this.stateMachine.RegistState<STATE>(STATE.ATTACK, new ATTACK());


        if (this.stateMachine != null)
            this.stateMachine.StartStateMachine(STATE.IDLE);

    }


    public class IDLE : State<SMSample>
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
            var nextState = this.stateMachine.GetRandomState<STATE>();
            Debug.Log($"MOVE TO NEXT STATE: {nextState}");
            this.stateMachine.SetState(nextState);


        }
    }

    public class MOVE : State<SMSample>
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

    public class ATTACK :State<SMSample>
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
            if (this.stateMachine.CheckStateElapseTime(maxStateTime))
            {
                this.stateMachine.SetState(STATE.IDLE);
            }

        }
    }
}
