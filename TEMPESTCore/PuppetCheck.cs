using System;
using System.Collections.Generic;
using ULTRAKILL;
using UnityEngine;

namespace TEMPESTCore
{
    /// <summary>
    /// Checks if this EnemyIdentifier is a puppet, if it is then it fires an ultrakill event.
    /// </summary>
    public class PuppetCheck : MonoBehaviour
    {
        [Tooltip("The EnemyIdentifier that we're checking.")]
        public EnemyIdentifier thisIdentifier;
        [Tooltip("Does this initially check on Start?")]
        public bool onStart;
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
        public void Check()
        {
            if (thisIdentifier == null)
            {
                Debug.LogError("Cannot check for puppet as thisIdentifier is null!", this);
                return;
            }

            if (thisIdentifier.puppet)
            {
                onPuppetConfirmed?.Invoke();
            }
        }

        public void Start()
        {
            if (onStart)
            {
                Check();
            }
        }
    }
}
