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
        public int SuccessiveDashAmount = 1;
        public int SuccessiveDashesLeft = 1;
        public float DashCooldown = 1f;
        public bool LimitedDashes = false;
        public float AnimationEndDelay = 0.3f;
        public bool ReadInput = true;


        private bool shouldKeepDashing = false;
        private float distanceTraveled = 0f;
        private float slopeAngleSave = 0f;
        private Vector2 initialPosition = Vector2.zero;
        private Vector2 dashDirection = Vector2.zero;
        private IEnumerator dashCoroutine;
        private bool dashEndedNaturally = true;

        private float startTime = 0f;
        private float cooldownTimeStamp = 0f;
        private float lastDashAt = 0f;

        private bool IsDashing = false;
        private const string dashAnimationParamName = "Dash";
        private int dashAnimParam;

        protected override void Initialize()
        {
            base.Initialize();
            SuccessiveDashesLeft = SuccessiveDashAmount;
        }

        protected override void HandleInput()
        {
            base.HandleInput();
            if (!ReadInput)
                return;

            if(Input.GetKeyDown(KeyCode.O))
            {
                StartDash();
            }
            
        }

        protected override void UpdateBrickProcess()
        {
            base.UpdateBrickProcess();
            //// If the character is dashing, we cancel the gravity
            //if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
            //{
            //    _controller.GravityActive(false);
            //}

            //// we reset our slope tolerance if dash didn't end naturally
            //if ((!_dashEndedNaturally) && (_movement.CurrentState != CharacterStates.MovementStates.Dashing))
            //{
            //    _dashEndedNaturally = true;
            //    _controller.Parameters.MaximumSlopeAngle = _slopeAngleSave;
            //}

            //HandleAmountOfDashesLeft();
        }
        ///// <summary>
        ///// Checks if conditions are met to reset the amount of dashes left
        ///// </summary>
        //protected virtual void HandleAmountOfDashesLeft()
        //{
        //    if ((SuccessiveDashesLeft >= SuccessiveDashAmount) || (Time.time - _lastDashAt < DashCooldown))
        //    {
        //        return;
        //    }

        //    switch (SuccessiveDashResetMethod)
        //    {
        //        case SuccessiveDashResetMethods.Time:
        //            if (Time.time - _lastDashAt > SuccessiveDashResetDuration)
        //            {
        //                SetSuccessiveDashesLeft(SuccessiveDashAmount);
        //            }
        //            break;
        //        case SuccessiveDashResetMethods.Grounded:
        //            if (_controller.State.IsGrounded)
        //            {
        //                SetSuccessiveDashesLeft(SuccessiveDashAmount);
        //            }
        //            break;
        //    }
        //}

        ///// <summary>
        ///// A method to reset the amount of successive dashes left
        ///// </summary>
        ///// <param name="newAmount"></param>
        //public virtual void SetSuccessiveDashesLeft(int newAmount)
        //{
        //    SuccessiveDashesLeft = newAmount;
        //}

        private void StartDash()
        {
            if (DashConditions() == false)
                return;

            startTime = Time.time;
            dashEndedNaturally = false;
            initialPosition = this.transform.position;
            distanceTraveled = 0;
            shouldKeepDashing = true;
            cooldownTimeStamp = Time.time + DashCooldown;
            lastDashAt = Time.time;

            if (LimitedDashes)
            {
                SuccessiveDashesLeft -= 1;
            }

            // we prevent our character from going through slopes
            slopeAngleSave = controller.MaximumSlopeAngle;
            controller.MaximumSlopeAngle = 0;

            ComputeDashDirection();

            dashCoroutine = Dash();
            StartCoroutine(dashCoroutine);
        }

        IEnumerator Dash()
        {
            while(true)
            {
                IsDashing = true;
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

            Invoke("StopDashAnimation", this.AnimationEndDelay);

            // we play our exit sound 
            //StopStartFeedbacks();
            //PlayAbilityStopFeedbacks();
            // once the boost is complete, if we were dashing, we make it stop and start the dash cooldown 
            //if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
            //{
            //    if (controller.Conditions.IsGrounded)
            //    {
            //        _movement.ChangeState(CharacterStates.MovementStates.Idle);
            //    }
            //    else
            //    {
            //        _movement.RestorePreviousState();
            //    }
            //}
        }

        private void ComputeDashDirection()
        {

            dashDirection = agent.IsFacingRight ? Vector2.right : Vector2.left;

        }

        public bool DashConditions()
        {
            // if we're in cooldown between two dashes, we prevent dash
            if (cooldownTimeStamp > Time.time)
            {
                return false;
            }

            // if we don't have dashes left, we prevent dash
            if (SuccessiveDashesLeft <= 0)
            {
                return false;
            }

            return true;
        }

        private void StopDashAnimation()
        {
            this.IsDashing = false;
        }


        protected override void InitializeAnimParam()
        {
            animationController.AddAnimatorParameterIfExists(dashAnimationParamName, out this.dashAnimParam, AnimatorControllerParameterType.Bool);
        }

        public override void UpdateAnimator()
        {
            animationController.UpdateAnimatorBool(this.dashAnimParam, this.IsDashing);
        }


    }

   

}
