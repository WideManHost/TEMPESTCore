using UnityEngine;
namespace TEMPESTCore
{
    [System.Serializable]
    public class EnemyEvent
    {
        public EnemyEventFlags flags;
        public int eventNumber;
        public UltrakillEvent onEvent;
        private EnemyFlagChecker _flagChecker;
        public void Initialize(EnemyIdentifier eid, Enemy enemy)
        {
            _flagChecker = new EnemyFlagChecker(eid, enemy);
        }
        public void ToUltrakillEvent(int num)
        {
            if (num != this.eventNumber) return;
            if (!Validate()) return;
            this.onEvent.Invoke();
        }
        bool Validate()
        {
            return _flagChecker.CheckForFlags(flags);
        }
    }

    public class EnemyEvents : MonoBehaviour
    {
        private EnemyIdentifier _eid;
        private Enemy _en;
        public EnemyEvent[] events;
        public EnemyTimedEvent[] timedEvents;
        private void Awake()
        {
            _eid = GetComponent<EnemyIdentifier>();
            _en = GetComponent<Enemy>();
            if (_eid == null)
            {
                Debug.LogError("No Enemy Identifier Found on ", this);
            }
            if (_en == null)
            {
                Debug.LogError("No Enemy Component Found on ", this);
            }
            if (timedEvents != null)
            {
                foreach (EnemyTimedEvent e in timedEvents)
                {
                    e.Initialize(_eid, _en);
                }
            }
        }
        private void Update()
        {
            if (timedEvents != null)
            {
                foreach (EnemyTimedEvent e in timedEvents)
                {
                    e.Tick();
                }
            }

        }
        public void CallUltrakillEvent(int num)
        {
            foreach (var e in events)
            {
                e.ToUltrakillEvent(num);
            }
        }
    }

}
