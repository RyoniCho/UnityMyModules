using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class AIDecisionTimeSinceStart : AIDecision
    {
        public float TimeLimit;
        private float startTime;

        public override void OnEnterState()
        {
            base.OnEnterState();

            this.startTime = Time.time;
        }

        public override bool Decide()
        {

            return CheckTime();
           
        }

        private bool CheckTime()
        {
            return ((Time.time - startTime) >= TimeLimit);
        }
    }
}


