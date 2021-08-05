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

        private const string FallAnimationParameterName = "Fall";
        private int FallAnimationParam;

        private void Awake() 
        {
            this.controller=this.GetComponent<ControlRoom.PhysicsController>();
            this.bricks=this.GetComponentsInChildren<ControlRoom.Brick>();
            this.animationController=this.GetComponentInChildren<ControlRoom.AnimationController>();
            this.spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();

            CheckControllerSettings();

            if (this.animationController!=null)
            {
                this.animationController.SetBricks(this.bricks);
               
            }
               

            Initialize();

        }

        private void Start()
        {
            if(this.animationController!=null)
                InitializeAnimationParam();
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

            UpdateAnimationParam();
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

        public AnimationController AnimController { get { return this.animationController; } }

        private void InitializeAnimationParam()
        {
            if(this.animationController==null)
            {
                Debug.LogError("Animation Controller is null(Agent)");
                return;
            }

            this.animationController.AddAnimatorParameterIfExists(FallAnimationParameterName, out FallAnimationParam, AnimatorControllerParameterType.Bool);
        }

        private void UpdateAnimationParam()
        {
            if(this.animationController!=null)
                this.animationController.UpdateAnimatorBool(FallAnimationParam, controller.Conditions.IsFalling);
        }

        private void CheckControllerSettings()
        {

            if(this.controller==null)
            {
                Debug.LogError($"{this.gameObject.name} : PhysicsController is not exists");
            }
            if (this.animationController == null)
            {
                Debug.LogError($"{this.gameObject.name} : AnimationController is not exists");
            }
            if (this.spriteRenderer == null)
            {
                Debug.LogError($"{this.gameObject.name} : SpriteRenderer is not exists");
            }
          

        }

    }
}