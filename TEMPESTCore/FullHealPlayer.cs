using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TEMPESTCore
{
    public class FullHealPlayer : MonoBehaviour
    {
        private Collider _col;
        [Header("If you put this on a trigger it will use OnTriggerEnter")]
        public bool onEnable;
        [Tooltip("Makes the method set hard damage to 0")]
        public bool removeHardDamage;
        void OnTriggerEnter()
        {
            if (_col == null) return;
            FullHeal();
        }
        void OnEnable()
        {
            if (onEnable) FullHeal();
        }
        public void FullHeal()
        {
            if(removeHardDamage) MonoSingleton<NewMovement>.Instance.ForceAntiHP(0.0f, true);
            MonoSingleton<NewMovement>.Instance.FullHeal();
        }
    }
}
