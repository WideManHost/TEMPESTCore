namespace TEMPESTCore
{
    [System.Serializable]
    public class EnemyToUltrakillEvent
    {
        public int eventNumber;
        public Requirements requirements;
        public UltrakillEvent onEvent;
#if UNITY_EDITOR
        [SerializeField] private Comment comment;
#endif

        public void Initialize(EnemyIdentifier eid, IEnrage enemy)
        {
            requirements.Initialize(eid, enemy);
        }
        public void ToUltrakillEvent(int num)
        {
            if (num != this.eventNumber) return;
            if (!Validate()) return;
            this.onEvent.Invoke();
        }
        bool Validate()
        {
            return requirements.Validate();
        }
    }
}
