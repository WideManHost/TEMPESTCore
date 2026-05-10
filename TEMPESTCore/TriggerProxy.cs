using UnityEngine;
using System;

namespace TEMPESTCore
{
    public class TriggerProxy : MonoBehaviour
    {
        public Action<Collider> onEnter;
        public Action<Collider> onStay; 
        public Action<Collider> onExit;

        private void OnTriggerEnter(Collider other) => onEnter?.Invoke(other);
        private void OnTriggerStay(Collider other) => onStay?.Invoke(other); 
        private void OnTriggerExit(Collider other) => onExit?.Invoke(other);
    }
}
