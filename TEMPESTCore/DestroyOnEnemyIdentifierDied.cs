using System;
using System.Collections.Generic;
using ULTRAKILL;
using UnityEngine;

namespace TEMPESTCore
{
    public class DestroyOnEnemyIdentifierDied : ActiveStateChecker
    {
        public EnemyIdentifier enemyIdentifier;


        public override void FireBehavior() => DestroyIfDead();

        public void DestroyIfDead()
        {
            if (enemyIdentifier != null)
                throw new NullReferenceException("enemyIdentifier is null! Cannot check if null is dead.");

            if (enemyIdentifier.dead)
                Destroy(this.gameObject);
        }
    }
}
