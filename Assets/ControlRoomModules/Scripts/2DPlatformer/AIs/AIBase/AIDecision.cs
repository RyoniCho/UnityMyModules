using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public abstract class AIDecision : MonoBehaviour
    {
        public virtual void Initialize()
        {
            
        }

        public abstract bool Decide();
        public bool DecisionInProgress { get; set; }
        public string Label;

        protected AIBrain brain;

        private void Awake()
        {
            this.brain = this.GetComponentInParent<AIBrain>();
        }
        private void Start()
        {
            Initialize();
        }
        public virtual void OnEnterState()
        {
            DecisionInProgress = true;
        }

        public virtual void OnExitState()
        {
            DecisionInProgress = false;
        }



    }
}

