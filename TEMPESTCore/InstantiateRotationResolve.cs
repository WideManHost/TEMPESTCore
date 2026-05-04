using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TEMPESTCore
{
    public enum InstantiateFacingMode
    {
        useOwnRotation = 0,     
        useObjectRotation = 1,  
        noRotation = 2,
        faceTarget = 3,
        useShootpointRotation = 4
    }
    [System.Serializable]
    public class InstantiateRotationResolve
    {
        private EnemyIdentifier _eid;
        private SimpleInstantiate _si;

        public InstantiateFacingMode facing;
        [Tooltip("If true, stacks the enemy's rotation with the prefab's rotation.")]
        public bool combineRotations;
        public void Initialize(EnemyIdentifier eid, SimpleInstantiate si)
        {
            _eid = eid;
            _si = si;
        }

        public Vector3 ResolveRotation(Vector3 spawnPosition, Quaternion prefabRotation)
        {
            Quaternion targetRotation = Quaternion.identity;

            switch (facing)
            {
                case InstantiateFacingMode.useOwnRotation:
                    if (_eid != null)
                        targetRotation = _eid.transform.rotation;
                    break;

                case InstantiateFacingMode.faceTarget:
                    if (_eid != null && _eid.target != null)
                    {
                        Vector3 direction = _eid.target.position - spawnPosition;
                        if (direction != Vector3.zero)
                        {
                            targetRotation = Quaternion.LookRotation(direction);
                        }
                    }
                    break;

                case InstantiateFacingMode.useObjectRotation:
                    targetRotation = prefabRotation;
                    break;

                case InstantiateFacingMode.noRotation:
                    targetRotation = Quaternion.identity;
                    break;
                //added this bit last minute lmao
                case InstantiateFacingMode.useShootpointRotation:
                    targetRotation = _si._currentShootPoint.rotation;
                    break;
            }

            //ripped this straight from the InstantiateObject logic LMAOOOOO
            if (combineRotations)
            {
                targetRotation = targetRotation * prefabRotation;
            }

            return targetRotation.eulerAngles;
        }
    }
}
