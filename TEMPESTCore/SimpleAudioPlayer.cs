using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;

namespace TEMPESTCore
{
    [Serializable]
    public class InstanceAudioSettings
    {
        public AudioMixerGroup audioMixerGroup;
        public float volume = 1;
        public float pitch = 1;
        public float spatialBlend = 1;
        public float dopplerLevel = 0;
        public Vector2 minMaxDistance = new Vector2(1, 500);
        public int priority = 15;
    }

    [Serializable]
    public class SimpleAudioPlayer : SimpleClassWithRequirements
    {
        [Header("Audio Settings")]
        private AudioSource _aud;
        public AudioClip defaultAudioClip;
        private GoreZone _gz;
        public bool overrideSpatialSettings = true;
        public bool overrideAudioMixer;
        public InstanceAudioSettings audioSettings;
        public bool instantiateAndReparentToGorezone;
        public bool taunt;
        public string subtitle;

        public void InitializeAudio(AudioSource aud, GoreZone gz)
        {
            _gz = gz;
            _aud = aud;
        }

        public void Play(int num)
        {
            if (num != this.id) return;
            if (!Validate()) return;

            if (instantiateAndReparentToGorezone) InstantiateAudio();
            else 
            {
                SetAudioSettings(_aud);
                OverrideSpatialSettings(_aud);
                _aud.Play();
                HandleTaunt(this.subtitle);
            }
        }
        /// <summary>
        /// handles instantiating and reparenting to gorezone
        /// Initializes the audio source and all the settings
        /// </summary>
        void HandleTaunt(string sub)
        {
            if (!taunt || string.IsNullOrEmpty(sub)) return;
            if (MonoSingleton<SubtitleController>.Instance != null)
                MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(sub);
        }
        void InstantiateAudio()
        {
            //Instantiate the new audio object
            GameObject newAudioObj = new GameObject("EnemyAudio");
            if (_gz != null) newAudioObj.transform.SetParent(_gz.transform);
            if (_aud != null) newAudioObj.transform.position = _aud.transform.position;
            //initialize audio
            AudioSource newAud = newAudioObj.AddComponent<AudioSource>();
            SetAudioSettings(newAud);
            OverrideSpatialSettings(newAud);
            //finish initializing
            RemoveOnTime rot = newAudioObj.AddComponent<RemoveOnTime>();
            rot.useAudioLength = true;
            DestroyOnCheckpointRestart docr = newAudioObj.AddComponent<DestroyOnCheckpointRestart>();
            //play
            newAud.Play();
            HandleTaunt(this.subtitle);
        }
        void SetAudioSettings(AudioSource newAud)
        {
            if (newAud == null) return;
            newAud.outputAudioMixerGroup = overrideAudioMixer ?
             audioSettings.audioMixerGroup : _aud.outputAudioMixerGroup;
            newAud.clip = defaultAudioClip;
            newAud.priority = audioSettings.priority;
            newAud.playOnAwake = false;
            newAud.volume = audioSettings.volume;
            newAud.pitch = audioSettings.pitch;
        }
        void OverrideSpatialSettings (AudioSource newAud)
        {
            if (newAud == null || !overrideSpatialSettings) return;

            newAud.spatialBlend = audioSettings.spatialBlend;
            newAud.dopplerLevel = audioSettings.dopplerLevel;
            newAud.minDistance = Mathf.Min(audioSettings.minMaxDistance.x, audioSettings.minMaxDistance.y);
            newAud.maxDistance = Mathf.Max(audioSettings.minMaxDistance.x, audioSettings.minMaxDistance.y);
                
            if (_aud != null) newAud.rolloffMode = _aud.rolloffMode;
        }
        public override bool Validate()
        {
            if (defaultAudioClip == null){ Debug.LogError("No Audio Set!");return false; }
            if (_aud == null) return false;
            return base.Validate();
        }
    }
}
