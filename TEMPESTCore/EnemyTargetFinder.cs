using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TEMPESTCore
{
    /// <summary>
    /// Class for finding a target within a gorezone or a wave. Used by any future scripts that require finding a target. 
    /// </summary>
    [Serializable]
    public class EnemyTargetFinder
    {
        public SearchLocation location = SearchLocation.Gorezone;
        public Transform overrideTransform;

        [Header("Filters")]
        public bool onlyEnabled = true;
        public bool ignoreSelf = true;
        public bool ignoreOwnEnemyType = true;

        [Tooltip("If any entries exist, ONLY these types will be targeted.")]
        public List<EnemyType> whitelist = new List<EnemyType>();

        [Tooltip("These types will never be targeted.")]
        public List<EnemyType> blacklist = new List<EnemyType>();
        public EnemyIdentifier[] enemiesToIgnore;

        [Header("Area Filter")]
        public bool searchInRadius = false;
        public float radius = 15f;
        [Tooltip("If left empty, will use self's transform as center")]
        public Transform radiusCenterOverride;

        private EnemyIdentifier _self;
        private GoreZone _gz;
        private ActivateNextWave _anw;
        private Transform _center;

        public void Initialize(EnemyIdentifier self, GoreZone gz = null, ActivateNextWave anw = null, Transform customCenter = null)
        {
            _self = self;
            _gz = gz;
            _anw = anw;
            _center = customCenter != null ? customCenter :
                                  radiusCenterOverride != null ? radiusCenterOverride :
                                  self != null ? self.transform : null;
        }

        public EnemyIdentifier[] GetValidTargets(int maxCount = 0)
        {
            if (_self == null) return Array.Empty<EnemyIdentifier>();

            List<EnemyIdentifier> candidates = GetCandidates();

            var filtered = candidates.Where(t => IsValidTarget(t));

            if (maxCount > 0)
                return filtered.Take(maxCount).ToArray();
            else
                return filtered.ToArray();
        }
        private List<EnemyIdentifier> GetCandidates()
        {
            List<EnemyIdentifier> candidates = new List<EnemyIdentifier>();

            switch (location)
            {
                case SearchLocation.Override:
                    if (overrideTransform != null)
                        candidates.AddRange(overrideTransform.GetComponentsInChildren<EnemyIdentifier>(true));
                    break;

                case SearchLocation.Wave:
                    if (_anw != null)
                        candidates.AddRange(_anw.GetComponentsInChildren<EnemyIdentifier>(true));
                    break;

                case SearchLocation.Gorezone:
                    if (_gz != null)
                        candidates.AddRange(_gz.GetComponentsInChildren<EnemyIdentifier>(true));
                    break;

                case SearchLocation.Children:
                    if (_self != null)
                        candidates.AddRange(_self.GetComponentsInChildren<EnemyIdentifier>(true));
                    break;

                case SearchLocation.Global:
                    candidates.AddRange(UnityEngine.Object.FindObjectsOfType<EnemyIdentifier>(true));
                    break;
            }

            return candidates;
        }
        public bool IsValidTarget(EnemyIdentifier target)
        {
            if (target == null || target.dead || _self == null) return false;
            if (ignoreSelf && target == _self) return false;
            if (ignoreOwnEnemyType && target.enemyType == _self.enemyType) return false;
            if (onlyEnabled && !target.gameObject.activeInHierarchy)
                return false;

            if (whitelist != null && whitelist.Count > 0)
            {
                if (!whitelist.Contains(target.enemyType))
                    return false;
            }

            if (blacklist != null && blacklist.Count > 0)
            {
                if (blacklist.Contains(target.enemyType))
                    return false;
            }

            if (enemiesToIgnore != null && enemiesToIgnore.Length > 0)
            {
                if (enemiesToIgnore.Contains(target))
                    return false;
            }

            if (searchInRadius && _center != null)
            {
                float distance = Vector3.Distance(_center.position, target.transform.position);
                if (distance > radius)
                    return false;
            }

            return true;
        }
    }
}
