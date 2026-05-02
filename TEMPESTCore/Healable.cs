using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace TEMPESTCore
{


    public class Healable : MonoBehaviour
    {

        private EnemyIdentifier _eid;
        private GoreZone _gz;
        private ActivateNextWave _anw;

        [Header("Healing")]
        public bool overrideMaxHealth;
        public float maxHealth;
        public GameObject healParticle;
        public UltrakillEvent onFirstHeal;
        public int timesHealed;
        public EnemyTargetFinder healTargetFinder = new EnemyTargetFinder();
        public EnemyIdentifier[] healables;

        [Header("Fetching Settings")]
        public bool fetchOnEnable = false;
        public bool fetchPeriodically = false;
        public float period = 3f;
        [Tooltip("0 = unlimited")]
        public int maxHealTargets = 0;
        public bool clearInvalidHealTargetsOnFetch = true;
        private float _timer;

        private void Awake()
        {
            _eid = GetComponent<EnemyIdentifier>();
            _gz = GetComponentInParent<GoreZone>();
            _anw = GetComponentInParent<ActivateNextWave>();

            if (_eid == null)
            {
                Debug.LogError($"No EnemyIdentifier on {gameObject.name}", this);
                enabled = false;
                return;
            }

        }
        private void Start()
        {
            if (!overrideMaxHealth || maxHealth <= 0)
                maxHealth = _eid.health;

            if (fetchOnEnable)
                FetchHealTargets();

            healTargetFinder.Initialize(_eid, _gz, _anw, transform);
        }

        private void Update()
        {
            if (!fetchPeriodically) return;

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _timer = period;
                FetchHealTargets();
            }
        }
        public void FetchHealTargets()
        {
            healables = healTargetFinder.GetValidTargets(maxHealTargets);

            if (clearInvalidHealTargetsOnFetch)
                ClearInvalidHealTargets();
        }


        public void ClearInvalidHealTargets()
        {
            if (healables == null || healables.Length == 0) return;

            healables = healables.Where(target => target != null && !target.dead).ToArray();
        }

        public void Heal(float amount)
        {
            if (_eid == null || _eid.dead || amount <= 0) return;

            float newHealth = _eid.health + amount;

            if (overrideMaxHealth)
                newHealth = Mathf.Min(newHealth, maxHealth);

            _eid.health = newHealth;

            timesHealed++;
            if (timesHealed == 1) onFirstHeal?.Invoke();

            SpawnHealParticle();
        }
        private void SpawnHealParticle()
        {
            if (healParticle == null) return;

            Vector3 center = _eid.GetCenter().position;
            Instantiate(healParticle, center, Quaternion.identity);
        }


    }
}
