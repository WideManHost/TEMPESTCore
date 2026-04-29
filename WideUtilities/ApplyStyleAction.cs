using System;
using System.Collections.Generic;
using ULTRAKILL;
using UnityEngine;

namespace WideUtilities
{
    /// <summary>
    /// Applies a style action, registers the style action if its not already registered.
    /// </summary>
    public class ApplyStyleAction : ActiveStateChecker
    {
        [Tooltip("Custom Style Action Object that will be applied")]
        public StyleActionSO styleAction;

        public override void FireBehavior() => ApplyStyle();

        public void ApplyStyle()
        {
            if (styleAction == null)
                throw new NullReferenceException("Referenced StyleActionSO is null, did you properly set it in the editor?");

            // if it's not registered then lets register!
            if (MonoSingleton<StyleHUD>.Instance.GetLocalizedName(styleAction.styleID) == styleAction.styleID)
            {
                MonoSingleton<StyleHUD>.Instance.RegisterStyleItem(styleAction.styleID, styleAction.richStyleText);
            }
            MonoSingleton<StyleHUD>.Instance.AddPoints(styleAction.points, styleAction.styleID);
        }
        
    }
}
