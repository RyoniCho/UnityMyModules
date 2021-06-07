using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class PhysicsController : MonoBehaviour
    {
		public Vector2 MaxVelocity = new Vector2(100f, 100f);

        [Tooltip("a small value added to all raycasts to accomodate for edge cases")]
        public float RayOffset = 0.05f;
        public int numberOfVerticalRays=8;
		public int numberOfHorizontalRays=8;
        public LayerMask PlatformMask;

			/// the speed multiplier to apply when walking on a slope
		[Tooltip("the speed multiplier to apply when walking on a slope")]
		public AnimationCurve SlopeAngleSpeedFactor = new AnimationCurve(new Keyframe(-90f,1f),new Keyframe(0f,1f),new Keyframe(90f,1f));


        public GameObject StandingOn;
        /// the object the character was standing on last frame
        public GameObject StandingOnLastFrame { get; protected set; }
		/// gives you the collider the character is standing on
		public Collider2D StandingOnCollider { get; protected set; }	

		public GameObject CurrentWallCollider;

		
		public PhysicsControllerConditions Conditions { get; protected set; }
        private BoxCollider2D boxCollider;

		private Vector2 speed=Vector2.zero;
        private Vector2 newPosition=Vector2.zero;
		private Vector2 externalForce;

		private float currentGravity=0f;
        private float movingPlatformCurrentGravity=0f;
		private float movementDirection;
		private float storedMovementDirection;		
		private const float movementDirectionThreshold = 0.0001f;

        private bool gravityActive=true;

        public readonly float initialGravity=-30f;
        private readonly float fallMultiplier=1f;
        private readonly float ascentMultiplier=1f;

        private readonly float _smallValue=0.0001f;
		private readonly float obstacleHeightTolerance=0.05f;
		private float maximumSlopeAngle = 30f;
		public  float MaximumSlopeAngle
        {
            get
            {
				return maximumSlopeAngle;
            }
            set
            {
				if(value>=0)
                {
					maximumSlopeAngle = value;
					Debug.Log($"Maximum Slope Angel Change from {maximumSlopeAngle} to {value}");
				}
					
				
            }
        }
		Vector3 crossBelowSlopeAngle;



        Vector2 boundsTopLeftCorner;
        Vector2 boundsTopRightCorner;
        Vector2 boundsBottomLeftCorner;
        Vector2 boundsBottomRightCorner;

        Vector2 boundsCenter;


        private Vector2 verticalRayCastFromLeft = Vector2.zero;
		private Vector2 verticalRayCastToRight = Vector2.zero;

		private Vector2 horizontalRayCastFromBottom=Vector2.zero;
		private Vector2 horizontalRayCastToTop=Vector2.zero;


        float boundsWidth;
        float boundsHeight;


        RaycastHit2D[] belowHitsStorage;	
		RaycastHit2D[] sideHitsStorage;


		public Vector2 Speed { get{ return speed; } }
		public Vector2 ExternalForce { get { return externalForce; } }



		private void Awake() {
            
            boxCollider=this.GetComponent<BoxCollider2D>();
            belowHitsStorage=new RaycastHit2D[numberOfVerticalRays];
			sideHitsStorage=new RaycastHit2D[numberOfHorizontalRays];

			Conditions = new PhysicsControllerConditions(); 

        }

		private void FrameInitialization()
		{
			//contactList.Clear();
			// we initialize our newposition, which we'll use in all the next computations			
			newPosition = Speed * Time.deltaTime;
			Conditions.WasGroundedLastFrame = Conditions.IsCollidingBelow;          
            StandingOnLastFrame = StandingOn;
			Conditions.WasTouchingTheCeilingLastFrame = Conditions.IsCollidingAbove;
			CurrentWallCollider = null;
			Conditions.Reset(); 
		}

        private void Update() 
        { 
            if (Time.timeScale == 0f)
            {
                return;
            }
			
            SetGravity();
			FrameInitialization();

            SetRaysParameters();

          
			
			DetermineMovementDirection();
           
			//Raycast
      		if(movementDirection==-1)
				CastRayToLeft();
			else 
				CastRayToRight();
			
			CastRaysBelow();


			//MoveTransform
            this.transform.Translate(newPosition, Space.Self);

			SetRaysParameters();
			ComputeNewSpeed();

			if( !Conditions.WasGroundedLastFrame && Conditions.IsCollidingBelow )
			{
				Conditions.JustGotGrounded=true;
			}

			externalForce.x = 0;
			externalForce.y = 0;


		}


		private void SetGravity()
        {
            currentGravity=initialGravity;

            if(speed.y<0)
            {
                currentGravity=currentGravity*fallMultiplier;
            }
            else if(speed.y>0)
            {
                currentGravity=currentGravity/ascentMultiplier;
            }
            
            if (gravityActive)
			{
				speed.y += (currentGravity + movingPlatformCurrentGravity) * Time.deltaTime;
			}

			// if (fallSlowFactor!=0)
			// {
			// 	speed.y*=fallSlowFactor;
			// }
        }

        public void SetRaysParameters() 
		{		
			float top = boxCollider.offset.y + (boxCollider.size.y / 2f);
			float bottom = boxCollider.offset.y - (boxCollider.size.y / 2f);
			float left = boxCollider.offset.x - (boxCollider.size.x / 2f);
			float right = boxCollider.offset.x + (boxCollider.size.x /2f);

			boundsTopLeftCorner.x = left;
			boundsTopLeftCorner.y = top;

			boundsTopRightCorner.x = right;
			boundsTopRightCorner.y = top;

			boundsBottomLeftCorner.x = left;
			boundsBottomLeftCorner.y = bottom;

			boundsBottomRightCorner.x = right;
			boundsBottomRightCorner.y = bottom;

			boundsTopLeftCorner = this.transform.TransformPoint (boundsTopLeftCorner);
			boundsTopRightCorner = this.transform.TransformPoint (boundsTopRightCorner);
			boundsBottomLeftCorner = this.transform.TransformPoint (boundsBottomLeftCorner);
			boundsBottomRightCorner = this.transform.TransformPoint (boundsBottomRightCorner);
			boundsCenter = boxCollider.bounds.center;

			boundsWidth = Vector2.Distance (boundsBottomLeftCorner, boundsBottomRightCorner);
			boundsHeight = Vector2.Distance (boundsBottomLeftCorner, boundsTopLeftCorner);
		}

        void CastRaysBelow()
        {
            float rayLength = (boundsHeight / 2) + RayOffset; 	

            if(newPosition.y<0)
            {
                rayLength+=Mathf.Abs(newPosition.y);
            }

            verticalRayCastFromLeft = (boundsBottomLeftCorner + boundsTopLeftCorner) / 2;
			verticalRayCastToRight = (boundsBottomRightCorner + boundsTopRightCorner) / 2;	
			verticalRayCastFromLeft += (Vector2)transform.up * RayOffset;
			verticalRayCastToRight += (Vector2)transform.up * RayOffset;
			verticalRayCastFromLeft += (Vector2)transform.right * newPosition.x;
			verticalRayCastToRight += (Vector2)transform.right * newPosition.x;

            float smallestDistance = float.MaxValue; 
			int smallestDistanceIndex = 0; 						
			bool hitConnected = false; 		

			for (int i = 0; i < numberOfVerticalRays; i++)
            {
                Vector2 rayOriginPoint = Vector2.Lerp(verticalRayCastFromLeft, verticalRayCastToRight, (float)i / (float)(numberOfVerticalRays - 1));

                belowHitsStorage[i]=Physics2D.Raycast(rayOriginPoint,-transform.up,rayLength,PlatformMask);
                
                float distance=DistanceBetweenPointAndLine(belowHitsStorage[smallestDistanceIndex].point,verticalRayCastFromLeft,verticalRayCastToRight);

                if(belowHitsStorage[i])
                {
                    // if(belowHitsStorage[i].collider==ignoreCollier)
                    //     continue;

                    hitConnected=true;

					Conditions.BelowSlopeAngle = Vector2.Angle( belowHitsStorage[i].normal, transform.up )  ;
					crossBelowSlopeAngle = Vector3.Cross (transform.up, belowHitsStorage [i].normal);
					if (crossBelowSlopeAngle.z < 0)
					{
						Conditions.BelowSlopeAngle = -Conditions.BelowSlopeAngle;
					}


                    if(belowHitsStorage[i].distance<smallestDistance)
                    {
                        smallestDistance=belowHitsStorage[i].distance;
                        smallestDistanceIndex=i;
                    }


                }

                if (distance < _smallValue)
                {
                    break;
                }

            }

            if (hitConnected)
			{

				Conditions.IsFalling = false;			
				Conditions.IsCollidingBelow = true;


				StandingOn = belowHitsStorage[smallestDistanceIndex].collider.gameObject;
				StandingOnCollider = belowHitsStorage [smallestDistanceIndex].collider;

				// // if we're applying an external force (jumping, jetpack...) we only apply that
				if (speed.y > 0&& externalForce.y > 0)
				{
					newPosition.y = speed.y * Time.deltaTime;
					Conditions.IsCollidingBelow = false;
				}
				// // if not, we just adjust the position based on the raycast hit
				else
				{
					float distance = DistanceBetweenPointAndLine (belowHitsStorage [smallestDistanceIndex].point, verticalRayCastFromLeft, verticalRayCastToRight);

					newPosition.y = -distance+ boundsHeight / 2 + RayOffset;
				}

				if (speed.y>0&&!Conditions.WasGroundedLastFrame)//(!State.WasGroundedLastFrame && _speed.y > 0)
				{
					newPosition.y += speed.y * Time.deltaTime;
				}				

				if (Mathf.Abs(newPosition.y) < _smallValue)
                {
                    newPosition.y = 0;
                }					

				// // we check if whatever we're standing on applies a friction change
				// _frictionTest = _belowHitsStorage[smallestDistanceIndex].collider.gameObject.MMGetComponentNoAlloc<SurfaceModifier>();
				// if ((_frictionTest != null) && (_frictionTest.enabled))
				// {
				// 	_friction = _belowHitsStorage[smallestDistanceIndex].collider.GetComponent<SurfaceModifier>().Friction;
				// }

				// // we check if the character is standing on a moving platform
				// _movingPlatformTest = _belowHitsStorage[smallestDistanceIndex].collider.gameObject.MMGetComponentNoAlloc<MMPathMovement>();
				// if (_movingPlatformTest != null && State.IsGrounded)
				// {
				// 	_movingPlatform=_movingPlatformTest.GetComponent<MMPathMovement>();
				// }
				// else
				// {
				// 	DetachFromMovingPlatform();
				// }
			}
			else
			{
				 Conditions.IsCollidingBelow = false;
			}

			//if (StickToSlopes)
			//{
			//	StickToSlope();
			//}
		}

		void CastRayToLeft()
		{
			CastRaysToTheSides(-1);
		}

		void CastRayToRight()
		{
			CastRaysToTheSides(1);
		}

		void CastRaysToTheSides(float raysDirection) 
		{	
            // we determine the origin of our rays
			horizontalRayCastFromBottom = (boundsBottomRightCorner + boundsBottomLeftCorner) / 2;
			horizontalRayCastToTop = (boundsTopLeftCorner + boundsTopRightCorner) / 2;	
			horizontalRayCastFromBottom = horizontalRayCastFromBottom + (Vector2)transform.up * obstacleHeightTolerance;
			horizontalRayCastToTop = horizontalRayCastToTop - (Vector2)transform.up * obstacleHeightTolerance;

			// we determine the length of our rays
			float horizontalRayLength = Mathf.Abs(speed.x * Time.deltaTime) + boundsWidth / 2 + RayOffset * 2;

			// we resize our storage if needed
			if (sideHitsStorage.Length != numberOfHorizontalRays)
			{
				sideHitsStorage = new RaycastHit2D[numberOfHorizontalRays];	
			}
                        
            // we cast rays to the sides
            for (int i=0; i < numberOfHorizontalRays;i++)
			{	
				Vector2 rayOriginPoint = Vector2.Lerp(horizontalRayCastFromBottom, horizontalRayCastToTop, (float)i / (float)(numberOfHorizontalRays-1));

				sideHitsStorage[i] = Physics2D.Raycast(rayOriginPoint,raysDirection*(transform.right),horizontalRayLength,PlatformMask);	
				
				// if we've hit something
				if (sideHitsStorage[i].distance > 0)
				{	
					// if this collider is on our ignore list, we break
					// if (sideHitsStorage[i].collider == ignoredCollider)
					// {
					// 	break;
					// }
                    
					// we determine and store our current lateral slope angle
					float hitAngle = Mathf.Abs(Vector2.Angle(sideHitsStorage[i].normal, transform.up));
					
					if (movementDirection == raysDirection)
                    {
                        Conditions.LateralSlopeAngle = hitAngle;
                    }        

					// if the lateral slope angle is higher than our maximum slope angle, then we've hit a wall, and stop x movement accordingly
					if (hitAngle > MaximumSlopeAngle)
					{
						if (raysDirection < 0)
						{
							Conditions.IsCollidingLeft = true;
                            Conditions.DistanceToLeftCollider = sideHitsStorage[i].distance;
						} 
						else
						{
							Conditions.IsCollidingRight = true;
                            Conditions.DistanceToRightCollider = sideHitsStorage[i].distance;
                        }

                        if(movementDirection == raysDirection) 
                        {
                            CurrentWallCollider = sideHitsStorage[i].collider.gameObject;
                            Conditions.SlopeAngleOK = false;

                            float distance = DistanceBetweenPointAndLine(sideHitsStorage[i].point, horizontalRayCastFromBottom, horizontalRayCastToTop);
                            if (raysDirection <= 0)
                            {
								//HitDistance+RayLength
                                newPosition.x = -distance+ boundsWidth / 2 + RayOffset * 2;
								
								
                            }
                            else
                            {
                                newPosition.x = distance- boundsWidth / 2 - RayOffset * 2;
                            }

                            // if we're in the air, we prevent the character from being pushed back.
                            if (!Conditions.IsGrounded && (Speed.y != 0) && (!Mathf.Approximately(hitAngle, 90f)))
                            {
                                newPosition.x = 0;
                            }

                            //contactList.Add(_sideHitsStorage[i]);
                            speed.x = 0;
                        }

						break;
					}
				}						
			}


		}

		private void ComputeNewSpeed()
		{
			// we compute the new speed
			if (Time.deltaTime > 0)
			{
				speed = newPosition / Time.deltaTime;	
			}	

			// we apply our slope speed factor based on the slope's angle
			if (Conditions.IsGrounded)
			{
				speed.x *= SlopeAngleSpeedFactor.Evaluate(Mathf.Abs(Conditions.BelowSlopeAngle) * Mathf.Sign(speed.y));
			}

			if (!Conditions.OnAMovingPlatform)				
			{
				// we make sure the velocity doesn't exceed the MaxVelocity specified in the parameters
				speed.x = Mathf.Clamp(speed.x,-MaxVelocity.x,MaxVelocity.x);
				speed.y = Mathf.Clamp(speed.y,-MaxVelocity.y,MaxVelocity.y);
			}
		}

		private void DetermineMovementDirection()
        {
            movementDirection = storedMovementDirection;
            if (speed.x < -movementDirectionThreshold)
            {
                movementDirection = -1;
            } 
			else if (speed.x > movementDirectionThreshold)
            {
                movementDirection = 1;
            }
            else if (externalForce.x < -movementDirectionThreshold)
            {
                movementDirection = -1;
            }
            else if (externalForce.x > movementDirectionThreshold)
            {
                movementDirection = 1;
            }

            // if (_movingPlatform != null)
            // {
            //     if (Mathf.Abs(_movingPlatform.CurrentSpeed.x) > Mathf.Abs(_speed.x))
            //     {
            //        _movementDirection = Mathf.Sign(_movingPlatform.CurrentSpeed.x);
            //     }
            // }
            storedMovementDirection = movementDirection;                        
        }

		public void SetVerticalForce (float y)
		{
			speed.y = y;
			externalForce.y = y;

		}

		public void AddVerticalForce(float y)
		{
			speed.y += y;
			externalForce.y += y;
		}

		public void SetHorizontalForce (float x)
		{
			speed.x = x;
			externalForce.x = x;
		}

		public void AddHorizontalForce(float x)
		{
			speed.x += x;
			externalForce.x += x;
		}

		public void GravityActive(bool state)
		{
		    gravityActive = state;
		}


        public float DistanceBetweenPointAndLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
		{
			return Vector3.Magnitude(ProjectPointOnLine(point, lineStart, lineEnd) - point);
		}

        public Vector3 ProjectPointOnLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
		{
			Vector3 rhs = point - lineStart;
			Vector3 vector2 = lineEnd - lineStart;
			float magnitude = vector2.magnitude;
			Vector3 lhs = vector2;
			if (magnitude > 1E-06f)
			{
				lhs = (Vector3)(lhs / magnitude);
			}
			float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
			return (lineStart + ((Vector3)(lhs * num2)));
		}
		

    }

}

