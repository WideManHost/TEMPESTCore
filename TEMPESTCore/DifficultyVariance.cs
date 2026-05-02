using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TEMPESTCore
{
    [System.Serializable]
    public class DifficultyVariance
    {
        public float harmless = 1f;
        public float lenient = 1f;
        public float standard = 1f;
        public float violent = 1f;
        public float brutal = 1f;
        public float UKMD = 1f;
        public float Calculate(float baseValue)
        {
            if (MonoSingleton<PrefsManager>.Instance == null)
                return baseValue;

            int difficulty = MonoSingleton<PrefsManager>.Instance.GetInt("difficulty");

            float multiplier = 1f;

            switch (difficulty)
            {
                case 0: // Harmless
                    multiplier = harmless;
                    break;
                case 1: // Lenient
                    multiplier = lenient;
                    break;
                case 2: // Standard
                    multiplier = standard;
                    break;
                case 3: // Violent
                    multiplier = violent;
                    break;
                case 4: // Brutal
                    multiplier = brutal;
                    break;
                case 5: // UKMD (UltraKill Must Die)
                    multiplier = UKMD;
                    break;
                default:
                    multiplier = 1f;
                    break;
            }
            return baseValue * multiplier;
        }
    }


}
