using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class AIActionMoveTowardsTarget : AIAction
    {

        public float targetStopDistance = 1f;

        private Agent agent;
        private HorizontalMovementBrick horizontalBrick;
        

        protected override void Initialization()
        {
            base.Initialization();
            this.agent = this.GetComponentInParent<Agent>();
            this.horizontalBrick = agent?.FindBrick<HorizontalMovementBrick>();
        }

        public override void StartAction()
        {
            Move();
        }

        private void Move()
        {
            if (brain.Target == null)
            {
                horizontalBrick.SetHorizontalMove(0f);
                return;
            }
            if (Mathf.Abs(this.transform.position.x - brain.Target.position.x) <= targetStopDistance)
            {
                horizontalBrick.SetHorizontalMove(0f);
                return;
            }

            if (this.transform.position.x < brain.Target.position.x)
            {
                horizontalBrick.SetHorizontalMove(1f);
            }
            else
            {
                horizontalBrick.SetHorizontalMove(-1f);
            }
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            horizontalBrick.SetHorizontalMove(0f);
        }

        public override void OnExitState()
        {
            base.OnExitState();

            horizontalBrick.SetHorizontalMove(0f);
        }
    }

}
