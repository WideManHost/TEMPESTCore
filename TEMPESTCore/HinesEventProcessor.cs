using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TEMPESTCore
{
    /// <summary>
    /// Process the global events keyword
    /// </summary>
    [Serializable]
    public class HinesEventProcessor
    {
        public string keyword;
        public float delay;
        public UltrakillEvent OnSuccess;
        public void CallEvent(MonoBehaviour runner, string incomingKeyword)
        {
            if (incomingKeyword != keyword) return;

            if (delay <= 0)
            {
                OnSuccess?.Invoke();
            }
            else
            {
                runner.StartCoroutine(DelayedResponse());
            }
        }
        private IEnumerator DelayedResponse()
        {
            yield return new WaitForSeconds(delay);
            if (this != null) OnSuccess?.Invoke();
        }
    }
}
