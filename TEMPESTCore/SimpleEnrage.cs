using System.Collections;
using UnityEngine;
namespace TEMPESTCore
{
    public class SimpleEnrage : MonoBehaviour, IEnrage
    {
        [SerializeField] private EnemyIdentifier _eid;
        [SerializeField] private Enemy _en;
        public bool readyToEnrage = true;
        public bool enraged;
        public bool isEnraged => this.enraged;
        public bool notIfPuppet;
        [Tooltip("Unenrages when timer runs out; in seconds")]
        public bool useTimer;
        public float timer;
        private float currentTimer;
        public bool dropPitchOnExit;
        [HideInInspector] public AudioSource currentRageEffectAudio;

        [Header("Difficulty Requirement")]
        public bool requiresDifficulty;
        public DifficultyRequirement difficultyRequirement;
        [Header("Visuals")]
        public GameObject enrageEffect;
        [HideInInspector] public GameObject currentEnrageEffect;
        public Transform enrageEffectParent;
        [Tooltip("Set To one if all are set to zero")] public Vector3 rageEffectScale = Vector3.one;

        public bool enemySimplifierOverrideMaterials = true;
        public EnrageRenderer[] renderersToEnrage;

        public EnrageLight[] lightsToEnrage;

        private EnemySimplifier[] ensims;

        public ParticleSystem[] particleSystemsEnraged;
        public ParticleSystem[] particleSystemsUnEnraged;
        [Header("Events")]
        [Tooltip("This is activated when enraged and reversed when UnEnraged")] public UltrakillEvent onEnrage;

        private void Awake()
        {
            this._eid = this.GetComponent<EnemyIdentifier>();
            this._en = this.GetComponent<Enemy>();
            this.ensims = this.GetComponentsInChildren<EnemySimplifier>();
        }

        private void Start()
        {
            if (renderersToEnrage != null)
            {
                foreach (EnrageRenderer enrageData in renderersToEnrage)
                {
                    enrageData.Initialize();
                }
            }
            StartCoroutine(MaterialWatcherTick());
        }
        private void Update()
        {
            if (_eid.dead) return;
            if (useTimer && currentTimer > 0) HandleTimer();
        }
        void HandleTimer()
        {
            currentTimer -= Time.deltaTime;

            if (dropPitchOnExit && currentRageEffectAudio != null)
            {
                if (currentTimer < 3.0f && currentTimer > 0f)
                {
                    currentRageEffectAudio.pitch = currentTimer / 3f;
                }
            }
            if (currentTimer <= 0f)
            {
                currentTimer = 0f;

                if (currentRageEffectAudio != null) currentRageEffectAudio.pitch = 1f;

                ToggleEnrage(false);
            }
        }
        public void ToggleEnrage(bool status)
        {
            if (!Validate()) return;
            if (enraged == status) return;
            enraged = status;
            ToggleParticleSystems(status);
            HandleRageEffectInstantiate(status);
            if (status)
            {
                if (currentRageEffectAudio != null) currentRageEffectAudio.pitch = 1f;
                if (useTimer) currentTimer = timer;
                onEnrage?.Invoke();
            }
            else
            {
                onEnrage.Revert();
            }


            foreach (EnemySimplifier ensim in ensims) if (ensim.enraged != status) ensim.enraged = status;
            if (!enemySimplifierOverrideMaterials) foreach (var r in renderersToEnrage) r.SetEnrageState(status);
            foreach (var l in lightsToEnrage) l.SetEnrageState(status);
        }
        private bool Validate()
        {
            if (_eid == null || _eid.dead) return false;
            if (!readyToEnrage && !enraged) return false;
            if (requiresDifficulty && !CheckDifficultyFlags()) return false;
            if (_eid.puppet && notIfPuppet) return false;
            return true;
        }
        public bool CheckDifficultyFlags()
        {
            if (!requiresDifficulty)
                return true;

            // none = no requirements
            if (difficultyRequirement == DifficultyRequirement.None) return true;

            int currentDifficulty = GetCurrentDifficulty();

            int currentMask = 1 << currentDifficulty;

            return ((int)difficultyRequirement & currentMask) != 0;
        }
        private int GetCurrentDifficulty()
        {
            var prefs = MonoSingleton<PrefsManager>.Instance;
            if (prefs == null)
                return (int)GameDifficulty.Standard;
            int diff = prefs.GetInt("difficulty");

            if (diff < 0) return (int)GameDifficulty.Harmless;
            if (diff > 5) return (int)GameDifficulty.UKMD;

            return diff;
        }
        public void Enrage() { ToggleEnrage(true); }
        public void UnEnrage() { ToggleEnrage(false); }
        void HandleRageEffectInstantiate(bool state)
        {
            if (state)
            {
                if (enrageEffect == null) return;
                //create
                if (currentEnrageEffect == null) currentEnrageEffect = Instantiate<GameObject>(this.enrageEffect, this.transform.position, this.transform.rotation);
                //set parent if asigned
                currentEnrageEffect.transform.SetParent(enrageEffectParent != null ? enrageEffectParent : transform, true);
                //set local scale, if rageEffectScale isnt zero
                if (rageEffectScale == Vector3.zero) currentEnrageEffect.transform.localScale = Vector3.one;
                else currentEnrageEffect.transform.localScale = rageEffectScale;
                //get AudioSource
                currentRageEffectAudio = currentEnrageEffect.GetComponent<AudioSource>();
            }
            else
            {
                if (currentEnrageEffect != null)
                {
                    Destroy(currentEnrageEffect);
                }
            }
        }
        void ToggleParticleSystems(bool status)
        {
            if (status)
            {
                foreach (var pr in particleSystemsEnraged) pr.Play();
                foreach (var pn in particleSystemsUnEnraged) pn.Stop();
            }
            else
            {
                foreach (var pr in particleSystemsEnraged) pr.Stop();
                foreach (var pn in particleSystemsUnEnraged) pn.Play();
            }
        }


        private IEnumerator MaterialWatcherTick()
        {
            while (!_eid.dead && this != null)
            {
                yield return new WaitForSeconds(0.5f);
                if (this == null) yield break;
                if (_eid.dead) yield break;
                if (!enraged && renderersToEnrage != null)
                {
                    foreach (EnrageRenderer enrageData in renderersToEnrage)
                    {
                        enrageData.UpdateMaterials();
                    }
                }
            }
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (renderersToEnrage != null)
            {
                foreach (var r in renderersToEnrage)
                {
                    r.OnValidate();
                }
            }
        }
#endif
    }
}

