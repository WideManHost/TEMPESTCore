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
    public class SimpleEvents : MonoBehaviour
    {
        public bool notIfDead;
        public List<SimpleUltrakillEvent> events;
        public List<SimplePeriodicEvent> timedEvents;
        public List<SimpleAudioPlayer> audios;
        public List<SimpleInstantiate> toInstantiate;
        public List<HinesEventProcessor> globalEvents;
        public List<SimpleInstantiate> objectsToInstantiate;
        [HideInInspector]public List<GameObject> instantiatedObjects;
        public bool clearInstantiatedOnDeath;
        public UltrakillEvent onPlayerDeath;
        public UltrakillEvent onDestroy;
        public GameObject instantiateOnDestroy;
        public PlayerDeadChecker deadChecker;


        private AudioSource _aud;
        private GoreZone _gz;
        private EnemyIdentifier _eid;
        private IEnrage _ienrage;
        private bool _isAllowed => !(notIfDead && _eid != null && _eid.dead);
        private bool _isQuitting;

        void OnApplicationQuit() => _isQuitting = true;
        //dude i lowk keep fucking adding new methods whenever i gotta be organized
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
                foreach (SimpleUltrakillEvent e in events)
                {
                    e.Initialize(_eid, this, _ienrage);
                }
            }
            if (timedEvents != null)
            {
                if(_eid != null)
                foreach (SimplePeriodicEvent e in timedEvents)
                {
                    e.Initialize(_eid, this, _ienrage);
                }
            }
            if (audios != null)
            {
                if (_aud == null) return;
                _gz = GetComponentInParent<GoreZone>();
                foreach (SimpleAudioPlayer a in audios)
                {
                    a.InitializeAudio(_aud, _gz);
                }
            }
            if (toInstantiate != null)
            {
                if(_eid == null || _gz == null)
                foreach (SimpleInstantiate i in toInstantiate)
                {
                    i.InitializeInst(_eid, _gz, _ienrage, this);
                }
            }
        }

        private void Start()
        {
            deadChecker.Initialize();

        }
        void OnEnable()
        {
            HinesEventBus.OnCombatEvent += CheckHinesEvent;
            _eid.onDeath.AddListener(OnDeathOrDestroy);
        }
        void OnDestroy()
        {
            onDestroy.Invoke();
            if (instantiateOnDestroy != null)
            {
                GameObject newObject = Instantiate(instantiateOnDestroy, transform.position, transform.rotation);
            }
            OnDeathOrDestroy();
        }
        void OnDisable()
        {
            HinesEventBus.OnCombatEvent -= CheckHinesEvent;
            _eid.onDeath.RemoveListener(OnDeathOrDestroy);
        }
        public void Instantiate(int num)
        {
            instantiatedObjects.RemoveAll(item => item == null);
            if (toInstantiate == null) return;
            toInstantiate.RemoveAll(item => item == null);
            foreach (SimpleInstantiate item in toInstantiate)
            {
                item.Instantiate(num);
            }
        }
        void OnDeathOrDestroy()
        {
            if (clearInstantiatedOnDeath)
            {
                if(instantiatedObjects != null && instantiatedObjects.Count > 0)
                foreach (GameObject g in instantiatedObjects)
                {
                    if (g != null) Destroy(g);
                }
            }
            instantiatedObjects.Clear();
        }
        private void Update()
        {
            if (notIfDead && _eid != null && _eid.dead) return;
            if (timedEvents != null)
            {
                foreach (SimplePeriodicEvent e in timedEvents)
                {
                    e.Tick();
                }
            }
            if(deadChecker != null) deadChecker.Tick();
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
                foreach (SimpleAudioPlayer a in audios)
                {
                    a.Play(num);
                }
            }
        }
        public void HinesEvent(string keyword)
        {
            HinesEventBus.GlobalHinesEvent(keyword);
        }
        private void CheckHinesEvent(string keyword)
        {
            foreach (var listener in globalEvents)
            {
                listener.CallEvent(this, keyword);
            }
        }
    }
}
