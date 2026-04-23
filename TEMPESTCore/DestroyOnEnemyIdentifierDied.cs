using System;
using System.Collections.Generic;
using ULTRAKILL;
using UnityEngine;

namespace TEMPESTCore
{
    public class DestroyOnEnemyIdentifierDied : MonoBehaviour
    {
        public EnemyIdentifier enemyIdentifier;
        [Tooltip("Attempt DestroyIfDead() on Start()?")]
        public bool onStart;
        public bool onUpdate;

        public void DestroyIfDead()
        {
            if (enemyIdentifier != null)
                throw new NullReferenceException("enemyIdentifier is null! Cannot check if null is dead.");

            if (enemyIdentifier.dead)
                Destroy(this.gameObject);
        }


        private void Start()
        {
            if (onStart)
                DestroyIfDead();
        }

        private void Update()
        {
            if (onUpdate)
                DestroyIfDead();
        }
    }
}
