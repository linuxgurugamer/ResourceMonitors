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
                //string s = resource + "," +PartResourceLibrary.Instance.resourceDefinitions[resource].name +","+
                //    PartResourceLibrary.Instance.resourceDefinitions[resource].GetShortName() +","+
                //    PartResourceLibrary.Instance.resourceDefinitions[resource].displayName + "\r\n";
                //File.AppendAllText("resources.txt", s);

                GUILayout.BeginHorizontal();

                bool b = (lastSelectedResource == resource);
                bool newB = GUILayout.Toggle(b, "");
                if (newB)
                    lastSelectedResource = resource;

                if (lastSelectedResource == resource)
                {
                    Main.lStyle.normal.textColor = Color.green;
                    Main.lCompactStyle.normal.textColor = Color.green;
                }
                else
                {
                    Main.lStyle.normal.textColor = labelStyle.normal.textColor;
                    Main.lCompactStyle.normal.textColor = labelStyle.normal.textColor;
                }

                GUILayout.Label(PartResourceLibrary.Instance.resourceDefinitions[resource].displayName,
                    HighLogic.CurrentGame.Parameters.CustomParams<RM_2>().compact?Main.lCompactStyle:Main.lStyle);
        
                cnt++;
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
    
            if (GUILayout.Button("OK", GUILayout.Width(90)))
            {
                resourceAlert.resname = lastSelectedResource;
                resourceSelectionWindow = false;
            }
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel", GUILayout.Width(90)))
            {
                resourceSelectionWindow = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }
}
