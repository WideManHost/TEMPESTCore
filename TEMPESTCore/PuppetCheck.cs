using System;
using System.Collections.Generic;
using ULTRAKILL;
using UnityEngine;

namespace WideUtilities
{
    /// <summary>
    /// Checks if this EnemyIdentifier is a puppet, if it is then it fires an ultrakill event.
    /// </summary>
    public class PuppetCheck : ActiveStateChecker
    {
        [Tooltip("The EnemyIdentifier that we're checking.")]
        public EnemyIdentifier thisIdentifier;
        
        [Header("Event")]
        [Tooltip("Fires after a check is successful.")]
        public UltrakillEvent onPuppetConfirmed = new UltrakillEvent();

        public void SetSelfIdentifier(EnemyIdentifier selfIdentifier)
        {
            thisIdentifier = selfIdentifier;
        }

        /// <summary>
        /// Checks if they're a puppet.
        /// </summary>
        public override void FireBehavior() => Check();
       
          

        public void Check()
        {
            if (thisIdentifier == null)
            {
                throw new NullReferenceException("Cannot check for puppet as thisIdentifier is null!");
            }

            if (thisIdentifier.puppet)
            {
                onPuppetConfirmed?.Invoke();
            }
        }
    }
}
