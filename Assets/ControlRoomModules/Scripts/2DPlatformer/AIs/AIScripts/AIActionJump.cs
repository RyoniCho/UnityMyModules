using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class AIActionJump : AIAction
    {
        public int MaxNumberOfJump = 1;

        JumpBrick JumpBrick;
        private int currentNumberOfJump = 0;

        protected override void Initialization()
        {
            base.Initialization();

            this.JumpBrick= this.gameObject.GetComponentInParent<Agent>()?.FindBrick<JumpBrick>();

        }

        public override void StartAction()
        {
            Jump();
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            currentNumberOfJump = 0;
        }

        private void Jump()
        {
            if (currentNumberOfJump < MaxNumberOfJump)
            {
                this.JumpBrick.JumpStart();
                currentNumberOfJump++;
            }
        }
    }

}
