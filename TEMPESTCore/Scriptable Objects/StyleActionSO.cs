using System;
using System.Collections.Generic;
using ULTRAKILL;
using UnityEngine;

namespace TEMPESTCore
{
    [CreateAssetMenu(fileName = "Style Action Object", menuName = "TEMPEST/StyleAction")]
    public class StyleActionSO : ScriptableObject
    {
        [Tooltip("What the Style Action is being registered as i.e wideWade.Discombobulated")]
        public string styleID = "wideWade.StyleActionExample";
        [TextArea(2,4)]
        [Tooltip("The Rich text of the Style Action (What it'll look like on the UI): '<color=red>DISCOMBOBULATED</color>'")]
        public string richStyleText = "<color=red>WIDESTYLEACTIONEXAMPLE</color>";
        [Tooltip("How much points thsi style action gives")]
        public int points = 1;

    }
}
