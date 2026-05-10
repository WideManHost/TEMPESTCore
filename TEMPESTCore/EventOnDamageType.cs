using System;
using UnityEngine;

namespace TEMPESTCore
{
    public class EventOnDamageType : MonoBehaviour
    {
        private EnemyIdentifier _eid;
        public string damageType;

        private float _cachedLastHealth;

        public UltrakillEvent onDamageType;
        public int maxUses;
        private int _currentUses;
        private bool _maxReached;
        public UltrakillEvent onMaxUses;

        private void Awake()
        {
            if (_eid == null) _eid = GetComponent<EnemyIdentifier>();

            if (maxUses < 0) maxUses = Mathf.Abs(maxUses);

            _currentUses = 0;
            _maxReached = false;
        }

        private void Start()
        {
            if (_eid != null)
            {
                _cachedLastHealth = _eid.health;
            }
        }

        private void Update()
        {
            if (_eid == null || _eid.dead || _maxReached) return;

            float currentHealth = _eid.health;
            string currentHitter = _eid.hitter;

            if (currentHealth != _cachedLastHealth && currentHitter == damageType)
            {
                TriggerEvent();
            }

            _cachedLastHealth = currentHealth;
        }

        private void TriggerEvent()
        {
            onDamageType.Invoke();

            if (maxUses > 0)
            {
                _currentUses++;
                if (_currentUses >= maxUses)
                {
                    _maxReached = true;
                    onMaxUses.Invoke();
                }
            }
        }
    }
}