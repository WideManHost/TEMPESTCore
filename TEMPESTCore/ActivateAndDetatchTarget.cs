using UnityEngine;
using UnityEngine.Events;

namespace TEMPESTCore
{
    public class ActivateAndDetachTarget : MonoBehaviour
    {
        public GameObject target;

        [Tooltip("If true, sets parent to null. If false, sets parent to this object's parent.")]
        public bool noParent = true;

        public UltrakillEvent onDetach;

        public void Execute()
        {
            if (target == null)
            {
                Debug.LogWarning($"[{gameObject.name}] ActivateAndDetachTarget: No target assigned!");
                return;
            }

            target.SetActive(true);

            Transform newParent = noParent ? null : transform.parent;
            target.transform.SetParent(newParent);

            onDetach?.Invoke();

        }

        public void Detach(GameObject gb)
        {
            if (gb == null) return;

            gb.SetActive(true);
            gb.transform.SetParent(noParent ? null : transform.parent);
            onDetach?.Invoke();
        }
    }
}