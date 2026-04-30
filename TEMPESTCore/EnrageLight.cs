using UnityEngine;
namespace TEMPESTCore
{
    [System.Serializable]
    public class EnrageLight
    {
        [SerializeField] private Light _light;
        private bool _isEnraged;
        private Color _originalColor;
        public Color enragedColor;
        private float _originalIntensity;
        public float enragedIntensity;
        private float _originalRange;
        public float enragedRange;

        public void Initialize()
        {
            if (_light == null || _isEnraged) return;
            _originalColor = _light.color;
            _originalIntensity = _light.intensity;
            _originalRange = _light.range;
        }
        public void SetEnrageState(bool enrage)
        {
            if (_light == null || _isEnraged == enrage) return;

            if (enrage)
            {
                _light.color = enragedColor;
                _light.intensity = enragedIntensity;
                _light.range = enragedRange;
            }
            else
            {
                _light.color = _originalColor;
                _light.intensity = _originalIntensity;
                _light.range = _originalRange;
            }

            _isEnraged = enrage;
        }
    }
}
