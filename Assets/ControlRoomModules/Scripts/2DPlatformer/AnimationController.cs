using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
  
    public class AnimationController: MonoBehaviour
    {
        private Animator animator;
        private HashSet<int> animatorParameters;
        private Brick[] bricks;


        private void Awake() 
        {

            Initialize();
        }

        private void Update()
        {
            if(this.bricks?.Length>0)
            {
                foreach(var brick in this.bricks)
                {
                    brick.UpdateAnimator();
                }
            }
        }


        private void Initialize()
        {
            this.animator = this.GetComponent<Animator>();

            if (this.animator == null)
                return;

            animatorParameters = new HashSet<int>();
        }

        public void SetBricks(Brick[] bricks)
        {
            this.bricks = bricks;
        }

        public void AddAnimatorParameterIfExists(string parameterName, out int parameter, AnimatorControllerParameterType type)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                parameter = -1;
                return;
            }

            parameter = Animator.StringToHash(parameterName);

            if (this.animator.CheckToIncludeAnimatorParameter(parameterName, type))
            {
                this.animatorParameters.Add(parameter);
            }
        }

        public void UpdateAnimatorFloat(int paramter,float value)
        {
            if(this.animator!=null)
            {
               if(this.animatorParameters.Contains(paramter))  
                    this.animator.SetFloat(paramter, value);
            }
        }

        public void UpdateAnimatorBool(int paramter,bool value)
        {
            if (this.animator != null)
            {
                if (this.animatorParameters.Contains(paramter))
                    this.animator.SetBool(paramter, value);
            }
        }

        public void UpdateAnimatorTrigger(int paramter)
        {
            if (this.animator != null)
            {
                if (this.animatorParameters.Contains(paramter))
                    this.animator.SetTrigger(paramter);
            }
        }




    }


    public static class AnimatorExtension
    {
        public static bool CheckToIncludeAnimatorParameter(this Animator self, string paramName,AnimatorControllerParameterType paramType)
        {
            if (string.IsNullOrEmpty(paramName)) 
            { 
                 return false; 
            }

            AnimatorControllerParameter[] parameters = self.parameters;
            foreach (AnimatorControllerParameter currParam in parameters)
            {
                if (currParam.type == paramType && currParam.name == paramName)
                {
                    return true;
                }
            }
            return false;


        }
    }
}

