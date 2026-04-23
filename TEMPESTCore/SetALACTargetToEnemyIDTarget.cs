using System;
using System.Collections.Generic;
using ULTRAKILL;
using UnityEngine;

namespace TEMPESTCore
{
    public class SetALACTargetToEnemyIDTarget
    {
        public AlwaysLookAtCamera alwaysLookAtCamera;
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
            if (alwaysLookAtCamera == null)
                throw new NullReferenceException("alwaysLookAtCamera is set to null, cannot set alwaysLookAtCamera Target if it's null!");

            if (targetIdentifier == null)
                throw new NullReferenceException("TargetIdentifier is null! cannot set follow Target if it's null!");


            alwaysLookAtCamera.target = targetIdentifier.target;
        }
    }
}
