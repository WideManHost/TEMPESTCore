using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEMPESTCore
{
    /// <summary>
    /// class that holds basic requirements
    /// to be expanded upon
    /// basically just a pipeline
    /// </summary>
    [System.Serializable]
    public class Requirements 
    {
        public EnemyFlags enemyFlags;
        public DifficultyRequirement difficultyFlags;

        private ValidatorEnemyFlag enemy;
        private ValidatorDifficulty difficulty;

        public void Initialize(EnemyIdentifier eid, IEnrage enrage = null)
        {
            difficulty.Initialize(eid, enrage);
            enemy.Initialize(eid, enrage);
        }

        public bool Validate()
        { 
            if (!difficulty.Validate(enemyFlags, difficultyFlags)) return false;
            if (!enemy.Validate(enemyFlags, difficultyFlags)) return false;

            return true;
        }
        //put this here coz its like supposed to be default behavior, just in case.
        //either way you can manually set this to off
        public void OnValidate()
        {
            enemyFlags |= EnemyFlags.NotIfDead;
            enemyFlags |= EnemyFlags.NotIfNoTarget;
        }
    }
}
