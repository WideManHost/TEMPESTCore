using System;
using System.Collections.Generic;
using System.Text;

namespace TEMPESTCore
{
    [Serializable]
    public class SimpleEnemyStates : SimpleClassWithRequirements
    {
        public string stateName;
        public float hpRequirement;
        public bool over;
        public UltrakillEvent onState;

        bool hpValid()
        {
            if (hpRequirement == 0) return true;
            return over ? hpRequirement > this._eid.health : this._eid.health > hpRequirement;
        }




    }
}
