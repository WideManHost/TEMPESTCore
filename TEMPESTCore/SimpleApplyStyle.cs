using System;
using UnityEngine;
using ULTRAKILL;

namespace TEMPESTCore
{
    /// <summary>
    /// Style addition module for SimpleEvents
    /// seriously though wade i really couldnt figure out wtf was going on with your apply style SO thing
    /// i literally copied this logic from the secret orb script LMAO
    /// </summary>
    [Serializable]
    public class SimpleApplyStyle
    {
        public int id;
        [Tooltip("The text to display on the HUD (can include rich text tags)")]
        public string styleName;

        [Tooltip("The amount of points to award")]
        public int amount;

        [Tooltip("Trigger the blue parry flash effect")]
        public bool parryFlash;

        public void ApplyStyle(int num)
        {
            if (num != this.id) return;

            if (parryFlash)
            {
                MonoSingleton<TimeController>.Instance.ParryFlash();
            }


            if (!string.IsNullOrEmpty(styleName))
            {
                MonoSingleton<StyleHUD>.Instance.AddPoints(amount, styleName);
            }
            else
            {
                MonoSingleton<StyleHUD>.Instance.AddPoints(amount, "");
            }
        }
    }
}