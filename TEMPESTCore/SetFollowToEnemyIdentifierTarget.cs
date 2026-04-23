using System;
using System.Collections.Generic;
using ULTRAKILL;
using UnityEngine;
namespace TEMPESTCore
{
    public class SetFollowToEnemyIdentifierTarget
    {
        public Follow follow;
        public EnemyIdentifier targetIdentifier;
        [Tooltip("Set Target OnStart?")]
        public bool onStart;


        private void Start()
        {
            if (onStart)
            {
                SetTarget();
            }
        }

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
