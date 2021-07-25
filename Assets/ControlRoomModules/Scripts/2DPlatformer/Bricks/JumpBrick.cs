using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ControlRoom
{
    public class JumpBrick : Brick
    {
        // animation parameters
        public int NumberOfJumps = 2;
        public float JumpHeight = 3.025f;
        public int NumberOfJumpsLeft;

        [Header("Proportional jumps")]

		/// if true, the jump duration/height will be proportional to the duration of the button's press
		[Tooltip("if true, the jump duration/height will be proportional to the duration of the button's press")]
		public bool JumpIsProportionalToThePressTime = true;
		/// the minimum time in the air allowed when jumping - this is used for pressure controlled jumps
		[Tooltip("the minimum time in the air allowed when jumping - this is used for pressure controlled jumps")]
		public float JumpMinimumAirTime = 0.1f;
		/// the amount by which we'll modify the current speed when the jump button gets released
		[Tooltip("the amount by which we'll modify the current speed when the jump button gets released")]
		public float JumpReleaseForceFactor = 2f;
        
        public bool CanJumpStop { get; set; }

        protected string _jumpingAnimationParameterName = "Jump";
        private string _doubleJumpingAnimationParameterName = "DoubleJumping";
        private string  _hitTheGroundAnimationParameterName = "HitTheGround";
        protected int _jumpingAnimationParameter;
        protected int _doubleJumpingAnimationParameter;
        protected int _hitTheGroundAnimationParameter;

        private float jumpButtonPressTime = 0;
        private bool jumpButtonPressed=false;
        private bool jumpButtonReleased=true;
        private bool doubleJumping=false;
        private bool jumpHappenedThisFrame=false;
        private float lastTimeGrounded=0f;

        

        protected override void Initialize() 
        {
            base.Initialize();

            NumberOfJumpsLeft=NumberOfJumps;
            jumpHappenedThisFrame=false;
        }

        protected override void HandleInput()
        {
            //if(InputManager.Instance.Jump.Down)
            if(Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("Jump Press down");
                JumpStart();
            }
            
			//if (InputManager.Instance.Jump.Up)
            if(Input.GetKeyUp(KeyCode.P))
			{
                 Debug.Log("Jump Press Up");
				JumpStop();
			}
        }

        protected override void UpdateBrickProcess()
        {
              jumpHappenedThisFrame = false;

            //if (!AbilityAuthorized) { return; }

			// if we just got grounded, we reset our number of jumps
			if (controller.Conditions.JustGotGrounded)
			{
                NumberOfJumpsLeft = NumberOfJumps;	
				doubleJumping = false;
            }

			// we store the last timestamp at which the character was grounded
			if (controller.Conditions.IsGrounded)
			{
				lastTimeGrounded = Time.time;
               
            }

            // If the user releases the jump button and the character is jumping up and enough time since the initial jump has passed, then we make it stop jumping by applying a force down.
            if ( (jumpButtonPressTime != 0) 
			    && (Time.time - jumpButtonPressTime >= JumpMinimumAirTime) 
			    && (controller.Speed.y > Mathf.Sqrt(Mathf.Abs(controller.initialGravity))) 
			    && (jumpButtonReleased))
			{
				jumpButtonReleased=false;
                
                if (JumpIsProportionalToThePressTime)	
				{	
					jumpButtonPressTime=0;
					if (JumpReleaseForceFactor == 0f)
					{
						controller.SetVerticalForce (0f);
					}
					else
					{
						controller.AddVerticalForce(-controller.Speed.y/JumpReleaseForceFactor);	
					}
				}
			}

            UpdateController();
        }

       
        public void JumpStart()
		{
			if (!CheckToJumpCondition())
			{
				return;
            }
			
            // we reset our walking speed
            // if ((_movement.CurrentState == CharacterStates.MovementStates.Crawling)
            //     || (_movement.CurrentState == CharacterStates.MovementStates.Crouching)
            //     || (_movement.CurrentState == CharacterStates.MovementStates.LadderClimbing))
			// {
			// 	_characterHorizontalMovement.ResetHorizontalSpeed();
			// }	
            
            // if (_movement.CurrentState == CharacterStates.MovementStates.LadderClimbing)
            // {
            //     _characterLadder.GetOffTheLadder();
            // }

			// _controller.ResetColliderSize();

            // // if we're still here, the jump will happen
            // // we set our current state to Jumping
            // _movement.ChangeState(CharacterStates.MovementStates.Jumping);

			// // we trigger a character event
			// MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Jump);


            // we start our feedbacks
            // if ((controller.State.IsGrounded) || _coyoteTime) 
            // {
            //     PlayAbilityStartFeedbacks();
            // }
            // else
            // {
            //     AirJumpFeedbacks?.PlayFeedbacks();
            // }

            // if (ResetCameraOffsetOnJump && (_sceneCamera != null))
            // {
            //     _sceneCamera.ResetLookUpDown();
            // }            

            if (NumberOfJumpsLeft != NumberOfJumps)
			{
				doubleJumping=true;
			}

			// we decrease the number of jumps left
			NumberOfJumpsLeft = NumberOfJumpsLeft - 1;

			// we reset our current condition and gravity
			//_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
			controller.GravityActive(true);
			//_controller.CollisionsOn ();

			// we set our various jump flags and counters
			SetJumpFlags();
            CanJumpStop = true;

            // we make the character jump
            controller.SetVerticalForce(Mathf.Sqrt( 2f * JumpHeight * Mathf.Abs(controller.initialGravity) ));
            jumpHappenedThisFrame = true;

            if (animationController != null)
                animationController.UpdateAnimatorTrigger(_jumpingAnimationParameter);


        }

        public void JumpStop()
		{
            if (!CanJumpStop)
            {
                return;
            }
			jumpButtonPressed =false;
			jumpButtonReleased=true;
		}

        public void SetJumpFlags()
        {
            jumpButtonPressTime=Time.time;
			jumpButtonPressed=true;
			jumpButtonReleased=false;
        }


        private void UpdateController()
        {
            // controller.Conditions.IsJumping = (_movement.CurrentState == CharacterStates.MovementStates.Jumping
            //         || _movement.CurrentState == CharacterStates.MovementStates.DoubleJumping
            //         || _movement.CurrentState == CharacterStates.MovementStates.WallJumping);
        }

        private bool CheckToJumpCondition()
        {
            if (controller.Conditions.IsCollidingAbove)
			{
                Debug.Log("CheckToJumpCondition:controller.Conditions.IsCollidingAbove");
				return false;
			}

            if(NumberOfJumpsLeft <= 0)
            {
                Debug.Log("CheckToJumpCondition:NumberOfJumpsLeft");
                return false;
            }           

            return true;
        }

        protected override void InitializeAnimParam()
        {
            if(animationController!=null)
                animationController.AddAnimatorParameterIfExists(_jumpingAnimationParameterName, out _jumpingAnimationParameter, AnimatorControllerParameterType.Trigger);
        }

       
    }
}

