using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ControlRoom
{
    public class DashBrick : Brick
    {
        public float DashDistance = 3f;
        public float DashForce = 40f;
        public bool ResetForcesOnExit = false;



        private bool shouldKeepDashing = false;
        private float distanceTraveled = 0f;
        private float slopeAngleSave = 0f;
        private Vector2 initialPosition = Vector2.zero;
        private Vector2 dashDirection = Vector2.zero;
        private IEnumerator dashCoroutine;
        private bool dashEndedNaturally = true;

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void HandleInput()
        {
            base.HandleInput();
        }

        protected override void UpdateBrickProcess()
        {
            base.UpdateBrickProcess();
        }

        private void StartDash()
        {
            //if (dashCondition() == false)
            //    return;

            dashCoroutine = Dash();
            StartCoroutine(dashCoroutine);
        }

        IEnumerator Dash()
        {
            while(true)
            {
                distanceTraveled = Vector3.Distance(initialPosition, this.transform.position);

                if(controller.Conditions.IsCollidingLeft&&dashDirection.x<0f||
                    controller.Conditions.IsCollidingRight&&dashDirection.x>0f)
                {
                    shouldKeepDashing = false;
                    controller.SetHorizontalForce(0f);
                }
                else
                {
                    controller.GravityActive(false);
                    controller.SetHorizontalForce(dashDirection.x * DashForce);
                }
                
                yield return null;

                if (distanceTraveled >= DashDistance || !shouldKeepDashing)
                    break;
                
            }

            StopDash();
        }

        public void StopDash()
        {
            if (dashCoroutine != null)
            {
                StopCoroutine(dashCoroutine);
            }
            // once our dash is complete, we reset our various states 
            controller.MaximumSlopeAngle = slopeAngleSave;
            controller.GravityActive(true);
            dashEndedNaturally = true;

            // we reset our forces 
            if (ResetForcesOnExit)
            {
                controller.SetHorizontalForce(0f);
            }

            // we play our exit sound 
            StopStartFeedbacks();
            PlayAbilityStopFeedbacks();
            // once the boost is complete, if we were dashing, we make it stop and start the dash cooldown 
            if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
            {
                if (controller.Conditions.IsGrounded)
                {
                    _movement.ChangeState(CharacterStates.MovementStates.Idle);
                }
                else
                {
                    _movement.RestorePreviousState();
                }
            }
        }


    }

   

}
