using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TEMPESTCore
{
    public enum InstantiateParentMode
    {
        asSibling = 0,
        toGorezone = 1,
        unparented = 2,
        asChild = 3,
        parentOverride = 4,
        underTarget,
    }
    [System.Serializable]
    public class InstantiateParentResolve
    {
        public InstantiateParentMode mode;
        public Transform parentOverride;
        private GoreZone _gz;
        private EnemyIdentifier _eid;
        private SimpleEvents _se;
        Transform thisTransform => _eid == null ? _se.transform : _eid.transform;
        Transform target => _eid.target.targetTransform;
        public void Initialize(GoreZone gz, EnemyIdentifier eid, SimpleEvents se)
        {
            _gz = gz;
            _eid = eid;
            _se = se;
        }
        public Transform ResolveParent()
        {
            switch (mode)
            {
                case InstantiateParentMode.asSibling:
                    return thisTransform.parent;
                case InstantiateParentMode.toGorezone:
                    return _gz != null ? _gz.transform : null;
                case InstantiateParentMode.unparented:
                    return null;
                case InstantiateParentMode.parentOverride:
                    return parentOverride;
                case InstantiateParentMode.asChild:
                    return thisTransform;
                case InstantiateParentMode.underTarget:
                    if (_eid != null && _eid.target != null && target != null)
                        return target;
                    return null;
                default: return parentOverride != null ? parentOverride : thisTransform;
            }
        }
    }
}
