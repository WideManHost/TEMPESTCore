using System;
using System.Collections.Generic;
using ULTRAKILL;
using UnityEngine;
namespace WideUtilities
{
    public class SetFollowToEnemyIdentifierTarget : ActiveStateChecker
    {
        public Follow follow;
        public EnemyIdentifier targetIdentifier;

        public override void FireBehavior() => SetTarget();

        public void SetTarget()
        {
            if (follow == null)
                throw new NullReferenceException("Follow is set to null, cannot set Follow Target if it's null!");

            if (targetIdentifier == null)
                throw new NullReferenceException("TargetIdentifier is null! cannot set follow Target if it's null!");

            follow.target = targetIdentifier.target.targetTransform;
        }
    }
}
