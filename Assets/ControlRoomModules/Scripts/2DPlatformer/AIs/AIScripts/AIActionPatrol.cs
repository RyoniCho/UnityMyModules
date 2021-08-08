using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class AIActionPatrol : AIAction
    {
        public float BoundsExtentsLeft;
        public float BoundsExtentsRight;


        Agent agent;
        PhysicsController controller;
        HorizontalMovementBrick horizontalMovementBrick;
        Vector2 startPosition;
        Vector2 direction;

        Vector2 boundsLeft;
        Vector2 boundsRight;


        protected override void Initialization()
        {
            base.Initialization();

            this.agent = this.gameObject.GetComponentInParent<Agent>();
            this.controller = this.gameObject.GetComponentInParent<PhysicsController>();

            this.horizontalMovementBrick = agent?.FindBrick<HorizontalMovementBrick>();

            this.startPosition = this.transform.position;
            
            this.direction = agent.IsFacingRight? Vector2.right : Vector2.left;

            EstablishBounds();

        }

        public override void StartAction()
        {
            this.Patrol();
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
        }

        public override void OnExitState()
        {
            base.OnExitState();

            this.horizontalMovementBrick.SetHorizontalMove(0f);
        }

        private void Patrol()
        {
            CheckForDistance();
            CheckForObstacle();

            this.horizontalMovementBrick.SetHorizontalMove(direction.x);
        }

        private void CheckForObstacle()
        {
            if(DetectObstaclesRegularLayermask())
                this.direction = -this.direction;

        }

        private bool DetectObstaclesRegularLayermask()
        {
            return (this.direction.x < 0 && this.controller.Conditions.IsCollidingLeft) || (this.direction.x > 0 && this.controller.Conditions.IsCollidingRight);
        }

        private void EstablishBounds()
        {
            this.boundsLeft = this.transform.position + Vector3.left * BoundsExtentsLeft;
            this.boundsRight = this.transform.position + Vector3.right * BoundsExtentsRight;
        }

        private void CheckForDistance()
        {
            if (this.transform.position.x < this.boundsLeft.x)
            {
                this.direction = Vector2.right;
            }
            if (this.transform.position.x > this.boundsRight.x)
            {
                this.direction = Vector2.left;
            }
        }

    }
}

