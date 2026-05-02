using System;
using UnityEngine;

namespace TEMPESTCore
{
    //this doesnt work 
    [Serializable]
    public class Comment
    {
        [Header("Comment")]
        [TextArea(3, 10)]
        public string comment;
    }
}
