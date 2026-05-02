using JetBrains.Annotations;
using UnityEngine;

namespace TEMPESTCore
{
    /// <summary>
    /// Class for timed events within EnemyIdentifier, checks flags too
    /// </summary>
    [System.Serializable]
    public class SimpleTimedEvent
    {
        [Header("Timed Event")]
        public bool activated;
        public int eventID;
        public bool deactivateOnTimerExpire;
        public DifficultyVariance difficultyVariance = new DifficultyVariance();
        public Requirements requirements;
        [Space(10f)]
        private float _timer;
        [Tooltip("X: Min Time, Y: Max Time")]
        public Vector2 TimerRange;
        public UltrakillEvent onTimerExpire;
        private float _currentMultiplier = 1f;

#if UNITY_EDITOR
        [SerializeField] private Comment comment;
#endif

        public void Initialize(EnemyIdentifier eid, IEnrage enemy)
        {
            if (requirements == null) requirements.Initialize(eid, enemy);
            _currentMultiplier = difficultyVariance.Calculate(1f);
            Randomize();
        }
        public bool Validate()
        {
            if (!activated || requirements == null)
                return false;

            return requirements.Validate();
        }
        public void Tick()
        {
            if (!activated || requirements == null)
                return;
            if (!Validate())
                return;
            _timer -= Time.deltaTime * _currentMultiplier;

            if (_timer <= 0f)
            {
                ExpireTimer();
            }
        }
        private void ExpireTimer()
        {
            if (Validate()) onTimerExpire?.Invoke();
            if (deactivateOnTimerExpire) activated = false;
            else Randomize();
        }
        public void Randomize()
        {
            float min = Mathf.Min(TimerRange.x, TimerRange.y);
            float max = Mathf.Max(TimerRange.x, TimerRange.y);

            _timer = Random.Range(min, max);
        }
        public void Deactivate(int num)
        {
            if (this.eventID != num) return;
            activated = false;
        }
        public void Activate(int num)
        {
            if (this.eventID != num) return;
            activated = true;
        }
    }
}
