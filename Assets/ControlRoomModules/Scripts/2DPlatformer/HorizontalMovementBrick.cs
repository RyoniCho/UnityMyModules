using UnityEngine;
using System.Collections;


namespace ControlRoom
{	
	public class HorizontalMovementBrick : Brick 
	{
		
		/// the current reference movement speed
		public float MovementSpeed { get; set; }

        [Header("Speed")]

		/// the speed of the character when it's walking
		[Tooltip("the speed of the character when it's walking")]
		public float WalkSpeed = 6f;
        /// the multiplier to apply to the horizontal movement
       
		[Tooltip("the multiplier to apply to the horizontal movement")]
		public float MovementSpeedMultiplier = 1f;
        /// the multiplier to apply when pushing
       
		[Tooltip("the multiplier to apply when pushing")]
		public float PushSpeedMultiplier = 1f;
        /// the multiplier that gets set and applied by CharacterSpeed
       
        [Tooltip("the multiplier that gets set and applied by CharacterSpeed")]
        public float StateSpeedMultiplier = 1f;
        /// if this is true, the character will automatically flip to face its movement direction
        [Tooltip("if this is true, the character will automatically flip to face its movement direction")]
        public bool FlipCharacterToFaceDirection = true;


        /// the current horizontal movement force
		public float HorizontalMovementForce { get { return _horizontalMovementForce; }}
        /// if this is true, movement will be forbidden (as well as flip)
        public bool MovementForbidden { get; set; }

        [Header("Input")]

		/// if this is true, will get input from an input source, otherwise you'll have to set it via SetHorizontalMove()
		[Tooltip("if this is true, will get input from an input source, otherwise you'll have to set it via SetHorizontalMove()")]
		public bool ReadInput = true;
		/// if this is true, no acceleration will be applied to the movement, which will instantly be full speed (think Megaman movement)
		[Tooltip("if this is true, no acceleration will be applied to the movement, which will instantly be full speed (think Megaman movement)")]
		public bool InstantAcceleration = false;
		/// the threshold after which input is considered (usually 0.1f to eliminate small joystick noise)
		[Tooltip("the threshold after which input is considered (usually 0.1f to eliminate small joystick noise)")]
		public float InputThreshold = 0.1f;
        /// how much air control the player has
        [Range(0f, 1f)]
		[Tooltip("how much air control the player has")]
		public float AirControl = 1f;
		/// whether or not the player can flip in the air
		[Tooltip("whether or not the player can flip in the air")]
		public bool AllowFlipInTheAir = true;
		/// whether or not this ability should keep taking care of horizontal movement after death
		[Tooltip("whether or not this ability should keep taking care of horizontal movement after death")]
		public bool ActiveAfterDeath = false;

        [Header("Touching the Ground")]
		/// the MMFeedbacks to play when the character hits the ground
		
		/// the duration (in seconds) during which the character has to be airborne before a feedback can be played when touching the ground
		[Tooltip("the duration (in seconds) during which the character has to be airborne before a feedback can be played when touching the ground")]
		public float MinimumAirTimeBeforeFeedback = 0.2f;

        [Header("Walls")]
        /// Whether or not the state should be reset to Idle when colliding laterally with a wall
		[Tooltip("Whether or not the state should be reset to Idle when colliding laterally with a wall")]
		public bool StopWalkingWhenCollidingWithAWall = false;
                
		protected float _horizontalMovement;
        protected float _lastGroundedHorizontalMovement;
        protected float _horizontalMovementForce;
	    protected float _normalizedHorizontalSpeed;
        protected float _lastTimeGrounded = 0f;

        // animation parameters
        protected const string _speedAnimationParameterName = "Speed";
        protected const string _walkingAnimationParameterName = "Walking";
        protected int _speedAnimationParameter;
        protected int _walkingAnimationParameter;

        /// <summary>
        /// On Initialization, we set our movement speed to WalkSpeed.
        /// </summary>
        protected override void Initialize()
		{
			base.Initialize ();
			MovementSpeed = WalkSpeed;
			MovementSpeedMultiplier = 1f;
            MovementForbidden = false;
		}

