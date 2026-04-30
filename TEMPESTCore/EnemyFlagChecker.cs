using UnityEngine;
using ULTRAKILL.Cheats;
namespace TEMPESTCore
{

    [System.Serializable]
    public class EnemyFlagChecker
    {
        [SerializeField] private EnemyIdentifier _eid;
        [SerializeField] private Enemy _enemy;

        public EnemyIdentifier EID => _eid;
        public Enemy Enemy => _enemy;

        public EnemyFlagChecker() { }

        public EnemyFlagChecker(EnemyIdentifier eid, Enemy enemy)
        {
            _eid = eid;
            _enemy = enemy;
        }

        public bool CheckForFlags(EnemyEventFlags flags)
        {
            if (flags == EnemyEventFlags.None)
                return true;

            if (_eid == null)
                return false;

            if (HasFlag(flags, EnemyEventFlags.NotIfDead) && _eid.dead)
                return false;

            if (HasFlag(flags, EnemyEventFlags.NotIfNoTarget) && _eid.target == null)
                return false;

            if (HasFlag(flags, EnemyEventFlags.NotIfSanded) && _eid.sandified)
                return false;

            if (HasFlag(flags, EnemyEventFlags.NotIfRadiant) && _eid.radianceTier > 0)
                return false;

            if (HasFlag(flags, EnemyEventFlags.NotIfPuppeted) && _eid.puppet)
                return false;

            if (HasFlag(flags, EnemyEventFlags.NotIfBlind) && BlindEnemies.Blind)
                return false;

            if (HasFlag(flags, EnemyEventFlags.NotIfBlessed) && _eid.blessed)
                return false;

            if (HasFlag(flags, EnemyEventFlags.NotIfUnderwater) && _eid.underwater)
                return false;

            if (HasFlag(flags, EnemyEventFlags.NotIfHarpooned) && _eid.harpooned)
                return false;

            if (HasFlag(flags, EnemyEventFlags.NotIfGasolined) && _eid.beenGasolined)
                return false;

            if (HasFlag(flags, EnemyEventFlags.NotIfDrilled) && _eid.drillers.Count > 0)
                return false;

            if (HasFlag(flags, EnemyEventFlags.IfEnraged))
            {
                bool isEnraged = _enemy != null && _enemy.isEnraged;
                if (!isEnraged)
                    return false;
            }

            return true;
        }

        private bool HasFlag(EnemyEventFlags allFlags, EnemyEventFlags flagToCheck)
        {
            return (allFlags & flagToCheck) == flagToCheck;
        }

        public void SetReferences(EnemyIdentifier eid, Enemy enemy)
        {
            _eid = eid;
            _enemy = enemy;
        }
    }
}