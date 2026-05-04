using System;
using HarmonyLib;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace TEMPESTCore
{
    [Serializable]
    public class GhostedRendererCache
    {
        /// <summary>
        /// simple data holder class that holds a renderer and the original materials, used for reversing the effects.
        /// </summary>
        public Renderer renderer;
        public Material[] originalMaterials;

        public void Cache(Renderer ren)
        {
            if (ren == null) return;
            renderer = ren;
            originalMaterials = ren.sharedMaterials;
        }
    }

    public enum GhostedParentDeathMode
    {
        Nothing,
        DestroyOnParentDeath,
        UnghostOnParentDeath,
        StartTimer
    }

    public class Ghosted : MonoBehaviour
    {
        /// <summary>
        /// THIS IS TOO MUCH IM DEPRECATING THIS
        /// i want to die
        /// disables collisions, swaps material, changes a few flags onEID
        /// 
        /// </summary>
        private Enemy _en;
        private EnemyIdentifier _eid;
        private GoreZone _gz;
        private ActivateNextWave _anw;

        private List<Collider> _cols = new List<Collider>();
        private List<int> _originalColliderLayers = new List<int>();
        private GhostedRendererCache[] _cache;
        private List<GameObject> _activeParticles = new List<GameObject>();

        private GameObject _deathCopyInstance;
        private GameObject _blueprintGhost;
        private Collider _col;
        private GhostManager _manager;
        private List<Vector3> _cachedColSizes = new List<Vector3>();

        [HideInInspector]public bool isAborting;


        [Header("Settings")]
        public bool ready;
        public bool onStart = false;
        public bool ghosted;
        public bool ignoreVisuals;
        public bool dontOverrideEnemySettings;
        public bool dontOverrideCountAsKill;
        //hidden because logic explicitly overrides this and this needs to be changeable - h
        [HideInInspector]public GhostType type;

        [Header("Effects")]
        public Material ghostedMaterial;
        public GameObject ghostedEffectPrefab;
        public GameObject ghostedParticlePrefab;
        public GameObject ghostedStartEffect;
        public GameObject ghostedExitEffect;

        [Header("Parent Ghost Logic")]
        public EnemyIdentifier parent;
        public GhostedParentDeathMode parentDeathMode = GhostedParentDeathMode.StartTimer;

        [Header("Ghost")]
        public float ghostSpawnDelay = 2f;
        public bool isCopy;
        public bool invincible; 
        public bool dontDieWithParent;
        [HideInInspector]public GameObject original;

        [Header("LifeTime")]
        public bool timerOnSpawn;
        public bool infinite;
        public bool timerActive = false;
        public float ghostLifetime = 5f;
        private float _lifetimeTimer;

        [Header("Additional")]
        public Renderer[] addedRenderers;

        // EID and EN cache settings
        private bool _origImmuneFF;
        private bool _origHookIgnore;
        private bool _origIgnoredByEn;
        private bool _origDontCountAsKills;
        private bool _origDontDie;

        public UnityEvent onGhosted = new UnityEvent();
        public UnityEvent onUnghosted = new UnityEvent();
        public UltrakillEvent onParentDeath;

        private void Awake()
        {
            InitializeManager();
            _en = GetComponent<Enemy>();
            _eid = GetComponent<EnemyIdentifier>();
            _col = GetComponent<Collider>();
            _anw = GetComponentInParent<ActivateNextWave>();
            _gz = GetComponentInParent<GoreZone>();

            CacheColliders();
            CacheRenderers();
            CacheOriginalStates();
            if (!isCopy || type != GhostType.Ghostable)
            {
                type = GhostType.Ghostable;
                CreateBlueprint();
                if (_eid != null)
                    _eid.onDeath.AddListener(OnDeath);
            }
            else 
            {  
                if (type == GhostType.Instance) _manager.RegisterGhost(this.parent, this);
            }
        }
        private void OnDeath()
        {
            if (this.isCopy || _manager == null) return;

            if (_deathCopyInstance != null && type == GhostType.Ghostable)
            {
                _manager.RequestSpawn(ghostSpawnDelay, _deathCopyInstance, transform.position, transform.rotation, _eid);
            }
        }
        private void CreateBlueprint()
        {
            // Create the template
            _blueprintGhost = Instantiate(gameObject, transform.position, transform.rotation);
            // set copy immediately as the create blueprint command runs on awake
            var blueprintGhost = _blueprintGhost.GetComponent<Ghosted>();
            if (blueprintGhost != null) { blueprintGhost.isCopy = true; blueprintGhost.type = GhostType.Blueprint; blueprintGhost.parent = this.parent; }
            // the rest
            _blueprintGhost.name = $"{gameObject.name}_Blueprint";
            _blueprintGhost.SetActive(false);
            blueprintGhost.original = this.original == null ? this.gameObject : this.original;
            _manager.RegisterGhost(blueprintGhost.parent, blueprintGhost);
            // Parent to Gorezone so it can be destroyed on CheckpointRestart or remain disabled alongside the room its in.
            if (_manager != null) _blueprintGhost.transform.SetParent(_gz.transform);
        }
        private void Start()
        {
            if (_manager == null) InitializeManager();

            if (parent != null && _manager != null)
                _manager.RegisterGhost(parent, this);
            if (!isCopy)
            {
                CreateDeathCopy();
                if (onStart) Toggle(true);
            }            
        }
        private void InitializeManager()
        {
            // Look for the persistent parent
            Transform p = _anw != null ? _anw.transform : (_gz != null ? _gz.transform : null);

            if (p != null)
                _manager = p.GetComponent<GhostManager>() ?? p.gameObject.AddComponent<GhostManager>();
            else
            {
                var globalMove = GameObject.Find("GhostManager_Global");
                if (globalMove == null) globalMove = new GameObject("GhostManager_Global");
                _manager = globalMove.GetComponent<GhostManager>() ?? globalMove.AddComponent<GhostManager>();
            }
        }
        private void Update()
        {
            if (ghosted && timerActive && !infinite && !invincible)
            {
                _lifetimeTimer -= Time.deltaTime;
                if (_lifetimeTimer <= 0f)
                    Destroy(gameObject);
            }
        }
        private void CacheColliders()
        {
            var all = GetComponentsInChildren<Collider>(true);
            foreach (var col in all)
            {
                if (col.CompareTag("Limb") || col.CompareTag("Head") ||
                    col.CompareTag("Armor") || col.CompareTag("Body"))
                {
                    _cols.Add(col);
                    _originalColliderLayers.Add(col.gameObject.layer);
                    _cachedColSizes.Add(col.bounds.size);
                }
            }
        }

        private void CacheRenderers()
        {
            var temp = new List<GhostedRendererCache>();

            foreach (var sim in GetComponentsInChildren<EnemySimplifier>(true))
            {
                var r = sim.GetComponent<Renderer>();
                if (r != null)
                {
                    var cache = new GhostedRendererCache();
                    cache.Cache(r);
                    temp.Add(cache);
                }
            }

            _cache = temp.ToArray();
        }

        private void CacheOriginalStates()
        {
            if (_eid != null)
            {
                _origImmuneFF = _eid.immuneToFriendlyFire;
                _origIgnoredByEn = _eid.ignoredByEnemies;
                _origHookIgnore = _eid.hookIgnore;
                _origDontCountAsKills = _eid.dontCountAsKills;
            }

            if (_en != null)
                _origDontDie = _en.dontDie;
        }

        private void CreateDeathCopy()
        {
            if (isCopy) return;

            _deathCopyInstance = Instantiate(gameObject, transform.position, transform.rotation);
            _deathCopyInstance.SetActive(false);

            Transform targetParent = null;

            if (_anw != null)
                targetParent = _anw.transform;
            else if (_gz != null)
                targetParent = _gz.transform;
            else if (parent != null)
                targetParent = parent.transform;

            _deathCopyInstance.transform.SetParent(targetParent);

            var copyGhost = _deathCopyInstance.GetComponent<Ghosted>();
            if (copyGhost != null)
            {
                copyGhost.isCopy = true;
                copyGhost._activeParticles = new List<GameObject>();
            }
        }
        //i can never find this the first try god fucking help me for not sorting these in alphabetical order ;-;
        public void Toggle(bool value)
        {
            // 1, check if ghost is valid 
            if (value && isCopy && !dontDieWithParent && !invincible  && (parent == null || parent.dead))
            {
                isAborting = true;
                Destroy(gameObject);
                return;
            }

            ghosted = value;

            if (value)
            {

                if (isCopy) CacheRenderers();

                if (ghostedStartEffect != null && !isAborting)
                    Instantiate(ghostedStartEffect, transform.position, Quaternion.identity);

                if (!infinite && !invincible && (timerOnSpawn || timerActive))
                {
                    StartGhostTimer();
                }
            }
            ApplyColliderState(value);
            if (!dontOverrideEnemySettings)
            {
                ApplyEnemyIdentifierState(value);
                ApplyEnemyState(value);
            }
            ApplyVisuals(value);

            if (value) onGhosted?.Invoke();
            else onUnghosted?.Invoke();
        }

        public void StartGhostTimer()
        {
            if (invincible) return;
            timerActive = true;
            _lifetimeTimer = ghostLifetime;
        }
        /// <summary>
        /// if false, enables all the colliders and sets them to their original layer, if true, sets them all to EnemyWall and turns them off
        /// fun fact this is the method the actual medium ghosts used isnt that fun?
        /// </summary>
        /// <param name="ghosted"></param>
        private void ApplyColliderState(bool ghosted)
        {
            int targetLayer = ghosted ? LayerMask.NameToLayer("EnemyWall") : -1;

            for (int i = 0; i < _cols.Count; i++)
            {
                var col = _cols[i];
                if (col == null) continue;

                col.enabled = !ghosted;

                if (col.gameObject != null)
                {
                    col.gameObject.layer = ghosted ? targetLayer : _originalColliderLayers[i];
                }
            }
            if (_col != null)
            {
                int thisLayer = ghosted ? LayerMask.NameToLayer("EnemyWall") : LayerMask.NameToLayer("EnemyTrigger");

                _col.gameObject.layer = thisLayer;
            }


        }

        private void ApplyEnemyIdentifierState(bool ghosted)
        {
            if (_eid == null) return;

            if (ghosted)
            {
                _eid.ignoredByEnemies = true;
                _eid.immuneToFriendlyFire = true;
                if(!dontOverrideCountAsKill) _eid.dontCountAsKills = true;
                _eid.dontUnlockBestiary = true;
                _eid.hookIgnore = true;
            }
            else
            {
                _eid.ignoredByEnemies = _origIgnoredByEn;
                _eid.immuneToFriendlyFire = _origImmuneFF;
                _eid.hookIgnore = _origHookIgnore;
                if (!dontOverrideCountAsKill) _eid.dontCountAsKills = _origDontCountAsKills;
            }
        }

        private void ApplyEnemyState(bool ghosted)
        {
            if (_en != null)
                _en.dontDie = ghosted ? true : _origDontDie;
        }

        private void ApplyVisuals(bool ghosted)
        {
            if (ignoreVisuals) return;
            //materials segment here
            /// <summary>
            /// if true, replaces every material with ghost material, if false, replaces every material with their cache.
            /// </summary>
            /// <param name="ghosted"></param>
            if (ghostedMaterial == null) return;

            foreach (var item in _cache)
            {
                if (item.renderer == null) continue;

                if (ghosted)
                {
                    //materials with the ghost material
                    Material[] ghostMats = new Material[item.originalMaterials.Length];
                    for (int i = 0; i < ghostMats.Length; i++)
                    {
                        ghostMats[i] = ghostedMaterial;       
                    }
                    item.renderer.sharedMaterials = ghostMats;
                }
                else
                {
                    item.renderer.sharedMaterials = item.originalMaterials;
                }
            }

            if (ghosted && ghostedParticlePrefab != null)
            {
                ClearParticles();
                for (int i = 0; i < _cols.Count; i++)
                {
                    if (_cols[i] == null) continue;

                    GameObject p = Instantiate(ghostedParticlePrefab, _cols[i].transform.position, _cols[i].transform.rotation);

                    p.transform.localScale = _cols[i].bounds.size;

                    p.transform.SetParent(_cols[i].transform, true);

                    p.transform.localPosition = Vector3.zero;

                    _activeParticles.Add(p);
                }
            }
            else if (!ghosted)
            {
                ClearParticles();
            }
        }

        private void ClearParticles()
        {
            if (_activeParticles == null) return;

            for (int i = _activeParticles.Count - 1; i >= 0; i--)
            {
                GameObject p = _activeParticles[i];
                if (p != null)
                {
                    Destroy(p);
                }
            }
            _activeParticles.Clear();
        }

        private void OnDestroy()
        {
            ClearParticles();
            if (isAborting) return;
            if (parent == null) return;
            if (ghostedExitEffect != null)
            {
                Instantiate(ghostedExitEffect, transform.position, Quaternion.identity);
            }
        }
    }
}
