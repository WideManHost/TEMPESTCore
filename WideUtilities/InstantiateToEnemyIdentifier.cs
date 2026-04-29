using System;
using System.Collections.Generic;
using ULTRAKILL;
using UnityEngine;

namespace WideUtilities
{
    public class InstantiateToEnemyIdentifierTarget : ActiveStateChecker
    {
        [Tooltip("Enemy Identifier that has the target we're using.")]
        public EnemyIdentifier enemyIdentifier;
        
        [Tooltip("Object to Instantiate")]
        public GameObject toSpawn;
        [Tooltip("Does the instantiated object parent itself to the EID target in the Hierarchy?")]
        public bool doReparent;
        [Tooltip("Does the instantiated object reposition itself to the EID target position?")]
        public bool doReposition;
        [Tooltip("Transform to spawn the object if the EID's target is null or doesn't reposition it.")]
        public Transform fallbackSpawnTransform;

        [Header("Ultrakill Event")]
        public UltrakillEvent onInstantiate = new UltrakillEvent();


        public override void FireBehavior() => InstantiateOnEnemyIDTarget();
        /// <summary>
        /// Instantiates the toSpawn GameObject onto the EnemyIdentifier's target, either at their position
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void InstantiateOnEnemyIDTarget()
        {
            if (!enemyIdentifier)
                throw new Exception("EnemyIdentifier was null! Cannot Instantiate on eID target");

            if (enemyIdentifier.target == null)
                Debug.LogWarning("EnemyIdentifier's Target was null, didn't spawn anything", this);

            GameObject clone = Instantiate(toSpawn);

            if (doReparent)
                clone.transform.parent = enemyIdentifier.target.trackedTransform;
            if (doReposition)
            {
                clone.transform.position = enemyIdentifier.target.position;
            }
            else
            {
                // spawn here just incase
                clone.transform.position = fallbackSpawnTransform.position;
            }
                
        }

        /// <summary>
        /// Sets the enemyIdentifier field.
        /// </summary>
        /// <param name="selfIdentifier"></param>
        public void SetSelfIdentifier(EnemyIdentifier selfIdentifier)
        {
            enemyIdentifier = selfIdentifier;
        }

      
    }
}
