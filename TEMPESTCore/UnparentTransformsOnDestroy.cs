using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TEMPESTCore
{
    /// <summary>
    /// Unparents self on destroy
    /// </summary>
    public class UnparentTransformsOndestroy : MonoBehaviour
    {
        public List<Transform> transformsOnDestroy;

        private void OnDestroy()
        {
            if (transformsOnDestroy == null || transformsOnDestroy.Count != 0)
                throw new Exception("list: transformsOnDestroy, might either be null or have no objects within it.");

            foreach (Transform t in transformsOnDestroy)
            {
                t.SetParent(null);
            }
        }
    }
}
