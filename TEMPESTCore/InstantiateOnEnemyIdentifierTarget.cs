using System.Collections.Generic;
using ULTRAKILL;
using UnityEngine;

namespace TEMPESTCore
{
    /// <summary>
    /// Like InstantiateObject but targets an enemy instead
    /// </summary>
    public class InstantiateOnEnemyIdentifierTarget : MonoBehaviour
    {
        [Tooltip("The EnemyIdentifier that will have an object spawned on.")]
        public EnemyIdentifier targetEnemyIdentifier;

        [SerializeField]
        private bool instantiateOnEnabled = true;
        [SerializeField]
        private bool useTargetRotation = true;
        [SerializeField]
        private bool removePreviousOnInstantiate = true;
        [SerializeField]
        private GameObject source;
        [SerializeField]
        private InstantiateObjectMode mode;
        [SerializeField]
        private bool combineRotations;
        [SerializeField]
        private bool parentToGoreZone;


        private GoreZone gz;
        private List<GameObject> createdObjects = new List<GameObject>();

        public void InstantiateObject()
        {
            if (targetEnemyIdentifier != null && source != null)
            {
                if (removePreviousOnInstantiate)
                {
                    foreach (Object createdObject in createdObjects)
                        Destroy(createdObject);
                    createdObjects.Clear();
                }
                GameObject gameObject = Instantiate(source);
                gameObject.transform.position = targetEnemyIdentifier.transform.position;

                if (useTargetRotation)
                {
                    if (combineRotations)
                        gameObject.transform.rotation *= transform.rotation;
                    else
                        gameObject.transform.rotation = transform.rotation;
                }

                createdObjects.Add(gameObject);
                switch (mode)
                {
                    case InstantiateObjectMode.ForceEnable:
                        gameObject.SetActive(true);
                        break;
                    case InstantiateObjectMode.ForceDisable:
                        gameObject.SetActive(false);
                        break;
                }

              
                if (parentToGoreZone)
                {
                    if (gz == null)
                        gz = GoreZone.ResolveGoreZone(transform);
                    gameObject.transform.SetParent(gz.transform, true);
                }
               
            }
        }
        
        void OnEnable()
        {
            if (instantiateOnEnabled)
            {
                InstantiateObject();
            }
        }

    }
}
