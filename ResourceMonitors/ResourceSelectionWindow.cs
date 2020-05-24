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
        void ResourceSelectionWindow(int id)
        {
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            resourceSelScrollVector = GUILayout.BeginScrollView(resourceSelScrollVector);
            int cnt = 0;
            foreach (var resource in resourceList)
            {
                GUILayout.BeginHorizontal();
                {
                    if (lastSelectedResource != resource)
                        Main.labelStyle.normal.textColor = Color.red;
                    else
                        Main.labelStyle.normal.textColor = Color.green;
                }
                GUILayout.Space(60);
                if (GUILayout.Button(PartResourceLibrary.Instance.resourceDefinitions[resource].displayName, Main.labelStyle))
                {

                    lastSelectedResource = resource;
                }
                cnt++;
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            //GUILayout.FlexibleSpace();
            GUILayout.Space(10);

            // GUILayout.FlexibleSpace();
            GUILayout.Space(10);
            if (GUILayout.Button("OK", GUILayout.Width(90)))
            {
                resourceAlert.resname = lastSelectedResource;
                resourceSelectionWindow = false;
            }
            //GUILayout.FlexibleSpace();
            GUILayout.Space(10);
            if (GUILayout.Button("Cancel", GUILayout.Width(90)))
            {
                resourceSelectionWindow = false;
            }
            //GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }
}
