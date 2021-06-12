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





        private GameObject attackArea;
        private BoxCollider2D boxCollider;
        private CircleCollider2D circleCollider;
        private Collider2D attackCollider;
        private OnDamage damager;
        private Vector3 flipVector = new Vector3(-1, 1, 1);
        private bool isAttacking = false;

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
            yield return new WaitForSeconds(InitialDelay);
            EnableAttackArea();
           
            yield return new WaitForSeconds(ActiveDuration);
            DisableAttackArea();
            isAttacking = false;
        }

        public override void Flip()
        {
            if(attackArea!=null)
            {
                attackArea.transform.localScale = Vector3.Scale(attackArea.transform.localScale, flipVector);
            }
        }
    }
}

