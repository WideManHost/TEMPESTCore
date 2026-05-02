using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TEMPESTCore
{
    [RequireComponent(typeof(Healable))]
    public class Consumer : MonoBehaviour
    {
        private Healable _healable;
        private EnemyIdentifier _eid;
        private GoreZone _gz;
        private ActivateNextWave _anw;

        [Header("Consume Options")]
        public bool isConsumeInstakill = true;
        public float consumeDamage;
        public float maxConsumeHeal = 5.0f;

        public UltrakillEvent onConsume;

        [Header("Fetching Options")]
        public EnemyIdentifier[] consumables;
        public EnemyTargetFinder targetFinder = new EnemyTargetFinder();
        public bool fetchOnEnable;
        public bool fetchPeriodically;
        public float period = 2f;
        private float _timer;
        [Tooltip("0 is every valid target")]
        public int maxTargetsPerFetch;
        public bool clearInvalidConsumeTargetsOnFetch;

        private void Awake()
        {
            _healable = GetComponent<Healable>();
            _eid = GetComponent<EnemyIdentifier>();
            _gz = GetComponentInParent<GoreZone>();
            _anw = GetComponentInParent<ActivateNextWave>();
            targetFinder.Initialize(_eid, _gz, _anw, transform);
        }

        private void Start()
        {
            _timer = period;                    
            if (fetchOnEnable)
                FetchConsumeTargets();
        }
        private void Update()
        {
            if (!fetchPeriodically) return;

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _timer = period;
                FetchConsumeTargets();
            }
        }
        public void ClearInvalidConsumeTargets()
        {
            if (consumables == null || consumables.Length == 0) return;
            consumables = consumables.Where(t => t != null && !t.dead).ToArray();
        }

        public void FetchConsumeTargets()
        {
            consumables = targetFinder.GetValidTargets(maxTargetsPerFetch);

            if (clearInvalidConsumeTargetsOnFetch)
                ClearInvalidConsumeTargets();
        }
        public void ConsumeInstant(EnemyIdentifier target)
        {
            if (!targetFinder.IsValidTarget(target)) return;

            float healthGained = isConsumeInstakill ? target.health : consumeDamage;

            if (isConsumeInstakill)
                target.Death();
            else
                target.DeliverDamage(target.gameObject, Vector3.zero, target.transform.position, consumeDamage, false);

            if (maxConsumeHeal > 0)
                healthGained = Mathf.Min(healthGained, maxConsumeHeal);

            _healable.Heal(healthGained);
            onConsume?.Invoke();

            consumables = consumables.Where(t => t != target).ToArray();
        }

    }
}
