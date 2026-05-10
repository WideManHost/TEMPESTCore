using UnityEngine;

namespace TEMPESTCore
{
    public class ResetAllAnimatorTriggers : MonoBehaviour
    {
        private Animator anim;
        public bool onUpdate;

        void Start()
        {
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            if (onUpdate) ResetTriggers();
        }

        public void ResetTriggers()
        {
            foreach (AnimatorControllerParameter param in anim.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Trigger)
                {
                    anim.ResetTrigger(param.name);
                }
            }
        }
    }
}
