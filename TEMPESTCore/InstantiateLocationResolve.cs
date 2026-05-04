using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TEMPESTCore
{
    public enum InstantiateLocationMode
    {
        shootPoint = 0,
        target = 1,
        ownPosition = 3,
        randomInTrigger = 4,
        list = 5
    }
    [System.Serializable]
    public class InstantiateLocationResolve
    {
        private EnemyIdentifier _eid;
        private SimpleInstantiate _si;
        public InstantiateLocationMode location;
        public bool offset;
        public bool local;
        public Vector3 positionOffset;
        public Transform shootpoint;
        [Header("If Random In Collider Bounds")]
        public Collider collider;
        [UnityEngine.Header("If On List")]
        [Space(0)]
        public List<Transform> list;
        [Tooltip("the location will cycle through the transforms in the list")]
        public bool sequentially;
        private int _case; //for calculating which item on the list should be chosen btw dont touch
        public void Initialize(EnemyIdentifier eid, SimpleInstantiate si)
        {
            _eid = eid;
            _si = si;
        }

        public Vector3 ResolveLocation(Transform fallbackTransform)
        {
            Vector3 position = Vector3.zero;

            switch (location)
            {
                case InstantiateLocationMode.shootPoint:
                    position = shootpoint != null ? shootpoint.position : fallbackTransform.position;
                    _si._currentShootPoint = shootpoint != null? shootpoint : fallbackTransform;
                    break;
                case InstantiateLocationMode.target:
                    if (_eid != null && _eid.target != null)
                        position = _eid.target.position;
                    else
                        position = fallbackTransform.position;
                    break;

                case InstantiateLocationMode.ownPosition:
                    position = fallbackTransform.position;
                    break;

                case InstantiateLocationMode.randomInTrigger:
                    position = ResolveInCollider(collider, fallbackTransform.position);
                    break;

                case InstantiateLocationMode.list:
                    if (list == null || list.Count == 0)
                    {
                        position = fallbackTransform.position;

                    }
                    else if (sequentially)
                    {
                        int index = _case % list.Count;
                        _case++;
                        position = list[index] != null ? list[index].position : fallbackTransform.position;
                        _si._currentShootPoint = list[index] != null ? list[index] : fallbackTransform;
                    }
                    else
                    {
                        int randomIndex = Random.Range(0, list.Count);
                        position = list[randomIndex] != null ? list[randomIndex].position : fallbackTransform.position;
                        _si._currentShootPoint = list[randomIndex] != null ? list[randomIndex] : fallbackTransform;
                    }
                    _si._isUsingList = true;
                    break;

                default:
                    position = fallbackTransform.position;
                    break;
            }

            if (offset)
            {
                if (local)
                {
                    Quaternion refRot = (shootpoint != null) ? shootpoint.rotation : fallbackTransform.rotation;
                    position += refRot * positionOffset;
                }
                else
                {
                    position += positionOffset;
                }
            }
            return position;
        }
        private Vector3 ResolveInCollider(Collider col, Vector3 fallback)
        {
            if (col == null || !col.isTrigger)
                return fallback;

            if (col is BoxCollider box)
            {
                Vector3 extents = box.size * 0.5f;
                Vector3 localPoint = new Vector3(
                    UnityEngine.Random.Range(-extents.x, extents.x),
                    UnityEngine.Random.Range(-extents.y, extents.y),
                    UnityEngine.Random.Range(-extents.z, extents.z)
                ) + box.center;

                return box.transform.TransformPoint(localPoint);
            }
            else if (col is SphereCollider sphere)
            {
                Vector3 localPoint = UnityEngine.Random.insideUnitSphere * sphere.radius;
                localPoint += sphere.center;
                return sphere.transform.TransformPoint(localPoint);
            }
            else if (col is CapsuleCollider capsule)
            {
                return GetRandomPointInCapsule(capsule);
            }
            else if (col is MeshCollider meshCollider)
            {
                return GetRandomPointInMeshCollider(meshCollider);
            }
            else
            {
                // Fallback
                Bounds b = col.bounds;
                return new Vector3(
                    Random.Range(b.min.x, b.max.x),
                    Random.Range(b.min.y, b.max.y),
                    Random.Range(b.min.z, b.max.z)
                );
            }

        }
        private Vector3 GetRandomPointInCapsule(CapsuleCollider capsule)
        {
            float radius = capsule.radius;
            float halfHeight = Mathf.Max(0f, (capsule.height - 2f * radius) * 0.5f);

            Vector3 localPoint;

            float t = Random.value;
            // Bottom cap
            if (t < 0.33f) 
            {
                localPoint = Random.insideUnitSphere * radius;
                localPoint.y -= halfHeight;
            }
            // Cylinder
            else if (t < 0.66f) 
            {
                float y = Random.Range(-halfHeight, halfHeight);
                Vector2 circle = Random.insideUnitCircle * radius;
                localPoint = new Vector3(circle.x, y, circle.y);
            }
            // Top cap
            else
            {
                localPoint = Random.insideUnitSphere * radius;
                localPoint.y += halfHeight;
            }

            // Handle capsule orientation
            Vector3 finalLocal = localPoint + capsule.center;

            switch (capsule.direction)
            {
                case 0: 
                    finalLocal = new Vector3(localPoint.y, localPoint.x, localPoint.z);
                    break;
                case 2: 
                    finalLocal = new Vector3(localPoint.z, localPoint.y, localPoint.x);
                    break;
            }

            return capsule.transform.TransformPoint(finalLocal);
        }
        private Vector3 GetRandomPointInMeshCollider(MeshCollider meshCollider, int maxAttempts = 25)
        {
            Bounds bounds = meshCollider.bounds;

            for (int i = 0; i < maxAttempts; i++)
            {
                Vector3 point = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y),
                    Random.Range(bounds.min.z, bounds.max.z)
                );

                if (meshCollider.ClosestPoint(point) == point)
                    return point;
            }

            return bounds.center;
        }
    }
}
