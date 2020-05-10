using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using KSP;
using System.ComponentModel.Design.Serialization;

namespace ResourceMonitors
{
    internal class AlertSoundPlayer
    {
        static GameObject alertMonitorObject = null; // new GameObject("alertmonitorplayer"); //Makes the GameObject
        FXGroup source; //The source to be added to the object
        AudioClip loadedClip = null;
#if ALTERNATIVE
        AudioClip alternativeClip = null;
#endif
        internal int altSoundCount;


        public void PlaySound(bool alternative = false)
        {
#if ALTERNATIVE
            if (alternative)
                source.audio.clip = alternativeClip;
            else
#endif
                source.audio.clip = loadedClip;
            source.audio.Play();
        }
        public void SetVolume(float vol)
        {
            if (source.audio != null)
                source.audio.volume = vol / 100;
            else
                Log.Error("source.audio is null");
        }
        public void StopSound()
        {
            // if (source != null && source.audio != null)
            source.audio.Stop();
        }
        public bool SoundPlaying() //Returns true if sound is playing, otherwise false
        {
            if (source != null && source.audio != null)
            {
                return source.audio.isPlaying;
            }
            else
            {
                return false;
            }
        }

        public void LoadNewSound(string soundPath, int cnt)
        {
            altSoundCount = cnt;
            LoadNewSound(soundPath, false);
        }
        public void LoadNewSound(string soundPath, bool alternative = false)
        {
#if ALTERNATIVE
            if (alternative)
                alternativeClip = GameDatabase.Instance.GetAudioClip(soundPath);
            else
#endif
                loadedClip = GameDatabase.Instance.GetAudioClip(soundPath);
            if (loadedClip == null)
                Log.Info("loadedClip is null");
        }
        public void Initialize(string soundPath)
        {
            //Initializing stuff;

            if (alertMonitorObject == null)
                alertMonitorObject = new GameObject(soundPath + "alertMonitorPlayerObject"); //Makes the GameObject
            source = new FXGroup(soundPath + "-alertmonitorplayer");
            source.audio = alertMonitorObject.AddComponent<AudioSource>();
            if (source.audio == null)
                Log.Error("Unable to do alertMonitorPlayer.AddComponent<AudioSource> for: " + soundPath);

            source.audio.volume = 0.5f;
            source.audio.spatialBlend = 0;
        }

    }
}
