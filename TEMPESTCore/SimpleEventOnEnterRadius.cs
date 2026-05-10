using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TEMPESTCore;

namespace TEMPESTCore
{
    [Serializable]
    public class SimpleEventOnEnterRadius : SimpleClassWithRequirements
    {
        public enum DetectionMode { UseCollider, UseRadiusCheck }

        [Header("Detection Mode")]
        public DetectionMode detectionMode = DetectionMode.UseRadiusCheck;

        [Header("Collider Mode")]
        [Tooltip("Colliders that will be used as triggers")]
        public List<Collider> colliders = new List<Collider>();
        [Tooltip("Makes the colliders trigger on awake")]
        public bool forceTrigger = true;

        [Header("Radius Mode")]
        public float radius = 15f;


        [Header("Events")]
        public UnityEvent OnEnter;
        public UnityEvent OnStay;
        public UnityEvent OnExit;
        [Tooltip("When Activates on Entering and Reverses on Exit")]
        public UltrakillEvent toActivate;

        [Header("Filtering (Optional)")]
        public LayerMask targetLayerMask = ~0;
        public string targetTag = "Player";

        private bool _wasInside;
        private Transform _target => _eid?.target?.trackedTransform;

        public override void Initialize(EnemyIdentifier eid, SimpleEvents se, IEnrage enemy = null)
        {
            base.Initialize(eid, se, enemy);

            if (detectionMode == DetectionMode.UseCollider)
            {
                SetupColliders();
            }
        }

        private void SetupColliders()
        {
            foreach (var col in colliders)
            {
                if (col == null) continue;

                if (forceTrigger)
                    col.isTrigger = true;

                if (!col.TryGetComponent<TriggerProxy>(out var proxy))
                {
                    proxy = col.gameObject.AddComponent<TriggerProxy>();
                }

                proxy.onEnter = null;
                proxy.onStay = null;
                proxy.onExit = null;

                proxy.onEnter = (other) => { if (ValidateTarget(other)) OnTargetEnter(); };
                proxy.onStay = (other) => { if (ValidateTarget(other)) OnTargetStay(); };
                proxy.onExit = (other) => { if (ValidateTarget(other)) OnTargetExit(); };
            }
        }

        public void Tick()
        {

            if (!activated)
            {
                _wasInside = false;
                return;
            }
            if (!Validate())
            {
                if (_wasInside) OnTargetExit(); 
                _wasInside = false;
                return;
            }
            if (detectionMode == DetectionMode.UseRadiusCheck)
            {
                bool isCurrentlyInside = IsInRadius();

                if (isCurrentlyInside && !_wasInside)
                    OnTargetEnter();
                else if (!isCurrentlyInside && _wasInside)
                    OnTargetExit();

                if (isCurrentlyInside)
                    OnTargetStay();

                _wasInside = isCurrentlyInside;
            }
        }

        private bool IsInRadius()
        {
            if (_target == null) return false;

            float distance = Vector3.Distance(requirements._radiusCenter.position, _target.position);

            bool inside = distance <= requirements.radius;
            return requirements.outsideOfRadius ? !inside : inside;
        }

        private bool ValidateTarget(Collider other)
        {
            if (other == null) return false;
            if (_target == null) return false;

            if (other.transform == _target)
                return true;
            if (other.transform.IsChildOf(_target))
                return true;

            if (((1 << other.gameObject.layer) & targetLayerMask) == 0)
                return false;

            if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag))
                return false;

            return true;
        }

        private void OnTargetEnter()
        {
            OnEnter?.Invoke();

            if (toActivate != null)
            {
                toActivate.Invoke();
            }
        }

        private void OnTargetStay()
        {
            OnStay?.Invoke();
        }

        private void OnTargetExit()
        {
            OnExit?.Invoke();

            if (toActivate != null)
            {
                toActivate.Revert();
            }
        }

        public void ForceColliderSetup() => SetupColliders();
    }
}