	    /// <summary>
	    /// The second of the 3 passes you can have in your ability. Think of it as Update()
	    /// </summary>
		protected override void UpdateBrickProcess()
	    {
			base.UpdateBrickProcess();

			HandleHorizontalMovement();
            DetectWalls();
	    }

	    /// <summary>
	    /// Called at the very start of the ability's cycle, and intended to be overridden, looks for input and calls
	    /// methods if conditions are met
	    /// </summary>
	    protected override void HandleInput()
	    {
            if (!ReadInput)
            {
                return;
            }

			_horizontalMovement = Input.GetAxisRaw("Horizontal");

            if ((AirControl < 1f) && !controller.Conditions.IsGrounded)
            {
                _horizontalMovement = Mathf.Lerp(_lastGroundedHorizontalMovement, Input.GetAxisRaw("Horizontal"), AirControl);
            }
	    }

        /// <summary>
        /// When using low (or null) air control, this method lets you externally set the direction air control should consider as the base value
        /// </summary>
        /// <param name="newInputValue"></param>
        public virtual void SetAirControlDirection(float newInputValue)
        {
            _lastGroundedHorizontalMovement = newInputValue;
        }

		/// <summary>
		/// Sets the horizontal move value.
		/// </summary>
		/// <param name="value">Horizontal move value, between -1 and 1 - positive : will move to the right, negative : will move left </param>
		public virtual void SetHorizontalMove(float value)
		{
			_horizontalMovement = value;
		}

