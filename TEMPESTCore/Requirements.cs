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
        public bool requireTargetInRange;
        public bool outsideOfRadius;
        public float radius;
        private EnemyIdentifier _eid;
        public Transform _radiusCenter => _eid.GetCenter() == null ? _eid.transform : _eid.GetCenter();
        [System.NonSerialized] private ValidatorEnemyFlag enemy;
        [System.NonSerialized] private ValidatorDifficulty difficulty;

        public void Initialize(EnemyIdentifier eid, IEnrage enrage = null)
        {
            _eid = eid;
            enemy = new ValidatorEnemyFlag();
            difficulty = new ValidatorDifficulty();

            difficulty.Initialize(eid, enrage);
            enemy.Initialize(eid, enrage);
        }

        public bool Validate()
        { 
            if (!difficulty.Validate(enemyFlags, difficultyFlags)) return false;
            if (!enemy.Validate(enemyFlags, difficultyFlags)) return false;
            if (requireTargetInRange) return IsTargetInRange();
            return true;
        }
        //put this here coz its like supposed to be default behavior, just in case.
        //either way you can manually set this to off
        public void OnValidate()
        {
            enemyFlags |= EnemyFlags.NotIfDead;
            enemyFlags |= EnemyFlags.NotIfNoTarget;
        }
        public bool IsTargetInRange()
        {
            if (_eid == null || _eid.target == null) return false;

            float currentDistance = Vector3.Distance(_radiusCenter.position, _eid.target.position);
            bool isInside = currentDistance <= radius;
            return outsideOfRadius ? !isInside : isInside;
        }
    }
}
