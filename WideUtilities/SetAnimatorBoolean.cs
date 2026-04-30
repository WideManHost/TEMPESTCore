using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace WideUtilities
{
    public class SetAnimatorBoolean : MonoBehaviour
    {
        public Animator anim;
        private void Awake()
        {
            anim = GetComponent<Animator>();
            if (anim == null)
            {
                Debug.LogWarning($"No Animator found on {gameObject.name} or its children!", this);
            }
        }
        public void SetTrueByIndex(int num)
        {
            if (anim != null)
                anim.SetBool(num, true);
            else
                Debug.LogWarning("Animator is not assigned!", this);
        }

        public void SetFalseByIndex(int num)
        {
            if (anim != null)
                anim.SetBool(num, false);
            else
                Debug.LogWarning("Animator is not assigned!", this);
        }

        public void SetTrueByName(string boolname)
        {
            if (anim != null && !string.IsNullOrEmpty(boolname))
                anim.SetBool(boolname, true);
            else if (anim == null)
                Debug.LogWarning("Animator is not assigned!", this);
        }

        public void SetFalseByName(string boolname)
        {
            if (anim != null && !string.IsNullOrEmpty(boolname))
                anim.SetBool(boolname, false);
            else if (anim == null)
                Debug.LogWarning("Animator is not assigned!", this);
        }
        public void ToggleByName(string boolname)
        {
            if (anim != null && !string.IsNullOrEmpty(boolname))
            {
                bool currentValue = anim.GetBool(boolname);
                anim.SetBool(boolname, !currentValue);
            }
            else if (anim == null)
            {
                Debug.LogWarning("Animator is not assigned!", this);
            }
        }
        public void ToggleByIndex(int num)
        {
            if (anim != null)
            {
                bool currentValue = anim.GetBool(num);
                anim.SetBool(num, !currentValue);
            }
            else
            {
                Debug.LogWarning("Animator is not assigned!", this);
            }
        }
    }

}
