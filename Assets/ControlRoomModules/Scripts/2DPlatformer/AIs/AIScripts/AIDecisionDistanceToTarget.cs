using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class AIDecisionDistanceToTarget : AIDecision
    {
        public float Distance;
        public enum ComparisonModes { StrictlyLowerThan, LowerThan, Equals, GreatherThan, StrictlyGreaterThan }
        public ComparisonModes comparisonModes = ComparisonModes.GreatherThan;

       
        public override bool Decide()
        {
            return EvaluateDistance();
        }

        private bool EvaluateDistance()
        {
            if (brain.Target == null)
            {
                return false;
            }

            float distance = Vector3.Distance(this.transform.position, brain.Target.position);

            if (comparisonModes == ComparisonModes.StrictlyLowerThan)
            {
                return (distance < Distance);
            }
            if (comparisonModes == ComparisonModes.LowerThan)
            {
                return (distance <= Distance);
            }
            if (comparisonModes == ComparisonModes.Equals)
            {
                return (distance == Distance);
            }
            if (comparisonModes == ComparisonModes.GreatherThan)
            {
                return (distance >= Distance);
            }
            if (comparisonModes == ComparisonModes.StrictlyGreaterThan)
            {
                return (distance > Distance);
            }
            return false;
        }
    }
}

