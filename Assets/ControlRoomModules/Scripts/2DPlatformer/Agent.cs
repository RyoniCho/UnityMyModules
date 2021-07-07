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

        public void FlickColor(Color initialColor,Color flickerColor,float flickerSpeed,float flickerDuration)
        {
            StartCoroutine(Flicker(initialColor, flickerColor, flickerSpeed, flickerDuration));
        }


        private IEnumerator Flicker(Color initialColor, Color flickerColor, float flickerSpeed, float flickerDuration)
        {
            if (this.spriteRenderer == null)
            {
                yield break;
            }

            if (!this.spriteRenderer.material.HasProperty("_Color"))
            {
                yield break;
            }

            if (initialColor == flickerColor)
            {
                yield break;
            }

            float flickerStop = Time.time + flickerDuration;

            while (Time.time < flickerStop)
            {
                this.spriteRenderer.material.color = flickerColor;
                yield return new WaitForSeconds(flickerSpeed);
                this.spriteRenderer.material.color = initialColor;
                yield return new WaitForSeconds(flickerSpeed);
            }

            this.spriteRenderer.material.color = initialColor;
        }

        public Color InitialColor
        {
            get
            {
                if (this.spriteRenderer != null)
                {
                    if (this.spriteRenderer.material.HasProperty("_Color"))
                    {
                        return this.spriteRenderer.material.color;
                    }
                }

                return Color.white;
            }
        }

        public T FindBrick<T>() where T: Brick
        {
            foreach(var brick in bricks)
            {
                if(brick is T findbrick)
                {
                    return findbrick;
                }
            }

            return null;
        }



    }
}