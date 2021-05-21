using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
  
    public class AnimationController: MonoBehaviour
    {
        private Animator animator;
        private HashSet<int> animatorParameters;

        //Animator Parameter
        protected const string groundedAnimationParameterName = "Grounded";
		protected const string airborneAnimationParameterName = "Airborne";
		protected const string xSpeedAnimationParameterName = "xSpeed";
		protected const string ySpeedAnimationParameterName = "ySpeed";
		protected const string worldXSpeedAnimationParameterName = "WorldXSpeed";
		protected const string worldYSpeedAnimationParameterName = "WorldYSpeed";
		protected const string collidingLeftAnimationParameterName = "CollidingLeft";
		protected const string collidingRightAnimationParameterName = "CollidingRight";
		protected const string collidingBelowAnimationParameterName = "CollidingBelow";
		protected const string collidingAboveAnimationParameterName = "CollidingAbove";
		protected const string idleSpeedAnimationParameterName = "Idle";
		protected const string aliveAnimationParameterName = "Alive";
		protected const string facingRightAnimationParameterName = "FacingRight";
        protected const string randomAnimationParameterName = "Random";
        protected const string randomConstantAnimationParameterName = "RandomConstant";

        protected int groundedAnimationParameter;
		protected int airborneSpeedAnimationParameter;
		protected int xSpeedSpeedAnimationParameter;
		protected int ySpeedSpeedAnimationParameter;
		protected int worldXSpeedSpeedAnimationParameter;
		protected int worldYSpeedSpeedAnimationParameter;
		protected int collidingLeftAnimationParameter;
		protected int collidingRightAnimationParameter;
		protected int collidingBelowAnimationParameter;
		protected int collidingAboveAnimationParameter;
		protected int idleSpeedAnimationParameter;
		protected int aliveAnimationParameter;
		protected int facingRightAnimationParameter;
        protected int randomAnimationParameter;
        protected int randomConstantAnimationParameter;


        private void Awake() 
        { 
            this.animator=this.GetComponent<Animator>();
        }


        private void SetAnimatorParameter()
        {
            if (this.animator == null) 
                return; 

            animatorParameters = new HashSet<int>();

            AddAnimatorParameterIfExists(groundedAnimationParameterName,out groundedAnimationParameter,AnimatorControllerParameterType.Bool,animatorParameters);
            AddAnimatorParameterIfExists(airborneAnimationParameterName,out airborneSpeedAnimationParameter,AnimatorControllerParameterType.Bool,animatorParameters);


        }

        public void AddAnimatorParameterIfExists(string parameterName, out int parameter, AnimatorControllerParameterType type, HashSet<int> parameterList)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                parameter = -1;
                return;
            }

            parameter = Animator.StringToHash(parameterName);

            if (this.animator.CheckToIncludeAnimatorParameter(parameterName, type))
            {
                parameterList.Add(parameter);
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

