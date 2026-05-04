namespace TEMPESTCore
{
    [System.Serializable]
    public class SimpleUltrakillEvent : SimpleClassWithRequirements
    {

        public UltrakillEvent onEvent;
#if UNITY_EDITOR
        [SerializeField] private Comment comment;
#endif
        public void ToUltrakillEvent(int num)
        {
            if (num != this.id) return;
            if (!Validate()) return;
            this.onEvent.Invoke();
        }
    }
}
