using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;

namespace TEMPESTCore
{
    [Serializable]
    public class EnemyAudioSettings
    {
        public AudioMixerGroup audioMixerGroup;
        public float volume = 1;
        public float pitch = 1;
        public float spatialBlend = 1;
        public float dopplerLevel = 0;
        public Vector2 minMaxDistance = new Vector2(1, 500);
        public float priority = 15;
    }

    [Serializable]
    public class EnemyAudio
    {
        public int audioId;
    private AudioSource _aud;
        public AudioClip defaultAudioClip;
        private GoreZone _gz;
        public bool overrideSpatialSettings = true;
        public bool overrideAudioMixer;
        public EnemyAudioSettings audioSettings;
        public bool instantiateAndReparentToGorezone;
        public bool taunt;
        public string subtitle;

        public void Initialize(AudioSource aud, GoreZone gz)
        {
            _gz = gz;
            _aud = aud;
        }
        public void Play(int num)
        {
            if (defaultAudioClip == null)
            {
                Debug.LogError("No Audio Set!");
                return;
            }
            if (num != audioId) return;

            if (instantiateAndReparentToGorezone)
            {                
                GameObject newAudioObj = new GameObject("EnemyAudio");
                AudioSource newAud = newAudioObj.AddComponent<AudioSource>();

                if (_aud != null)
                {
                    if (overrideAudioMixer) newAud.outputAudioMixerGroup = audioSettings.audioMixerGroup;
                    else newAud.outputAudioMixerGroup = _aud.outputAudioMixerGroup;
                    newAudioObj.transform.position = _aud.transform.position;
                    newAud.playOnAwake = false;
                }
                newAud.playOnAwake = false;
                newAud.clip = defaultAudioClip;
                newAud.volume = audioSettings.volume;
                newAud.pitch = audioSettings.pitch;

                if (overrideSpatialSettings)
                {
                    newAud.spatialBlend = audioSettings.spatialBlend;
                    newAud.dopplerLevel = audioSettings.dopplerLevel;
                    newAud.minDistance = audioSettings.minMaxDistance.x;
                    newAud.maxDistance = audioSettings.minMaxDistance.y;
                    newAud.rolloffMode = _aud.rolloffMode;
                }

                RemoveOnTime rot = newAudioObj.AddComponent<RemoveOnTime>();
                rot.useAudioLength = true;
                DestroyOnCheckpointRestart docr = newAudioObj.AddComponent<DestroyOnCheckpointRestart>();

                if (_gz != null)
                {
                    newAudioObj.transform.SetParent(_gz.transform);
                }

                newAud.Play();
            }
            else if (_aud != null)
            {
                _aud.clip = defaultAudioClip;
                _aud.volume = audioSettings.volume;
                _aud.pitch = audioSettings.pitch;
                if (overrideSpatialSettings)
                {
                    _aud.spatialBlend = audioSettings.spatialBlend;
                    _aud.dopplerLevel = audioSettings.dopplerLevel;
                    _aud.minDistance = audioSettings.minMaxDistance.x;
                    _aud.maxDistance =  audioSettings.minMaxDistance.y;
                    _aud.rolloffMode = _aud.rolloffMode;
                }
                _aud.Play();
            }
            if (taunt && string.IsNullOrEmpty(subtitle))
            {
                if (MonoSingleton<SubtitleController>.Instance != null)
                    MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(subtitle);
            }
        }
    }
}
