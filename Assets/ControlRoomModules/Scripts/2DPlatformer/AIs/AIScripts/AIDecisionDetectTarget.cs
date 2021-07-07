using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class AIDecisionDetectTarget : AIDecision
    {
        public float Radius = 3f;
        public LayerMask TargetLayer;
        public Vector3 DetectionOriginOffset = new Vector3(0, 0, 0);

        private Collider2D _detectionCollider = null;
        protected Vector2 _facingDirection;
        protected Vector2 _raycastOrigin;
        private Agent agent;

        public override void Initialize()
        {
            base.Initialize();
            this.agent = this.gameObject.GetComponentInParent<Agent>();
        }
        public override bool Decide()
        {

            return DetectTarget();
        }

        private bool DetectTarget()
        {
            _detectionCollider = null;

            _facingDirection = agent.IsFacingRight ? Vector2.right : Vector2.left;
            // we cast a ray to the left of the agent to check for a Player
            _raycastOrigin.x = transform.position.x + _facingDirection.x * DetectionOriginOffset.x / 2;
            _raycastOrigin.y = transform.position.y + DetectionOriginOffset.y;

            _detectionCollider = Physics2D.OverlapCircle(_raycastOrigin, Radius, TargetLayer);
            if (_detectionCollider == null)
            {
                return false;
            }
            else
            {
                brain.Target = _detectionCollider.gameObject.transform;
                return true;
            }
        }
    }

}
