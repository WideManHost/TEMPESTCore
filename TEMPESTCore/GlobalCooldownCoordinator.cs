using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TEMPESTCore
{
    [Serializable]
    public class GlobalCooldown
    {
        public string keyword;
        public float delay;
        private float _timer;
        public bool restartTimerOnValid;

        public void Tick()
        {
            if (_timer > 0) _timer -= Time.deltaTime;
        }

        public bool Validate(string key)
        {
            if (keyword != key) return false;
            if (_timer <= 0)
            {
                if (restartTimerOnValid) _timer = delay;
                return true;
            }
            return false;
        }
    }
    public class GlobalCooldownCoordinator : MonoBehaviour 
    {
        public List<GlobalCooldown> cooldowns;

    }
}
