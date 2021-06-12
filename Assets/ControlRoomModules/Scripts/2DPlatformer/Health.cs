using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class Health : MonoBehaviour
    {
        public int currentHealth;
        public bool TemporaryInvulnerable = false;
        [Tooltip("the initial amount of health of the object")]
        public int InitialHealth = 10;

        /// the maximum amount of health of the object
        [Tooltip("the maximum amount of health of the object")]
        public int MaximumHealth = 10;

        /// if this is true, this object can't take damage
        [Tooltip("if this is true, this object can't take damage")]
        public bool Invulnerable = false;

        [Tooltip("should the sprite (if there's one) flicker when getting damage ?")]
        public bool FlickerSpriteOnHit = true;

        /// the color the sprite should flicker to
        [Tooltip("the color the sprite should flicker to")]
        public Color FlickerColor = new Color32(255, 20, 20, 255);
        private Color initialColor;
        private int lastDamage;
        private Vector2 lastDamageDirection;
        private Agent agent;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
           
            if(this.TryGetComponent<Agent>(out agent))
            {
                initialColor = agent.InitialColor;
            }
            else
            {
                Debug.LogError($"{this.gameObject.name}-(Health) Could not found Agent");
            }

            currentHealth = MaximumHealth;
            DamageEnabled();
        }

        /// <summary>
        /// Called when the object takes damage
        /// </summary>
        /// <param name="damage">The amount of health points that will get lost.</param>
        /// <param name="instigator">The object that caused the damage.</param>
        /// <param name="flickerDuration">The time (in seconds) the object should flicker after taking the damage.</param>
        /// <param name="invincibilityDuration">The duration of the short invincibility following the hit.</param>
        public virtual void Damage(int damage, GameObject instigator, float flickerDuration,
            float invincibilityDuration)
        {
            if (damage <= 0)
            {
                //OnHitZero?.Invoke();
                return;
            }

            // if the object is invulnerable, we do nothing and exit
            if (TemporaryInvulnerable || Invulnerable)
            {
                //OnHitZero?.Invoke();
                return;
            }

            // if we're already below zero, we do nothing and exit
            if ((currentHealth <= 0) && (InitialHealth != 0))
            {
                return;
            }

            // we decrease the character's health by the damage
            float previousHealth = currentHealth;
            currentHealth -= damage;

            lastDamage = damage;
            //lastDamageDirection = damageDirection;
            //OnHit?.Invoke();

            if (currentHealth < 0)
            {
                currentHealth = 0;
            }

            //// we prevent the character from colliding with Projectiles, Player and Enemies
            if (invincibilityDuration > 0)
            {
                DamageDisabled();
                StartCoroutine(DamageEnabled(invincibilityDuration));
            }

            //// we trigger a damage taken event
            //MMDamageTakenEvent.Trigger(_character, instigator, CurrentHealth, damage, previousHealth);

            //if (_animator != null)
            //{
            //    _animator.SetTrigger("Damage");
            //}

            //// we play the damage feedback
            //DamageFeedbacks?.PlayFeedbacks();

            if (FlickerSpriteOnHit)
            {
                // We make the character's sprite flicker
                if (agent != null)
                {
                   agent.FlickColor(initialColor, FlickerColor, 0.05f, flickerDuration);
                }
            }

            //// we update the health bar
            //UpdateHealthBar(true);

            //// if health has reached zero
            //if (currentHealth <= 0)
            //{
            //    // we set its health to zero (useful for the healthbar)
            //    currentHealth = 0;
            //    if (_character != null)
            //    {
            //        if (_character.CharacterType == Character.CharacterTypes.Player)
            //        {
            //            LevelManager.Instance.KillPlayer(_character);
            //            return;
            //        }
            //    }

            //    Kill();
            //}
        }

        /// <summary>
        /// Prevents the character from taking any damage
        /// </summary>
        public virtual void DamageDisabled()
        {
            TemporaryInvulnerable = true;
        }

        /// <summary>
        /// Allows the character to take damage
        /// </summary>
        public virtual void DamageEnabled()
        {
            TemporaryInvulnerable = false;
        }

        /// <summary>
        /// makes the character able to take damage again after the specified delay
        /// </summary>
        /// <returns>The layer collision.</returns>
        public virtual IEnumerator DamageEnabled(float delay)
        {
            yield return new WaitForSeconds(delay);
            TemporaryInvulnerable = false;
        }


    }
}

