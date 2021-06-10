using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class OnDamage : MonoBehaviour
    {

        public LayerMask targetLayerMask;
        public int DamageCaused = 10;
        public float InvincibilityDuration = 0.5f;

        private Collider2D currentCollidingCollider;
        private Health colliderHealth;
      
      


        private void OnTriggerEnter2D(Collider2D collision)
        {
            CheckOnDamage(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            CheckOnDamage(collision);
        }

        private void CheckOnDamage(Collider2D collider)
        {
            if (!this.isActiveAndEnabled)
                return;

            if (!LayerInLayerMask(collider.gameObject.layer, targetLayerMask))
            {
                return;
            }

            currentCollidingCollider = collider;

            colliderHealth = null;

            bool detectObject=collider.gameObject.TryGetComponent<Health>(out colliderHealth);

            //OnHit?.Invoke();

            if(detectObject&&colliderHealth.enabled)
            {
                if(colliderHealth.currentHealth>0)
                {
                    colliderHealth.Damage(DamageCaused, gameObject, InvincibilityDuration, InvincibilityDuration);

                }
            }

            //// if what we're colliding with is damageable
            //if ((_colliderHealth != null) && (_colliderHealth.enabled))
            //{
            //    if (_colliderHealth.CurrentHealth > 0)
            //    {
            //        OnCollideWithDamageable(_colliderHealth);
            //    }
            //}
            //// if what we're colliding with can't be damaged
            //else
            //{
            //    OnCollideWithNonDamageable();
            //}

        }


        public static bool LayerInLayerMask(int layer, LayerMask layerMask)
        {
            if (((1 << layer) & layerMask) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