		/// <summary>
	    /// Called at Update(), handles horizontal movement
	    /// </summary>
	    protected virtual void HandleHorizontalMovement()
		{	
			// if we're not walking anymore, we stop our walking sound
			// if ((_movement.CurrentState != CharacterStates.MovementStates.Walking) && _startFeedbackIsPlaying)
            // {
            //     StopStartFeedbacks();
			// }

			// check if we just got grounded
			CheckJustGotGrounded();
                        
            // if movement is prevented, or if the character is dead/frozen/can't move, we exit and do nothing
            // if (!ActiveAfterDeath)
            // {
	        //     if (!AbilityAuthorized 
	        //         || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal) 
	        //         || (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
	        //     {
		    //         return;
	        //     }
            // }

            bool canFlip = true;

            // if (MovementForbidden)
            // {
            //     _horizontalMovement = _character.Airborne ? _controller.Speed.x * Time.deltaTime : 0f;
            //     canFlip = false;
            // }

            if (!controller.Conditions.IsGrounded && !AllowFlipInTheAir)
            {
                canFlip = false;
            }

            // If the value of the horizontal axis is positive, the character must face right.
            if (_horizontalMovement > InputThreshold)
			{
				_normalizedHorizontalSpeed = _horizontalMovement;
				// if (!_character.IsFacingRight && canFlip && FlipCharacterToFaceDirection)
                // {
                //     _character.Flip();
                // }					
			}		
			// If it's negative, then we're facing left
			else if (_horizontalMovement < -InputThreshold)
			{
				_normalizedHorizontalSpeed = _horizontalMovement;
				// if (_character.IsFacingRight && canFlip && FlipCharacterToFaceDirection)
                // {
                //     _character.Flip();
                // }					
			}
			else
			{
				_normalizedHorizontalSpeed = 0;
			}

            /// if we're dashing, we stop there
            // if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
            // {
            //     return;
            // }

			// if we're grounded and moving, and currently Idle, Dangling or Falling, we become Walking
			// if ( (_controller.State.IsGrounded)
			// 	&& (_normalizedHorizontalSpeed != 0)
			// 	&& ( (_movement.CurrentState == CharacterStates.MovementStates.Idle)
            //         || (_movement.CurrentState == CharacterStates.MovementStates.Dangling)
            //         || (_movement.CurrentState == CharacterStates.MovementStates.Falling)))
			// {				
			// 	_movement.ChangeState(CharacterStates.MovementStates.Walking);
            //     PlayAbilityStartFeedbacks();	
			// }

            // // if we're grounded, jumping but not moving up, we become idle
            // if ((_controller.State.IsGrounded) 
            //     && (_movement.CurrentState == CharacterStates.MovementStates.Jumping)
            //     && (_controller.Speed.y <= 0f))
            // {
            //     _movement.ChangeState(CharacterStates.MovementStates.Idle);
            // }

            // // if we're walking and not moving anymore, we go back to the Idle state
            // if ((_movement.CurrentState == CharacterStates.MovementStates.Walking) 
			// && (_normalizedHorizontalSpeed == 0))
			// {
			// 	_movement.ChangeState(CharacterStates.MovementStates.Idle);
            //     PlayAbilityStopFeedbacks();
			// }

			// // if the character is not grounded, but currently idle or walking, we change its state to Falling
			// if (!_controller.State.IsGrounded
			// 	&& (
			// 		(_movement.CurrentState == CharacterStates.MovementStates.Walking)
			// 		 || (_movement.CurrentState == CharacterStates.MovementStates.Idle)
			// 		))
			// {
			// 	_movement.ChangeState(CharacterStates.MovementStates.Falling);
			// }

            // we apply instant acceleration if needed
            if (InstantAcceleration)
            {
                if (_normalizedHorizontalSpeed > 0f) { _normalizedHorizontalSpeed = 1f; }
                if (_normalizedHorizontalSpeed < 0f) { _normalizedHorizontalSpeed = -1f; }
            }

			// we pass the horizontal force that needs to be applied to the controller.
			float groundAcceleration = 20;//_controller.Parameters.SpeedAccelerationOnGround;
			float airAcceleration = 6;//_controller.Parameters.SpeedAccelerationInAir;
			
			// if (_controller.Parameters.UseSeparateDecelerationOnGround && (Mathf.Abs(_horizontalMovement) < InputThreshold))
			// {
			// 	groundAcceleration = _controller.Parameters.SpeedDecelerationOnGround;
			// }
			// if (_controller.Parameters.UseSeparateDecelerationInAir && (Mathf.Abs(_horizontalMovement) < InputThreshold))
			// {
			// 	airAcceleration = _controller.Parameters.SpeedDecelerationInAir;
			// }
			
			float movementFactor = controller.Conditions.IsGrounded ? groundAcceleration : airAcceleration;
			float movementSpeed = _normalizedHorizontalSpeed * MovementSpeed * 1f * MovementSpeedMultiplier * StateSpeedMultiplier * PushSpeedMultiplier;//_controller.Parameters.SpeedFactor * MovementSpeedMultiplier * StateSpeedMultiplier * PushSpeedMultiplier;
                        
            if (InstantAcceleration && controller.Conditions.IsGrounded)
            {
                // if we are in instant acceleration mode, we just apply our movement speed
                _horizontalMovementForce = movementSpeed;

                // and any external forces that may be active right now
                // if (Mathf.Abs(_controller.ExternalForce.x) > 0)
                // {
                //     _horizontalMovementForce += _controller.ExternalForce.x;
                // }                
            }
            else
            {
                // if we are not in instant acceleration mode, we lerp towards our movement speed
                _horizontalMovementForce = Mathf.Lerp(controller.Speed.x, movementSpeed, Time.deltaTime * movementFactor);
				
            }			
				
			// we handle friction
			//_horizontalMovementForce = HandleFriction(_horizontalMovementForce);

			// we set our newly computed speed to the controller
			controller.SetHorizontalForce(_horizontalMovementForce);

            if (controller.Conditions.IsGrounded)
            {
                _lastGroundedHorizontalMovement = _horizontalMovement;
            }            
        }

        protected virtual void DetectWalls()
        {
            if (!StopWalkingWhenCollidingWithAWall)
            {
                return;
            }

            // if ((_movement.CurrentState == CharacterStates.MovementStates.Walking) || (_movement.CurrentState == CharacterStates.MovementStates.Running))
            // {
            //     if ((_controller.State.IsCollidingLeft) || (_controller.State.IsCollidingRight))
            //     {
            //         _movement.ChangeState(CharacterStates.MovementStates.Idle);
            //     }
            // }
        }

