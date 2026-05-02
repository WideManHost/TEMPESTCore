using UnityEngine;
using System.Collections.Generic;

namespace TEMPESTCore
{
    /// <summary>
    /// parent class for my validator shenanigans, there really isnt any point to it but i tried sob
    /// </summary>
    public abstract class Validator
    {
        public abstract bool Validate(EnemyFlags flags, DifficultyRequirement difficulty);
        public abstract void Initialize(EnemyIdentifier eid, IEnrage enrage);
    }
}