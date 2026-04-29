using System.Collections.Generic;
using ULTRAKILL;
using UnityEngine;

namespace WideUtilities
{
    /// <summary>
    /// Copies an EnemyIdentifier's Radiance/buff information and applies it to this EnemyIdentifier.
    /// </summary>
    public class CopyRadiance : MonoBehaviour
    {
        public EnemyIdentifier thisIdentifier;

        public void SetSelfIdentifier(EnemyIdentifier selfIdentifier)
        {
            thisIdentifier = selfIdentifier;
        }

        public void CopyFromIdentifier(EnemyIdentifier enemyIdentifier)
        {
            thisIdentifier.radianceTier = enemyIdentifier.radianceTier;
            thisIdentifier.healthBuff = enemyIdentifier.healthBuff;
            thisIdentifier.healthBuffModifier = enemyIdentifier.healthBuffModifier;
            thisIdentifier.speedBuff = enemyIdentifier.speedBuff;
            thisIdentifier.speedBuffModifier = enemyIdentifier.speedBuffModifier;
            thisIdentifier.damageBuff = enemyIdentifier.damageBuff;
            thisIdentifier.damageBuffModifier = enemyIdentifier.damageBuffModifier;
        }
    }
}
