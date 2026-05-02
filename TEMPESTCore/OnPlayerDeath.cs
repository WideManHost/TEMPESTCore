using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TEMPESTCore
{
    /// <summary>
    /// untested kinda should activate when player dies
    /// </summary>
    public class OnPlayerDeath : MonoBehaviour
    {
        private bool _activated;
        public bool dontResetOnRestart;
        public UltrakillEvent onPlayerDeath;
        private NewMovement _nm;
        private PlatformerMovement _pm;
        private bool _playerDead => _nm.dead || _pm.dead;

        private void Awake()
        {
            _nm = MonoSingleton<NewMovement>.Instance;
            _pm = MonoSingleton<PlatformerMovement>.Instance;
        }
        private void Start()
        {
            StatsManager.checkpointRestart += OnRestart;
        }
        private void OnDisable()
        {
            StatsManager.checkpointRestart -= OnRestart;
        }
        private void OnDestroy() 
        {
            StatsManager.checkpointRestart -= OnRestart;
        }
        private void Update()
        {
            if (_playerDead||!_activated)
            {
                _activated = true;
                onPlayerDeath.Invoke();
            }
        }
        public void OnRestart()
        {
            if (dontResetOnRestart) return;
            _activated = false;
        }

    }
}
