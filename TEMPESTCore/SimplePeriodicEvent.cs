using JetBrains.Annotations;
using UnityEngine;

namespace TEMPESTCore
{
    /// <summary>
    /// Class for timed events within EnemyIdentifier, checks flags too
    /// </summary>
    [System.Serializable]
    public class SimplePeriodicEvent : SimpleClassWithRequirements
    {
        [Header("Timed Event")]
        public bool deactivateOnTimerExpire;
        [Tooltip("use this to support radiance")]
        public bool affectedByEnemySpeed = true;
        private float _timer;
        [Space(10f)]
        [Tooltip("X: Min Time, Y: Max Time")]
        public Vector2 TimerRange;
        public UltrakillEvent onTimerExpire;

#if UNITY_EDITOR
        [SerializeField] private Comment comment;
#endif
        public override void Initialize(EnemyIdentifier eid, SimpleEvents se, IEnrage enemy = null)
        {
            base.Initialize(eid, se, enemy);
            Randomize();
        }
        public void Tick()
        {            
            if (!Validate())return;
            _timer = Mathf.MoveTowards(_timer, 0f, Time.deltaTime * (affectedByEnemySpeed ? _eid.totalSpeedModifier : 1f));
            if (_timer <= 0f)
            {
                ExpireTimer();
            }
        }
        private void ExpireTimer()
        {
            if (Validate()) onTimerExpire?.Invoke();
            if (deactivateOnTimerExpire) this.Deactivate(id);
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
