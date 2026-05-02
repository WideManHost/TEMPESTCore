using UnityEngine;
using ULTRAKILL.Cheats;

namespace TEMPESTCore
{
    /// <summary>
    /// validator subclass that checks for flags. originally an Enum to try to put all validators 
    /// in a single list component but i couldnt be bothered
    /// enemy events will be a bit rough because of it but i dont mind
    /// </summary>
    public class ValidatorEnemyFlag : Validator
    {
        private EnemyIdentifier _eid;
        private IEnrage _enrageComponent;

        public EnemyIdentifier EID => _eid;
        public IEnrage EnrageComponent => _enrageComponent;

        public override void Initialize(EnemyIdentifier eid, IEnrage enrage)
        {
            _eid = eid;
            _enrageComponent = enrage;
        }

        public override bool Validate(EnemyFlags flags, DifficultyRequirement difficulty)
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

            // Enrage check, skips if the enragecomponent is null
            if (HasFlag(flags, EnemyFlags.IfEnraged)|| _enrageComponent != null)
            {
                bool isEnraged = _enrageComponent != null && _enrageComponent.isEnraged;
                if (!isEnraged)
                    return false;
            }

            return true;
        }

        private bool HasFlag(EnemyFlags allFlags, EnemyFlags flagToCheck)
        {
            return (allFlags & flagToCheck) == flagToCheck;
        }
    }
}