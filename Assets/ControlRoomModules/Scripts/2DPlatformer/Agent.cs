using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class Agent: MonoBehaviour
    {
        protected ControlRoom.PhysicsController controller;
        protected ControlRoom.Brick[] bricks;
        protected ControlRoom.AnimationController animationController;
        protected SpriteRenderer spriteRenderer;
       
        public enum FacingDirections { Left, Right }
        public FacingDirections InitialFacingDirection = FacingDirections.Right;


        public bool CanFlip { get; set; }
        public bool IsFacingRight { get; set; }


        private void Awake() 
        {
            this.controller=this.GetComponent<ControlRoom.PhysicsController>();
            this.bricks=this.GetComponentsInChildren<ControlRoom.Brick>();
            this.animationController=this.GetComponentInChildren<ControlRoom.AnimationController>();
            this.spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();

            Initialize();
        }

        private void Initialize()
        {
            if (InitialFacingDirection == FacingDirections.Left)
            {
                IsFacingRight = false;
            }
            else
            {
                IsFacingRight = true;
            }

            CanFlip = true;
        }


        private void Update() 
        {
            foreach(var brick in this.bricks)
            {
                if(brick.isActiveAndEnabled)
                {
                    brick.UpdateFrame();
                }
            }   
        }

        public void Flip()
        {
            if (!CanFlip)
                return;

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !spriteRenderer.flipX;
               
            }

            IsFacingRight = !IsFacingRight;

            foreach (var brick in this.bricks)
            {
                if (brick.isActiveAndEnabled)
                {
                    brick.Flip();
                }
            }

        }



    }
}