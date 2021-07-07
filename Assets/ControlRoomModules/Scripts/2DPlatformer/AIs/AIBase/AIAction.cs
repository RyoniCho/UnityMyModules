using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public abstract class AIAction : MonoBehaviour
    {
        protected AIBrain brain;

        public abstract void StartAction();

        public bool ActionInProgress { get; set; }

        private void Awake()
        {
            brain = this.GetComponentInParent<AIBrain>();
        }

        protected virtual void Start()
        {
            Initialization();
        }

       
        protected virtual void Initialization()
        {

        }

        public virtual void OnEnterState()
        {
            ActionInProgress = true;
        }

       
        public virtual void OnExitState()
        {
            ActionInProgress = false;
        }
    }
}

