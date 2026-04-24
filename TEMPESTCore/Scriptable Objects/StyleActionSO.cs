using System;
using System.Collections.Generic;
using ULTRAKILL;
using UnityEngine;

namespace TEMPESTCore
{
    public class StyleActionSO : ScriptableObject
    {
        [Tooltip("What the Style Action is being registered as i.e wideWade.Discombobulated")]
        public string styleID = "wideWade.StyleActionExample";
        [Tooltip("The Rich text of the Style Action (What it'll look like on the UI): '<color=red>DISCOMBOBULATED</color>'")]
        public string richStyleText = "<color=red>WIDESTYLEACTIONEXAMPLE</color>";
        [Tooltip("How much points thsi style action gives")]
        public int points = 1;

    }
}
