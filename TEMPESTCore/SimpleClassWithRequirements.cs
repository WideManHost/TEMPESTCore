using System;
using System.Collections.Generic;
using System.Text;

namespace TEMPESTCore
{
    [System.Serializable]
    public abstract class SimpleClassWithRequirements
    {
        public bool activated = true;
        public int id;
        public Requirements requirements;
        [UnityEngine.HideInInspector] public EnemyIdentifier _eid;
        //anti-unityzipbomb-inator 10000
        [System.NonSerialized] public SimpleEvents _se;
        public virtual void Initialize(EnemyIdentifier eid, SimpleEvents se, IEnrage enemy = null)
        {
            _se = se;
            _eid = eid;
            if (requirements != null) requirements.Initialize(eid, enemy);
        }
        public virtual bool Validate()
        {
            if (!activated || requirements == null) return false;
            else return requirements.Validate();
        }
        public virtual void Activate(int id)
        {
            if (id != this.id) return;
            this.Toggle(true);
        }
        public virtual void Deactivate(int id)
        {
            if (id != this.id) return;
            this.Toggle(false);
        }
        public void Toggle(bool state)
        {
            activated = state;
        }
    }
}
