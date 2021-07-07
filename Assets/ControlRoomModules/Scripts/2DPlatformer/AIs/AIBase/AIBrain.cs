using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class AIBrain : MonoBehaviour
    {
        public bool BrainActive = true;

        public List<AIState> states;

        public AIState CurrentState { get; protected set; }

        [Header("Frequencies")]
        /// the frequency (in seconds) at which to perform actions (lower values : higher frequency, high values : lower frequency but better performance)
        public float ActionsFrequency = 0f;
        /// the frequency (in seconds) at which to evaluate decisions
        public float DecisionFrequency = 0f;

        private float lastActionsUpdate = 0f;
        private float lastDecisionsUpdate = 0f;
        private AIDecision[] decisions;
        public float TimeInThisState;
        public Transform Target;


        public virtual AIDecision[] GetAttachedDecisions()
        {
            AIDecision[] decisions = this.gameObject.GetComponentsInChildren<AIDecision>();
            return decisions;
        }

        private void Awake()
        {
            foreach (AIState state in states)
            {
                state.SetBrain(this);
            }

            this.decisions = GetAttachedDecisions();

        }

        private void Update()
        {
            if (!BrainActive || (CurrentState == null) || (Time.timeScale == 0f))
            {
                return;
            }

            if (Time.time - lastActionsUpdate > ActionsFrequency)
            {
                CurrentState.StartActions();
                lastActionsUpdate = Time.time;
            }

            if (Time.time - lastDecisionsUpdate > DecisionFrequency)
            {
                CurrentState.EvaluateTransitions();
                lastDecisionsUpdate = Time.time;
            }

            TimeInThisState += Time.deltaTime;

        }

        private void Start()
        {
            ResetBrain();
        }


        public void ResetBrain()
        {
            InitializeDecisions();
            if (states.Count > 0)
            {
                CurrentState = states[0];
                CurrentState?.OnEnterState();
            }
        }

        private void InitializeDecisions()
        {
            if (decisions == null)
            {
                decisions = GetAttachedDecisions();
            }
            foreach (AIDecision decision in decisions)
            {
                decision.Initialize();
            }
        }

        public void TransitionToState(string newStateName)
        {
            if (CurrentState == null)
            {
                CurrentState = FindState(newStateName);
                if (CurrentState != null)
                {
                    CurrentState.OnEnterState();
                }
                return;
            }
            if (newStateName != CurrentState.StateName)
            {
                CurrentState.OnExitState();
                OnExitState();

                CurrentState = FindState(newStateName);
                if (CurrentState != null)
                {
                    CurrentState.OnEnterState();
                }
            }
        }

        protected AIState FindState(string stateName)
        {
            foreach (AIState state in states)
            {
                if (state.StateName == stateName)
                {
                    return state;
                }
            }
            if (stateName != "")
            {
                Debug.LogError("You're trying to transition to state '" + stateName + "' in " + this.gameObject.name + "'s AI Brain, but no state of this name exists. Make sure your states are named properly, and that your transitions states match existing states.");
            }
            return null;
        }

        protected virtual void OnExitState()
        {
            TimeInThisState = 0f;
        }



    }
}


