using UnityEngine;
namespace WideUtilities
{
    [System.Serializable]
    public class EnrageRenderer
    {
        private bool _materialsAreEnraged;
        public Renderer renderer;

        public Material[] enragedMaterials;

        private Material[] _unenragedMaterials;

        private Material[] _lastSeenArrayReference;
        [SerializeField, HideInInspector]
        private Renderer _previousRenderer;
        public void Initialize()
        {
            if (renderer != null || _materialsAreEnraged)
            {
                _unenragedMaterials = renderer.sharedMaterials;
                _lastSeenArrayReference = _unenragedMaterials;
            }
            else
            {
                Debug.LogWarning("Renderer is not assigned!");
            }
        }
        public void SetEnrageState(bool enrage)
        {
            if (renderer == null || _materialsAreEnraged == enrage) return;

            if (enrage)
            {
                if (enragedMaterials != null && enragedMaterials.Length > 0)
                {
                    renderer.sharedMaterials = enragedMaterials;
                }
            }
            else
            {
                if (_unenragedMaterials != null)
                {
                    renderer.sharedMaterials = _unenragedMaterials;
                    _lastSeenArrayReference = _unenragedMaterials; // Sync the watcher
                }
            }
            _materialsAreEnraged = enrage;
        }

        public void UpdateMaterials()
        {
            if (renderer == null || _materialsAreEnraged) return;
            if (renderer.sharedMaterials != _lastSeenArrayReference) SyncMaterials();
        }

        private void SyncMaterials()
        {
            _unenragedMaterials = renderer.sharedMaterials;
            _lastSeenArrayReference = _unenragedMaterials;
            Debug.Log($"Detected external material change on {renderer.name}. Synced backup.");
        }



#if UNITY_EDITOR
        public void OnValidate()
        {
            if (renderer == null) { _previousRenderer = null; return; }
            bool rendererChanged = renderer != _previousRenderer;
            int requiredCount = renderer.sharedMaterials.Length;

            if (rendererChanged || enragedMaterials == null || enragedMaterials.Length != requiredCount)
            {
                enragedMaterials = new Material[requiredCount];
                for (int i = 0; i < requiredCount; i++)
                {
                    enragedMaterials[i] = renderer.sharedMaterials[i];
                }

                _previousRenderer = renderer;

                Debug.Log($"EnrageRenderer: Auto-populated materials from {renderer.name}.");
            }
        }
    }
#endif

    }
}
