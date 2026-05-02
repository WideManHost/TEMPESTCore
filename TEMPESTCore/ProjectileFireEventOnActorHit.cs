using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace TEMPESTCore
{
    /// <summary>
    /// Fires an event when the projectile hits an enemy - wade
    /// </summary>
    public class ProjectileFireEventOnActorHit : MonoBehaviour
    {
        [Tooltip("Determines if its actively ")]
        public bool isActive;
        private Collider _col;
        public EnemyIdentifier targetEnemy;
        [SerializeField] private LayerMask enemyLayerMask;
        public UnityEvent onSuccessfulHit;

        [SerializeField] private Projectile _projectile;
        public bool activateOnParry;

        void Awake()
        {
            _col = GetComponent<Collider>();
            if (_projectile == null) _projectile = GetComponent<Projectile>();
        }

        void Update()
        {
            if (_projectile != null && _projectile.parried && !isActive && activateOnParry)
            {
                isActive = true;
            }
        }

        void OnTriggerEnter(Collider hitCollider)
        {
            if (!isActive) return;

            if (((1 << hitCollider.gameObject.layer) & enemyLayerMask) == 0) return;

            EnemyIdentifier hitEnemy = hitCollider.gameObject.GetComponentInParent<EnemyIdentifier>();

            if (hitEnemy != null && (targetEnemy == null || hitEnemy == targetEnemy))
            {
                onSuccessfulHit?.Invoke();
            }
        }

        public void OverrideTarget(EnemyIdentifier newTarget)
        {
            if (newTarget != null) targetEnemy = newTarget;
        }
        public void OnSuccess()
        {
            onSuccessfulHit?.Invoke();
        }
    }
}
