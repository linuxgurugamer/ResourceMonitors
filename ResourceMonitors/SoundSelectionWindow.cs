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
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("(click button for preview)");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            soundFileSelScrollVector = GUILayout.BeginScrollView(soundFileSelScrollVector);
            int cnt = 0;
            foreach (var d in soundEntriesList)
            {
                string fileName = d;

                GUILayout.BeginHorizontal();
                bool b = (lastSelectedSoundFile == fileName);
                bool newB = GUILayout.Toggle(b, "", toggleStyle);
                if (newB)
                    lastSelectedSoundFile = fileName;

                if (lastSelectedSoundFile == fileName)
                {
                    Main.lStyle.normal.textColor = Color.green;
                    Main.lCompactStyle.normal.textColor = Color.green;
                }
                else
                {
                    Main.lStyle.normal.textColor = labelStyle.normal.textColor;
                    Main.lCompactStyle.normal.textColor = labelStyle.normal.textColor;
                }


                GUILayout.Label(fileName, HighLogic.CurrentGame.Parameters.CustomParams<RM_2>().compact ? Main.lCompactStyle : Main.lStyle, GUILayout.Width(120));
                if (GUILayout.Button("►", btnStyle, GUILayout.Width(25)))
                {
                    
                        soundplayer.LoadNewSound(Main.SOUND_DIR + fileName, true);
                        soundplayer.SetVolume(HighLogic.CurrentGame.Parameters.CustomParams<RM_2>().previewVolume);
                        soundplayer.PlaySound(); //Plays sound
                    
                }
                cnt++;
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("OK", GUILayout.Width(90)))
            {
                resourceAlert.alarm = lastSelectedSoundFile;
                soundSelectionWindow = false;
                if (soundplayer.SoundPlaying())
                    soundplayer.StopSound();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel", GUILayout.Width(90)))
            {
                soundSelectionWindow = false;
                if (soundplayer.SoundPlaying())
                    soundplayer.StopSound();

            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }
    }
}
