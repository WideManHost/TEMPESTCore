using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TEMPESTCore
{
    public class SetParentToObjectsParent : ActiveStateChecker
    {
        [Tooltip("The Object that will become the selected object's sibling in the Hierarchy.")]
        Transform targetTransformToParent;

        public override void FireBehavior() => ChangeObjectHierarchy();


        public void ChangeObjectHierarchy()
        {
            transform.parent = targetTransformToParent.parent;
        }
    }
}
