using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TEMPESTCore
{
    /// <summary>
    /// Set Object Transform to this object's parent
    /// </summary>
    public class SetParentToObjectsParent : ActiveStateChecker
    {
        [Tooltip("The Object that will become the selected object's sibling in the Hierarchy.")]
        Transform targetTransformToParent;

        public override void FireBehavior() => ChangeObjectHierarchy();

        /// improved for versatility, now this can be called through a unityEvent - hines
        public void ChangeObjectHierarchy(Transform child = null)
        {
            if (child = null) transform.parent = targetTransformToParent.parent;
            else transform.parent = child.parent;
        }
    }
}
