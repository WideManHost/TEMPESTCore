using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TEMPESTCore
{
    [System.Serializable]
    public abstract class SimpleClassWithRequirements
    {
        [Tooltip("Describe what this specific module does here.")]
        public string Title;                 
        public bool activated = true;
        public int id;
        [SerializeReference]
        public Requirements requirements;
        [Space(20f)]
        [NonSerialized] public EnemyIdentifier _eid;
        //anti-unityzipbomb-inator 10000
        [NonSerialized] public SimpleEvents _se;
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
