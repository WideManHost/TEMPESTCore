using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WideUtilities
{
    /// <summary>
    /// Just a base script for me to use whenever hines wasnts something to have on start/update and whatever functionality.
    /// </summary>
    public abstract class ActiveStateChecker : MonoBehaviour
    {
        [Tooltip("Does this initially check on Start()?")]
        public bool onStart;
        [Tooltip("Does this check on every Update()?")]
        public bool onUpdate;

        private void Start()
        {
            if (onStart)
            {
                OnStart();
            }
        }

      
        private void Update()
        {
            if (onUpdate)
            {
                OnUpdate();
            }
        }

        // go my subclasses, give this a purpose
        protected void OnStart()
        {
            FireBehavior();
        }
        protected void OnUpdate()
        {
            FireBehavior();
        }

        /// <summary>
        /// This is where you would do your behaviour in your inherited classes, make sure to do error checks!
        /// </summary>
        public abstract void FireBehavior();
    }
}
