using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace TEMPESTCore
{

    /// <summary>
    /// Subscribes to the event bus and fires an event if fired
    /// </summary>
    public class HinesEventListener : MonoBehaviour
    {
        [Header("Combat Event Listeners")]
        [SerializeField] private List<HinesEventProcessor> listeners = new List<HinesEventProcessor>();

        private void OnEnable()
        {
            HinesEventBus.OnCombatEvent += CheckCombatEvent;
        }

        private void OnDisable()
        {
            HinesEventBus.OnCombatEvent -= CheckCombatEvent;
        }

        private void CheckCombatEvent(string keyword)
        {
            foreach (var listener in listeners)
            {
                listener.CallEvent(this, keyword);
            }
        }

        public void HinesEvent(string keyword)
        {
            HinesEventBus.GlobalHinesEvent(keyword);
        }
    }
}

