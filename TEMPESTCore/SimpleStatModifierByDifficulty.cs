using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TEMPESTCore
{
    public enum floatOperationType
    {
        SetToNumber = 0,
        AddNumber = 1,
        MultiplyByNumber = 2,
    }

    [Serializable]
    public class DifficultyByStat
    {
        public bool modifySpeed;
        public floatOperationType speedOperation;
        public DifficultyVariance speedByDifficulty;
        public bool modifyHealth;
        public floatOperationType healthOperation;
        public DifficultyVariance healthByDifficulty;
        public bool modifyDamage;
        public floatOperationType damageOperation;
        public DifficultyVariance damageByDifficulty;

        public float GetSpeedModifier(float speed)
        {
            return speedByDifficulty.Calculate(speed);
        }
        public float GetHealthModifier(float health)
        {
            return healthByDifficulty.Calculate(health);
        }
        public float GetDamageModifier(float dmg)
        {
            return damageByDifficulty.Calculate(dmg);
        }
        
        public bool active => modifyDamage || modifyHealth || modifySpeed;
    }
    public class SimpleStatModifierByDifficulty : MonoBehaviour
    {
        private EnemyIdentifier _eid;
        public DifficultyByStat difficultySettings;
        public bool dontUpdate;
        private int _lastHealthRequests;
        private int _lastSpeedRequests;
        private int _lastDamageRequests;

        void Update()
        {
            if (!dontUpdate) return;
            if (!difficultySettings.active) return;
            bool needsUpdate = false;

            if (_eid.healthBuffRequests != _lastHealthRequests ||
                _eid.speedBuffRequests != _lastSpeedRequests ||
                _eid.damageBuffRequests != _lastDamageRequests)
            {
                needsUpdate = true;
            }

            if (needsUpdate)
            {
                _eid.UpdateBuffs();

                ApplyModifiers();


                _lastHealthRequests = _eid.healthBuffRequests;
                _lastSpeedRequests = _eid.speedBuffRequests;
                _lastDamageRequests = _eid.damageBuffRequests;
            }
        }
        void Awake()
        {
            _eid = GetComponent<EnemyIdentifier>();
        }
        void Start()
        {
            ApplyModifiers();
        }
        private void ApplyModifiers()
        {
            _eid.UpdateBuffs(); 

            if (difficultySettings.modifySpeed)
            {
                _eid.totalSpeedModifier = CalculateStat(
                    _eid.totalSpeedModifier,
                    difficultySettings.speedOperation,
                    difficultySettings.speedByDifficulty
                );
            }


            if (difficultySettings.modifyHealth)
            {
                _eid.totalHealthModifier = CalculateStat(
                    _eid.totalHealthModifier,
                    difficultySettings.healthOperation,
                    difficultySettings.healthByDifficulty
                );
            }


            if (difficultySettings.modifyDamage)
            {
                _eid.totalDamageModifier = CalculateStat(
                    _eid.totalDamageModifier,
                    difficultySettings.damageOperation,
                    difficultySettings.damageByDifficulty
                );
            }
        }

        private float CalculateStat(float baseValue, floatOperationType op, DifficultyVariance variance)
        {

            float modifier = variance.Calculate(baseValue);

            return op switch
            {
                floatOperationType.SetToNumber => modifier,
                floatOperationType.AddNumber => baseValue + modifier,
                floatOperationType.MultiplyByNumber => baseValue * modifier,
                _ => baseValue
            };
        }
    }
}
