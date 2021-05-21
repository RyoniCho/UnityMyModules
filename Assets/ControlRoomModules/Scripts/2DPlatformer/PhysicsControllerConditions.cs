using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class PhysicsControllerConditions
    {
        public bool IsCollidingRight { get; set; }
        public bool IsCollidingLeft { get; set; }
        public bool IsCollidingAbove { get; set; }
        public bool IsCollidingBelow { get; set; }

        public float DistanceToLeftCollider;
        public float DistanceToRightCollider;

        public float LateralSlopeAngle;
        public float BelowSlopeAngle { get; set; }
        public bool SlopeAngleOK;
        public bool OnAMovingPlatform { get; set; }

        public bool JustGotGrounded { get; set;}
        public bool IsGrounded { get { return IsCollidingBelow; } }
        public bool IsFalling { get; set;}
        public bool IsJumping{ get; set; }
   
       
        public bool WasGroundedLastFrame { get; set;}
        public bool WasTouchingTheCeilingLastFrame {get; set;}
        
        


        public void Reset()
		{
			IsCollidingLeft = false;
			IsCollidingRight = false;
			IsCollidingAbove = false;
            DistanceToLeftCollider = -1;
            DistanceToRightCollider = -1;
			SlopeAngleOK = false;
			JustGotGrounded = false;
			IsFalling = true;
			LateralSlopeAngle = 0;
        }

    }
}