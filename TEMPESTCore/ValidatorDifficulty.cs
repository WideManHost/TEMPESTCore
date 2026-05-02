using UnityEngine;
namespace TEMPESTCore
{
    /// <summary>
    /// title
    /// </summary>
    public class ValidatorDifficulty : Validator
    {
        private int _currentDiff = -1;
        public override void Initialize(EnemyIdentifier eid, IEnrage enrage)
        {
            _currentDiff = MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0);
        }

        public override bool Validate(EnemyFlags flags, DifficultyRequirement difficulty)
        {
            if (difficulty == DifficultyRequirement.None) return true;
            if (_currentDiff < 0)
            {
                Debug.LogError("Difficulty Not Initialized, skipping...");
                return true;
            }

            int currentMask = 1 << _currentDiff;

            return ((int)difficulty & currentMask) != 0;
        }
    }
}