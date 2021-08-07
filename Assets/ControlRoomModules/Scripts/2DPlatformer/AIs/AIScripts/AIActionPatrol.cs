using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class AIActionPatrol : AIAction
    {

        HorizontalMovementBrick horizontalMovementBrick;
        Vector3 startPosition;


        protected override void Initialization()
        {
            base.Initialization();

            this.horizontalMovementBrick = this.gameObject.GetComponentInParent<Agent>()?.FindBrick<HorizontalMovementBrick>();

            startPosition = this.transform.position;

        }

        public override void StartAction()
        {
            
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
        }

        public override void OnExitState()
        {
            base.OnExitState();

            this.horizontalMovementBrick.SetHorizontalMove(0f);
        }
    }
}

