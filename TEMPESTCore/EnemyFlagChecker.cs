    using UnityEngine;
    using ULTRAKILL.Cheats;
    namespace TEMPESTCore
    {
    /// <summary>
    /// DEPRECATED we use ValidatorEnemyFlag now
    /// </summary>
    [System.Serializable]
        public class EnemyFlagChecker
        {
            [SerializeField] private EnemyIdentifier _eid;
            [SerializeField] private IEnrage _enrageComponent;

            public EnemyIdentifier EID => _eid;
            public IEnrage Enemy => _enrageComponent;

            public EnemyFlagChecker() { }

            public EnemyFlagChecker(EnemyIdentifier eid, IEnrage enemy)
            {
                _eid = eid;
                _enrageComponent = enemy;
            }

            public bool CheckForFlags(EnemyFlags flags)
            {
                if (flags == EnemyFlags.None)
                    return true;

                if (_eid == null)
                    return false;

                if (HasFlag(flags, EnemyFlags.NotIfDead) && _eid.dead)
                    return false;

                if (HasFlag(flags, EnemyFlags.NotIfNoTarget) && _eid.target == null)
                    return false;

                if (HasFlag(flags, EnemyFlags.NotIfSanded) && _eid.sandified)
                    return false;

                if (HasFlag(flags, EnemyFlags.NotIfRadiant) && _eid.radianceTier > 0)
                    return false;

                if (HasFlag(flags, EnemyFlags.NotIfPuppeted) && _eid.puppet)
                    return false;

                if (HasFlag(flags, EnemyFlags.NotIfBlind) && BlindEnemies.Blind)
                    return false;

                if (HasFlag(flags, EnemyFlags.NotIfBlessed) && _eid.blessed)
                    return false;

                if (HasFlag(flags, EnemyFlags.NotIfUnderwater) && _eid.underwater)
                    return false;

                if (HasFlag(flags, EnemyFlags.NotIfHarpooned) && _eid.harpooned)
                    return false;

                if (HasFlag(flags, EnemyFlags.NotIfGasolined) && _eid.beenGasolined)
                    return false;

                if (HasFlag(flags, EnemyFlags.NotIfDrilled) && _eid.drillers.Count > 0)
                    return false;

                if (_enrageComponent != null)
                {
                    if (HasFlag(flags, EnemyFlags.IfEnraged))
                    {
                        bool isEnraged = _enrageComponent != null && _enrageComponent.isEnraged;
                        if (!isEnraged)
                            return false;
                    }
                }

                return true;
            }

            private bool HasFlag(EnemyFlags allFlags, EnemyFlags flagToCheck)
            {
                return (allFlags & flagToCheck) == flagToCheck;
            }

        }
    }