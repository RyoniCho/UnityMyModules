using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class AIActionAttack : AIAction
    {
        private Agent agent;
        private MeleeAttackBrick attackBrick;
        

        protected override void Initialization()
        {
            base.Initialization();

            this.agent = this.GetComponentInParent<Agent>();

            this.attackBrick = this.agent?.FindBrick<MeleeAttackBrick>();

            if(this.agent==null)
            {
                Debug.LogError($"{this.gameObject.name} : agent is not exists");
            }
            if(this.attackBrick==null)
            {
                Debug.LogError($"{this.gameObject.name} : attackBrick is not exists");
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        public override void StartAction()
        {
            this.attackBrick.PlayAttack();
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
        }

        public override void OnExitState()
        {
            base.OnExitState();
        }

        

        
    }

}
