using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class AIDecisionRandom : AIDecision
    {

        public int MaxPossiblity = 100;
        public int RandomRatio = 50;


        public override bool Decide()
        {
            return TryRamdomNumber();
        }

        private bool TryRamdomNumber()
        {
            int selectedNum=UnityEngine.Random.Range(1, MaxPossiblity+1);
            if (selectedNum <= RandomRatio)
                return true;

            return false;
        }
    }
}

