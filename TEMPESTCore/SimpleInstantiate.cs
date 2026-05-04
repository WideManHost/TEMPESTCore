using System.Collections.Generic;
using UnityEngine;

namespace TEMPESTCore
{
    public enum InstantiateObjType
    {
        generic = 0,
        projectile = 1,
        virtueInsignia = 2,
        hitscanBeam = 3,
        follow = 4,
        groundWave = 5,
        swingCheck2 = 6,
        physWave = 79,
    }

    [System.Serializable]
    public class SimpleInstantiate : SimpleClassWithRequirements
    {

        private Enemy _en;
        private GoreZone _gz;

        [Header("Settings")]
        public GameObject toInstantiate;
        public InstantiateObjType objectType;
        public InstantiateObjectMode mode;

        [Space]
        public InstantiateLocationResolve locationSettings;
        public InstantiateRotationResolve rotationSettings;
        public InstantiateParentResolve parentSettings;

        [Space]
        public bool scaleWithSpeed = true;
        public bool scaleWithDamage = true;
        public bool setSafeEnemyType = true;
        public bool setTarget = true;
        public bool removePreviousOnInstantiate;


        private GameObject _lastSpawnedObject;
        //realized that i wasnt telling the rotation resolver what to use as a reference so im gonna use this
        [HideInInspector]public Transform _currentShootPoint;
        [HideInInspector]public bool _isUsingList;
        public Transform thisTransform => _eid == null ? _se.transform : _eid.transform;
        public EnemyType safeEnemyType => _eid.enemyType;

        public EnemyTarget target => _eid.target;
        public Transform targetTransform => _eid.target.targetTransform;

        float speedModifier => _eid.totalSpeedModifier;
        float dmgModifier => _eid.totalDamageModifier;
        public void Instantiate(int id)
        {
            if (!this._se.gameObject.scene.isLoaded) return;
            if (id != this.id) return;
            if (!Validate()) return;
            if (removePreviousOnInstantiate && _lastSpawnedObject != null)
            {
                UnityEngine.Object.Destroy(_lastSpawnedObject);
                _lastSpawnedObject = null;
            }
            Vector3 spawnPos = locationSettings.ResolveLocation(thisTransform);
            Vector3 spawnRot = rotationSettings.ResolveRotation(spawnPos, toInstantiate.transform.rotation);
            Transform parent = parentSettings.ResolveParent();
                                   
            GameObject obj = UnityEngine.Object.Instantiate(toInstantiate, spawnPos, Quaternion.Euler(spawnRot), parent);
            _lastSpawnedObject = obj;
            ConfigureSpawnedObject(obj);
            if (_se.instantiatedObjects == null)
            {
                _se.instantiatedObjects = new List<GameObject>();
            }
            _se.instantiatedObjects.Add(obj);
        }

