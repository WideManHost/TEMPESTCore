using System;
using System.Collections.Generic;
using UnityEngine;
namespace TEMPESTCore
{
    /// <summary>
    /// God class for enemy state control,  has in it usually everything i need - animation events
    /// timed events, playing audio for attacks, planning on adding more
    /// TODO: Add difficulty settings
    /// TODO: find a better name for this  class
    /// TODO: uhhhhh 02.05.26 fuck we are almost halfway through the year
    /// moved TODO to TODO.txt
    /// </summary>
    public class SimpleEnemyEvents : MonoBehaviour
    {
        public bool notIfDead;
        public List<EnemyToUltrakillEvent> events;
        public List<SimpleTimedEvent> timedEvents;
        public List<EnemyAudio> audios;
        public List<HinesEventProcessor> globalEvents;
        public PlayerDeadChecker deadChecker;
        public UltrakillEvent onPlayerDeath;


        private AudioSource _aud;
        private GoreZone _gz;
        private EnemyIdentifier _eid;
        private IEnrage _ienrage;
        private bool _isAllowed => !(notIfDead && _eid != null && _eid.dead);
        private int _currentDifficulty; // TO ADD I AUHUIOERNGAUIEGNUI I NEED TO ADD SUPPORT FOR DIFFICULTIES FUUUUUUUUUUUUCK


        private SubtitleController subtitleController;
        private void Awake()
        {
            _eid = GetComponent<EnemyIdentifier>();
            _ienrage = GetComponent<IEnrage>();
            _aud = GetComponent<AudioSource>();
            _gz = GetComponentInParent<GoreZone>();
            if (_eid == null) Debug.LogError("No Enemy Identifier Found on ", this);

            if (_ienrage == null) Debug.Log("No Enrageable Component Found on ", this);
            if (_aud == null) Debug.LogError("No Audio Source Component Found on ", this);

            if (events != null)
            {
                if (_eid != null)
                foreach (EnemyToUltrakillEvent e in events)
                {
                    e.Initialize(_eid, _ienrage);
                }
            }
            if (timedEvents != null)
            {
                if(_eid != null)
                foreach (SimpleTimedEvent e in timedEvents)
                {
                    e.Initialize(_eid, _ienrage);
                }
            }
            if (audios != null)
            {
                if (_aud == null) return;
                _gz = GetComponentInParent<GoreZone>();
                foreach (EnemyAudio a in audios)
                {
                    a.Initialize(_aud, _gz);
                }
            }
        }

        private void Start()
        {
            subtitleController = MonoSingleton<SubtitleController>.Instance;
            deadChecker.Initialize();
        }
        private void Update()
        {
            if (notIfDead && _eid != null && _eid.dead) return;
            if (timedEvents != null)
            {
                foreach (SimpleTimedEvent e in timedEvents)
                {
                    e.Tick();
                }
            }

        }
        public void CallUltrakillEvent(int num)
        {
            if (_isAllowed) return;
            if (events == null || events.Count <= 0) return;
            foreach (var e in events)
            {
                e.ToUltrakillEvent(num);
            }
        }
        public void DeactivateTimedEvent(int id)
        {
            if (_isAllowed) return;
            if (timedEvents == null || timedEvents.Count <= 0) return;
            foreach (var e in timedEvents)
            {
                e.Deactivate(id);
            }
        }
        public void ActivateTimedEvent(int id)
        {
            if (_isAllowed) return;
            if (timedEvents == null || timedEvents.Count <= 0) return;
            foreach (var e in timedEvents)
            {
                e.Activate(id);
            }
        }
        public void PlaySound(AudioClip audio)
        {
            if (_isAllowed) return;
            if (audio == null || _aud == null) return;
            _aud.clip = audio;
            _aud.Play(true);
        }
        public void PlayFromList(int num)
        {
            if (_isAllowed) return;
            if (audios != null)
            {
                foreach (EnemyAudio a in audios)
                {
                    a.Play(num);
                }
            }
        }

    }
}
