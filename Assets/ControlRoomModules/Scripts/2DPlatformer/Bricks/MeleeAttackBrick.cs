using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ControlRoom
{
    public class MeleeAttackBrick : Brick
    {
        public AreaColliderType colliderType= AreaColliderType.BoxCollider;
        /// the size of the damage area
		[Tooltip("the size of the damage area")]
        public Vector2 AreaSize = new Vector2(1, 1);
        /// the offset to apply to the damage area (from the weapon's attachment position
        [Tooltip("the offset to apply to the damage area (from the weapon's attachment position")]
        public Vector2 AreaOffset = new Vector2(1, 0);
        public float InitialDelay = 0f;
        public float ActiveDuration = 0.3f;
        public LayerMask TargetLayerMask;
        public int DamageCaused;
        public bool ShowGizmosWhenPlaying = false;


        private GameObject attackArea;
        private BoxCollider2D boxCollider;
        private CircleCollider2D circleCollider;
        private Collider2D attackCollider;
        private OnDamage damager;
        private Vector3 flipVector = new Vector3(-1, 1, 1);
        private bool isAttacking = false;
        private const string attackAnimationParameterName = "Attack";
        private int attackAnimationParameter;
        private Vector3 gizmoOffset;

        public enum AreaColliderType
        {
            BoxCollider,
            CircleCollider,
        }

        protected override void Initialize()
        {
            base.Initialize();

            CreateAttackArea();
            DisableAttackArea();
        }

        protected override void HandleInput()
        {
            base.HandleInput();

            if(Input.GetKeyDown(KeyCode.I))
            {
                PlayAttack();
            }

        }

        protected override void UpdateBrickProcess()
        {
            base.UpdateBrickProcess();
        }

        private void CreateAttackArea()
        {
            if(attackArea==null)
            {
                attackArea = new GameObject("AttackArea");
                attackArea.transform.position = this.transform.position;
                attackArea.transform.rotation = this.transform.rotation;
                attackArea.transform.parent = this.transform;
                attackArea.layer = this.agent.gameObject.layer;

                switch(this.colliderType)
                {
                    case AreaColliderType.BoxCollider:
                        this.boxCollider = attackArea.AddComponent<BoxCollider2D>();
                        this.boxCollider.offset = AreaOffset;
                        this.boxCollider.size = AreaSize;
                        this.attackCollider = this.boxCollider;

                        break;
                    case AreaColliderType.CircleCollider:
                        this.circleCollider = attackArea.AddComponent<CircleCollider2D>();
                        this.circleCollider.radius = AreaSize.x / 2;
                        this.circleCollider.offset = AreaOffset;
                        this.attackCollider = this.circleCollider;
                        break;
                }

                this.attackCollider.isTrigger = true;

                Rigidbody2D rigidBody = this.attackArea.AddComponent<Rigidbody2D>();
                rigidBody.isKinematic = true;

                damager = this.attackArea.AddComponent<OnDamage>();
                damager.targetLayerMask = TargetLayerMask;
                damager.DamageCaused = DamageCaused;
                //damager.DamageCausedKnockbackType = Knockback;
                //damager.DamageCausedKnockbackForce = KnockbackForce;
                //damager.InvincibilityDuration = InvincibilityDuration;
            }
        }

        public void PlayAttack()
        {
            StartCoroutine(AttackCouroutine());
        }

        private void EnableAttackArea()
        {
           if (this.attackCollider != null)
                this.attackCollider.enabled = true;
        }

        private void DisableAttackArea()
        {
            if(this.attackCollider!=null)
                this.attackCollider.enabled = false;
        }

        private IEnumerator AttackCouroutine()
        {
            if (isAttacking) { yield break; }

            isAttacking = true;

            
            if(controller.Conditions.IsFalling)
            {
                controller.GravityActive(false);
                controller.Conditions.IsFalling = false;

            }

            animationController.UpdateAnimatorTrigger(attackAnimationParameter);

            yield return new WaitForSeconds(InitialDelay);
            EnableAttackArea();
           

            yield return new WaitForSeconds(ActiveDuration);
            DisableAttackArea();
            isAttacking = false;

            controller.GravityActive(true);
        }

        public override void Flip()
        {
            if(attackArea!=null)
            {
                attackArea.transform.localScale = Vector3.Scale(attackArea.transform.localScale, flipVector);
            }
        }

        protected override void InitializeAnimParam()
        {
            animationController.AddAnimatorParameterIfExists(attackAnimationParameterName, out attackAnimationParameter, AnimatorControllerParameterType.Trigger);
        }

        private void DrawGizmos()
        {
            gizmoOffset = AreaOffset;

            Gizmos.color = Color.red;
            if (this.colliderType== AreaColliderType.CircleCollider)
            {
                Gizmos.DrawWireSphere(this.transform.position + gizmoOffset, AreaSize.x / 2);
            }
            else if (this.colliderType == AreaColliderType.BoxCollider)
            {
                
                DrawGizmoRectangle(this.transform.position + gizmoOffset, AreaSize, Color.red);
            }
        }

        /// <summary>
        /// Draws gizmos on selected if the app is not playing
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            if(ShowGizmosWhenPlaying==false)
            {
                if (!Application.isPlaying)
                {
                    DrawGizmos();
                }

            }
            else
            {
                DrawGizmos();
            }
            
        }

        private void DrawGizmoRectangle(Vector2 center, Vector2 size, Color color)
        {
          
            Gizmos.color = color;

            Vector3 v3TopLeft = new Vector3(center.x - size.x / 2, center.y + size.y / 2, 0);
            Vector3 v3TopRight = new Vector3(center.x + size.x / 2, center.y + size.y / 2, 0); ;
            Vector3 v3BottomRight = new Vector3(center.x + size.x / 2, center.y - size.y / 2, 0); ;
            Vector3 v3BottomLeft = new Vector3(center.x - size.x / 2, center.y - size.y / 2, 0); ;

            Gizmos.DrawLine(v3TopLeft, v3TopRight);
            Gizmos.DrawLine(v3TopRight, v3BottomRight);
            Gizmos.DrawLine(v3BottomRight, v3BottomLeft);
            Gizmos.DrawLine(v3BottomLeft, v3TopLeft);
        }


    }
}