        private void ConfigureSpawnedObject(GameObject obj)
        {
            //whoops almost forgot, honestly switches are kinda goated
            switch (mode)
            {
                case InstantiateObjectMode.Normal:
                    break;
                case InstantiateObjectMode.ForceEnable:
                    obj.SetActive(true);
                    break;
                case InstantiateObjectMode.ForceDisable:
                    obj.SetActive(false);
                    break;
            }
            switch (objectType)
            {
                case InstantiateObjType.virtueInsignia:
                    if (obj.TryGetComponent(out VirtueInsignia insignia))
                    {
                        //setting target anyway coz otherwise virtue insignias do not work bro
                        insignia.target = target;
                        insignia.parentEnemy = this._en;
                        insignia.hadParent = true;
                        if (scaleWithDamage) insignia.damage = Mathf.RoundToInt(insignia.damage * dmgModifier);
                        if(scaleWithSpeed) insignia.windUpSpeedMultiplier *= speedModifier;
                    }
                    break;

                case InstantiateObjType.projectile:
                    if (setTarget || scaleWithDamage || scaleWithSpeed)
                    {
                        if (obj.TryGetComponent(out Projectile proj))
                        {
                            //btw this is how the game handles it DO NOT PANIC this shit will still work
                            // took this logic from minos prime like if that works this is probably gonna work lols
                            if (setTarget) proj.target = this.target.isPlayer ? new EnemyTarget(MonoSingleton<CameraController>.Instance.transform) : this.target;
                            if (scaleWithDamage) proj.damage *= dmgModifier;
                            if (scaleWithSpeed) proj.speed *= speedModifier;
                        }
                    }
                    break;

                case InstantiateObjType.groundWave:
                    if (obj.TryGetComponent(out GroundWave gw))
                    {
                        //set target anyway this cannot really function without it
                        gw.target = target;
                        gw.eid = this._eid; 
                    }
                    break;

                case InstantiateObjType.follow:
                    if (setTarget || scaleWithSpeed)
                    {
                        if (obj.TryGetComponent(out Follow follow))
                        {
                            if (setTarget) follow.SetTarget(targetTransform);
                            if (scaleWithSpeed) follow.speed *= speedModifier;
                        }
                    }
                    break;

                case InstantiateObjType.physWave:
                    if (scaleWithSpeed || scaleWithDamage)
                    {
                        if (obj.TryGetComponent(out PhysicalShockwave psw))
                        {
                            if (scaleWithSpeed) psw.speed *= speedModifier;
                            if (scaleWithDamage) psw.damage = Mathf.RoundToInt(psw.damage * dmgModifier);
                        }
                    }
                    break;

                case InstantiateObjType.swingCheck2:
                    if (scaleWithSpeed || scaleWithDamage)
                    {
                        if (obj.TryGetComponent(out SwingCheck2 sc))
                        {
                            sc.type = safeEnemyType;
                            if (scaleWithDamage) sc.damage = Mathf.RoundToInt(sc.damage * dmgModifier);
                        }
                    }
                    break;

                case InstantiateObjType.hitscanBeam:
                    if (scaleWithDamage || scaleWithSpeed)
                    {
                        if (obj.TryGetComponent(out RevolverBeam rb))
                        {
                            if (setSafeEnemyType) rb.ignoreEnemyType = safeEnemyType;
                            if (scaleWithDamage) rb.damage *= dmgModifier;
                        }
                    }
                    break;
            }
            HandleHurtzoneLogic(obj);
            HandleBeamLogic(obj);
        }

        private void HandleHurtzoneLogic(GameObject obj)
        {
            if (scaleWithDamage || setSafeEnemyType)
            {
                HurtZone[] allHurtZones = obj.GetComponentsInChildren<HurtZone>();
                if (allHurtZones.Length == 0) return;
                foreach (HurtZone hz in allHurtZones)
                {
                    if (hz == null) continue;
                    //btw some of these use ints in their damage definition instead of floats
                    //because ?????
                    //what was hakita cooking sob
                    if (scaleWithDamage)
                    {
                        hz.setDamage = Mathf.RoundToInt(hz.setDamage * dmgModifier);
                        hz.enemyDamageOverride = hz.enemyDamageOverride == 0f ? hz.enemyDamageOverride : Mathf.RoundToInt(hz.enemyDamageOverride * dmgModifier);
                    }
                    if (setSafeEnemyType)
                    {
                        if (!hz.ignoredEnemyTypes.Contains(safeEnemyType))
                        {
                            hz.ignoredEnemyTypes.Add(safeEnemyType);
                        }
                    }
                }
            }
        }
        private void HandleBeamLogic(GameObject obj)
        {
            //same as above coz honestly i forgot the case that projectiles have beams n stuff
            if (setSafeEnemyType || scaleWithSpeed || scaleWithDamage)
            {
                ContinuousBeam[] allBeams = obj.GetComponentsInChildren<ContinuousBeam>();
                if (allBeams.Length == 0) return;
                foreach (ContinuousBeam cb in allBeams)
                {
                    if (cb == null) continue;
                    if (setSafeEnemyType) cb.safeEnemyType = this.safeEnemyType;
                    if (scaleWithSpeed) cb.startUpSpeed *= speedModifier;
                    if (scaleWithDamage) cb.damage *= dmgModifier;
                }
            }

        }
        public void InitializeInst(EnemyIdentifier eid, GoreZone gz, IEnrage enemy = null, SimpleEvents se = null)
        {
            this._eid = eid;
            this._gz = gz;
            this._en = se.GetComponentInParent<Enemy>();
            locationSettings.Initialize(eid, this);
            rotationSettings.Initialize(eid, this);
            parentSettings.Initialize(_gz, eid, se);
            base.Initialize(eid, se, enemy);
        }
        public override bool Validate()
        {
            if (_se == null) return false;
            if (toInstantiate == null) return false;
            return base.Validate();
        }
    }
}
