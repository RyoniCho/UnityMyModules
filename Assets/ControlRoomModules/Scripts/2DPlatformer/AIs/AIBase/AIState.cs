using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ControlRoom
{
    [System.Serializable]
    public class AIState 
    {
        public string StateName;
        public List<AIAction> Actions;
        public List<AITransition> Transitions;

        private AIBrain brain;

      
        public void SetBrain(AIBrain brain)
        {
            this.brain = brain;
        }

        public void OnEnterState()
        {
            foreach (AIAction action in Actions)
            {
                action.OnEnterState();
            }
            foreach (AITransition transition in Transitions)
            {
                if (transition.Decision != null)
                {
                    transition.Decision.OnEnterState();
                }
            }
        }

        public void OnExitState()
        {
            foreach (AIAction action in Actions)
            {
                action.OnExitState();
            }
            foreach (AITransition transition in Transitions)
            {
                if (transition.Decision != null)
                {
                    transition.Decision.OnExitState();
                }
            }
        }

        public void StartActions()
        {
            if (Actions.Count == 0) { return; }
            for (int i = 0; i < Actions.Count; i++)
            {
                if (Actions[i] != null)
                {
                    Actions[i].StartAction();
                }
                else
                {
                    Debug.LogError("An action in " + brain.gameObject.name + " is null.");
                }
            }
        }

        public void EvaluateTransitions()
        {
            if (Transitions.Count == 0) { return; }
            for (int i = 0; i < Transitions.Count; i++)
            {
                if (Transitions[i].Decision != null)
                {
                    if (Transitions[i].Decision.Decide())
                    {
                        if (!string.IsNullOrEmpty(Transitions[i].TrueState))
                        {
                            brain.TransitionToState(Transitions[i].TrueState);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Transitions[i].FalseState))
                        {
                            brain.TransitionToState(Transitions[i].FalseState);
                        }
                    }
                }
            }
        }
    }
}

