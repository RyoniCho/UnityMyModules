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


        private GameObject attackArea;
        private BoxCollider2D boxCollider;
        private CircleCollider2D circleCollider;
        private Collider2D collider;
        private Vector3 flipVector = new Vector3(-1, 1, 1);
        public enum AreaColliderType
        {
            BoxCollider,
            CircleCollider,
        }

        protected override void Initialize()
        {
            base.Initialize();

            CreateAttackArea();
        }

        protected override void HandleInput()
        {
            base.HandleInput();
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
                        this.collider = this.boxCollider;

                        break;
                    case AreaColliderType.CircleCollider:
                        this.circleCollider = attackArea.AddComponent<CircleCollider2D>();
                        this.circleCollider.radius = AreaSize.x / 2;
                        this.circleCollider.offset = AreaOffset;
                        this.collider = this.circleCollider;
                        break;
                }

                this.collider.isTrigger = true;

                //Rigidbody2D rigidBody = this.collider.AddComponent<Rigidbody2D>();
                //rigidBody.isKinematic = true;

                //_damageOnTouch = this.collider.AddComponent<DamageOnTouch>();
                //_damageOnTouch.TargetLayerMask = TargetLayerMask;
                //_damageOnTouch.DamageCaused = DamageCaused;
                //_damageOnTouch.DamageCausedKnockbackType = Knockback;
                //_damageOnTouch.DamageCausedKnockbackForce = KnockbackForce;
                //_damageOnTouch.InvincibilityDuration = InvincibilityDuration;
            }
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

