using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP;
using UnityEngine;
using System.IO;
using System.Reflection;
using KSP.UI.Screens;

using ClickThroughFix;
using ToolbarControl_NS;

namespace ResourceMonitors
{
    partial class ResourceAlertWindow
    {
        void SoundSelectionWindow(int id)
        {
            //GUIStyle Main.toggleStyle = new GUIStyle(GUI.skin.label);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Sound Selection");
            GUILayout.FlexibleSpace();
            previewEnabled = GUILayout.Toggle(previewEnabled, "Preview Enabled");
            if (!previewEnabled && soundplayer.SoundPlaying())
                soundplayer.StopSound();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            soundFileSelScrollVector = GUILayout.BeginScrollView(soundFileSelScrollVector);
            int cnt = 0;
            foreach (var d in soundEntriesList)
            {
                string fileName = d;

                GUILayout.BeginHorizontal();
                {
                    if (lastSelectedSoundFile != fileName)
                        Main.labelStyle.normal.textColor = Color.red;
                    else
                        Main.labelStyle.normal.textColor = Color.green;
                }
                GUILayout.Space(60);
                if (GUILayout.Button(fileName, Main.labelStyle))
                {

                    lastSelectedSoundFile = fileName;
                    if (previewEnabled)
                    {
                        soundplayer.LoadNewSound(Main.SOUND_DIR + lastSelectedSoundFile, true);
                        soundplayer.SetVolume(HighLogic.CurrentGame.Parameters.CustomParams<RM_2>().masterVolume);
                        soundplayer.PlaySound(); //Plays sound

                    }
                }
                cnt++;
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            //GUILayout.FlexibleSpace();
            GUILayout.Space(10);

            GUILayout.Space(10);
            if (GUILayout.Button("OK", GUILayout.Width(90)))
            {
                resourceAlert.alarm = lastSelectedSoundFile;
                soundSelectionWindow = false;
                if (soundplayer.SoundPlaying())
                    soundplayer.StopSound();
            }
            //GUILayout.FlexibleSpace();
            GUILayout.Space(10);
            if (GUILayout.Button("Cancel", GUILayout.Width(90)))
            {
                soundSelectionWindow = false;
                if (soundplayer.SoundPlaying())
                    soundplayer.StopSound();

            }
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }
    }
}