		/// <summary>
		/// Every frame, checks if we just hit the ground, and if yes, changes the state and triggers a particle effect
		/// </summary>
		protected virtual void CheckJustGotGrounded()
		{
			// if the character just got grounded
            // if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
            // {
            //     return;
            // }

			// if (_controller.State.JustGotGrounded)
			// {
            //     if (_controller.State.ColliderResized)
            //     {
            //         _movement.ChangeState(CharacterStates.MovementStates.Crouching);
            //     }
            //     else
            //     {
            //         _movement.ChangeState(CharacterStates.MovementStates.Idle);
            //     }
				
			// 	_controller.SlowFall (0f);
            //     if (Time.time - _lastTimeGrounded > MinimumAirTimeBeforeFeedback)
            //     {
            //         TouchTheGroundFeedback?.PlayFeedbacks();
            //     }
                
            // }
            // if ((_controller.State.IsGrounded) 
            //     || (_character.MovementState.CurrentState == CharacterStates.MovementStates.LadderClimbing)
            //     || (_character.MovementState.CurrentState == CharacterStates.MovementStates.LedgeClimbing)
            //     || (_character.MovementState.CurrentState == CharacterStates.MovementStates.LedgeHanging)
            //     || (_character.MovementState.CurrentState == CharacterStates.MovementStates.Gripping)
            //     || (_character.MovementState.CurrentState == CharacterStates.MovementStates.SwimmingIdle))
            // {
            //     _lastTimeGrounded = Time.time;
            // }
		}

		/// <summary>
		/// Handles surface friction.
		/// </summary>
		/// <returns>The modified current force.</returns>
		/// <param name="force">the force we want to apply friction to.</param>
		// protected virtual float HandleFriction(float force)
		// {
		// 	// if we have a friction above 1 (mud, water, stuff like that), we divide our speed by that friction
		// 	if (_controller.Friction>1)
		// 	{
		// 		force = force/_controller.Friction;
		// 	}

		// 	// if we have a low friction (ice, marbles...) we lerp the speed accordingly
		// 	if (_controller.Friction<1 && _controller.Friction > 0)
		// 	{
		// 		force = Mathf.Lerp(_controller.Speed.x, force, Time.deltaTime * _controller.Friction * 10);
		// 	}

		// 	return force;
		// }

		/// <summary>
		/// A public method to reset the horizontal speed
		/// </summary>
		public virtual void ResetHorizontalSpeed()
		{
			MovementSpeed = WalkSpeed;
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		// protected override void InitializeAnimatorParameters()
		// {
		// 	RegisterAnimatorParameter (_speedAnimationParameterName, AnimatorControllerParameterType.Float, out _speedAnimationParameter);
		// 	RegisterAnimatorParameter (_walkingAnimationParameterName, AnimatorControllerParameterType.Bool, out _walkingAnimationParameter);
		// }

		// /// <summary>
		// /// Sends the current speed and the current value of the Walking state to the animator
		// /// </summary>
		// public override void UpdateAnimator()
		// {
        //     MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _speedAnimationParameter, Mathf.Abs(_normalizedHorizontalSpeed), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
        //     MMAnimatorExtensions.UpdateAnimatorBool(_animator, _walkingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Walking), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
		// }
        
        // /// <summary>
        // /// When the character gets revived we reinit it again
        // /// </summary>
		// protected virtual void OnRevive()
		// {
		// 	Initialization ();
		// }

		/// <summary>
		/// When the player respawns, we reinstate this agent.
		/// </summary>
		/// <param name="checkpoint">Checkpoint.</param>
		/// <param name="player">Player.</param>
		// protected override void OnEnable ()
		// {
		// 	base.OnEnable ();
		// 	if (gameObject.GetComponentInParent<Health>() != null)
		// 	{
		// 		gameObject.GetComponentInParent<Health>().OnRevive += OnRevive;
		// 	}
		// }

		// protected override void OnDisable()
		// {
		// 	base.OnDisable ();
		// 	if (_health != null)
		// 	{
		// 		_health.OnRevive -= OnRevive;
		// 	}			
		// }
	}
}