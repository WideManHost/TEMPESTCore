using JetBrains.Annotations;
using UnityEngine;

namespace WideUtilities
{
    [System.Serializable]
    public class EnemyTimedEvent
    {
        [Header("Timed Event")]
        public bool activated;
        public string eventName;
        public bool deactivateOnTimerExpire;
        public DifficultyVariance difficultyVariance = new DifficultyVariance();
        public EnemyEventFlags flags;
        public DifficultyRequirement difficultyRequirement;
        private float _timer;
        [Tooltip("X: Min Time, Y: Max Time")]
        public Vector2 TimerRange;
        public UltrakillEvent onTimerExpire;
        private float _currentMultiplier = 1f;
        private EnemyFlagChecker _flagChecker;
        public void Initialize(EnemyIdentifier eid, Enemy enemy)
        {
            _flagChecker = new EnemyFlagChecker(eid, enemy);
            _currentMultiplier = difficultyVariance.Calculate(1f);
        }
        public bool Validate()
        {
            if (!activated || _flagChecker == null)
                return false;

            return _flagChecker.CheckForFlags(flags);
        }
        public void Tick()
        {
            if (!activated || _flagChecker == null)
                return;
            _timer -= Time.deltaTime * _currentMultiplier;

            if (_timer <= 0f)
            {
                ExpireTimer();
            }
        }
        private void ExpireTimer()
        {
            if (_flagChecker.CheckForFlags(flags)) onTimerExpire?.Invoke();
            if (deactivateOnTimerExpire) activated = false;
            else Randomize();
        }
        public void Randomize()
        {
            float min = Mathf.Min(TimerRange.x, TimerRange.y);
            float max = Mathf.Max(TimerRange.x, TimerRange.y);

            _timer = Random.Range(min, max);
        }

    }
}
