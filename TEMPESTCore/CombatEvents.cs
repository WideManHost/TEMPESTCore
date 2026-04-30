using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace TEMPESTCore
{
    [Serializable]
    public class CombatEventListener
    {
        public string keyword;
        public float delay;
        public UltrakillEvent OnSuccess;
        public void CallEvent(MonoBehaviour runner, string incomingKeyword)
        {
            if (incomingKeyword != keyword) return;

            if (delay <= 0)
            {
                OnSuccess?.Invoke();
            }
            else
            {
                runner.StartCoroutine(DelayedResponse());
            }
        }
        private IEnumerator DelayedResponse()
        {
            yield return new WaitForSeconds(delay);
            if (this != null) OnSuccess?.Invoke();
        }
    }

    public class CombatEvents : MonoBehaviour
    {
        [Header("Combat Event Listeners")]
        [SerializeField] private List<CombatEventListener> listeners = new List<CombatEventListener>();

        private void OnEnable()
        {
            GlobalCombatEventRelay.OnCombatEvent += CheckCombatEvent;
        }

        private void OnDisable()
        {
            GlobalCombatEventRelay.OnCombatEvent -= CheckCombatEvent;
        }

        private void CheckCombatEvent(string keyword)
        {
            foreach (var listener in listeners)
            {
                listener.CallEvent(this, keyword);
            }
        }

        public void CombatEvent(string keyword)
        {
            GlobalCombatEventRelay.CombatEvent(keyword);
        }
    }
}

