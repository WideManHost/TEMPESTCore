using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TEMPESTCore
{
    public class FadeOutLight : MonoBehaviour
    {
        public Light lightSource;

        [Header("Settings")]
        public bool fadeOnEnable = true;
        public bool resetOnEnable = true;
        public bool startFadedOut = false;

        public float duration = 1.0f;
        public bool useRadius = true;
        public bool useIntensity = true;
        public bool destroyOnComplete = false;

        [Header("Effects")]
        public AnimationCurve curve = AnimationCurve.Linear(0, 1, 1, 0);
        public Gradient colorGradient;

        [Header("Delay")]
        public bool useDelay;
        public float delayTime;

        private float _initialIntensity;
        private float _initialRange;
        private Color _initialColor;

        private float _currentProgress = 0f;
        private Coroutine _fadeRoutine;

        private void Awake()
        {
            if (lightSource == null) lightSource = GetComponent<Light>();

            if (lightSource != null)
            {
                _initialIntensity = lightSource.intensity;
                _initialRange = lightSource.range;
                _initialColor = lightSource.color;
            }

            if (startFadedOut)
            {
                _currentProgress = 1f;
                ApplyValues(1f);
            }
        }

        private void OnEnable()
        {
            if (resetOnEnable && !startFadedOut)
            {
                ResetToOriginalValues();
            }

            if (fadeOnEnable)
            {
                TriggerFade(startFadedOut);
            }
        }

        public void TriggerFade(bool reverse)
        {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(FadeLightRoutine(reverse));
        }

        public void ResetToOriginalValues()
        {
            _currentProgress = 0f;
            ApplyValues(0f);
        }

        private IEnumerator FadeLightRoutine(bool fadeIn)
        {
            if (useDelay) yield return new WaitForSeconds(delayTime);

            float target = fadeIn ? 0f : 1f;

            while (!Mathf.Approximately(_currentProgress, target))
            {
                float step = Time.deltaTime / duration;
                _currentProgress = Mathf.MoveTowards(_currentProgress, target, step);

                ApplyValues(_currentProgress);
                yield return null;
            }

            if (_currentProgress >= 0.99f && destroyOnComplete)
            {
                Destroy(gameObject);
            }

            _fadeRoutine = null;
        }

        private void ApplyValues(float progress)
        {
            float curveMultiplier = curve.Evaluate(progress);

            if (useIntensity)
                lightSource.intensity = _initialIntensity * curveMultiplier;

            if (useRadius)
                lightSource.range = _initialRange * curveMultiplier;

            if (colorGradient != null && colorGradient.colorKeys.Length > 0)
                lightSource.color = colorGradient.Evaluate(progress);
        }
    }
}
