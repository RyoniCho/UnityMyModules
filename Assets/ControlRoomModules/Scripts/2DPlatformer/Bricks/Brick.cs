using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class Brick: MonoBehaviour
    {
        protected float horizontalInput=0f;
        protected float verticalInput=0f;
        protected bool IsInitialized=false;

        protected ControlRoom.PhysicsController controller;
		protected ControlRoom.Agent agent;
		protected ControlRoom.AnimationController animationController;

        protected virtual void Start () 
		{
			Initialize();
		}

        protected virtual void Initialize()
        {
            this.controller=this.GetComponentInParent<ControlRoom.PhysicsController>();
			this.agent = this.GetComponentInParent<ControlRoom.Agent>();

			if(this.agent!=null)
				this.animationController = this.agent.AnimController;

			InitializeAnimParam();

			IsInitialized=true;
        }

        public void UpdateFrame()
        {
            this.InternalHandleInput();
            this.UpdateBrickProcess();
        }

		

        protected virtual void InternalHandleInput()
		{
			// if (_inputManager == null) { return; }

			verticalInput = Input.GetAxisRaw("Vertical");//_inputManager.PrimaryMovement.y;
			horizontalInput = Input.GetAxisRaw("Horizontal");//_inputManager.PrimaryMovement.x;

			// if (_characterGravity != null)
			// {
			// 	if (_characterGravity.ShouldReverseInput())
			// 	{
			// 		if (_characterGravity.ReverseVerticalInputWhenUpsideDown)
			// 		{
			// 			_verticalInput = -_verticalInput;
			// 		}
			// 		if (_characterGravity.ReverseHorizontalInputWhenUpsideDown)
			// 		{
			// 			_horizontalInput = -_horizontalInput;
			// 		}	
			// 	}
			// }
			HandleInput();
		}

		

		/// <summary>
		/// Called at the very start of the ability's cycle, and intended to be overridden, looks for input and calls methods if conditions are met
		/// </summary>
		protected virtual void HandleInput()
		{

		}

        protected virtual void UpdateBrickProcess()
        {

        }

		public virtual void Flip()
        {

        }

		protected virtual void InitializeAnimParam()
        {

        }

		public virtual void UpdateAnimator()
		{

		}

	}    
}
