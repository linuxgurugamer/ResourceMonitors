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
using Expansions.Missions.Tests;
using HaystackReContinued;
using System.Globalization;

namespace ResourceMonitors
{
#if false
    enum GUIWindow
    {
        OVERVIEW,
        OPTIONS,
        COLLISION,
        RESOURCE
    }
#endif

    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    class ResourceAlertWindow : MonoBehaviour
    {
#if false
        public GUIWindow Window = GUIWindow.OPTIONS;
#endif

        int resourceIndex = 0;

        string resourcePercentage = "20";

        internal AlertSoundPlayer soundplayer = null;
        ToolbarControl toolbarControl = null;

        internal static Rect windowPosition = new Rect(Screen.width / 2 - Main.WIDTH / 2, Screen.height / 2 - Main.HEIGHT / 2, Main.WIDTH, Main.HEIGHT);
        internal static Rect soundWindowPosition = new Rect(Screen.width / 2 - Main.SOUND_WIDTH / 2, Screen.height / 2 - Main.HEIGHT / 2, Main.SOUND_WIDTH, Main.HEIGHT);
        private bool visible = false; //Inbuilt "visible" boolean, in case I need it for something else.


        static string[] dirEntries;
        static List<string> soundEntriesList = null;

        internal const string MODID = "AlertMonitors_NS";
        internal const string MODNAME = "Alert Monitors";

        void Start()
        {
            if (toolbarControl == null)
            {
                toolbarControl = gameObject.AddComponent<ToolbarControl>();
                toolbarControl.AddToAllToolbars(GuiOn, GuiOff,
                    ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW,
                    MODID,
                    "dangerAlertButton",
                    "ResourceMonitors/PluginData/Icons/icon_38",
                    "ResourceMonitors/PluginData/Icons/icon_24",
                    MODNAME
                );
            }
            if (soundplayer == null)
            {
                soundplayer = new AlertSoundPlayer();
                soundplayer.Initialize("selection");
            }
#if false
            if (!GUIStyleInitted)
            {
                buttonStyle = new GUIStyle(GUI.skin.button);
                toggleStyle = new GUIStyle(GUI.skin.toggle);
                textFieldStyle = new GUIStyle(GUI.skin.textField);
             //   GUIStyleInitted = true;
            }
#endif
            GameEvents.onVesselChange.Add(onVesselChange);

            Log.Info("windowPosition: " + windowPosition.ToString());
        }
        void onVesselChange(Vessel to)
        {
            Log.Info("onVesselChange: " + to.vesselName);
            GetResourceList(to);
            Get_AlertMonitorsModule(to);
        }



        internal static List<ResourceMonitorDef> workingRMD;



        public void GuiOn()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                workingRMD = null;
                if (FlightGlobals.ActiveVessel != null)
                    workingRMD = FlightGlobals.ActiveVessel.GetComponent<AlertMonitors_Module>().rmdList;
                else
                    workingRMD = null;
                //Log.Info("GuiOn, vessel: " + FlightGlobals.ActiveVessel.vesselName + ",  rmdList.Count: " + workingRMD.Count);
                Get_AlertMonitorsModule();
            }
            else
            {
                workingRMD = Scenario_Module.defaultRMD;
            }
            GetResourceList();

            if (soundEntriesList == null)
            {
                dirEntries = Directory.GetFiles("GameData/" + Main.SOUND_DIR, "*.wav");
                soundEntriesList = new List<string>();
                for (int x = 0; x < dirEntries.Count(); x++)
                {
                    string s = dirEntries[x].Substring(0, dirEntries[x].LastIndexOf('.'));
                    s = s.Substring(s.LastIndexOf('/') + 1);
                    soundEntriesList.Add(s);
                }
            }
            visible = true;
        }

        public void GuiOff()
        {
            visible = false;
            toolbarControl.SetFalse(false);
        }


        public List<string> resourceList = new List<string>();

        public void Get_AlertMonitorsModule(Vessel to = null)
        {
            if (to == null)
                to = FlightGlobals.ActiveVessel;
            workingRMD = null;
            if (to != null)
                workingRMD = to.GetComponent<AlertMonitors_Module>().rmdList;
            else
                workingRMD = null;
        }

        public void GetResourceList(Vessel to = null)
        {
            resourceList.Clear();
            Log.Info("GetResourceList");
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (to == null)
                    to = FlightGlobals.ActiveVessel;
                for (int i = 0; i < to.Parts.Count; i++)
                {
                    Part p = to.Parts[i];
                    Log.Info("Part: " + p.partInfo.title);
                    for (int j = 0; j < p.Resources.Count; j++)
                    {
                        PartResource r = p.Resources[j];
                        Log.Info("Part: " + p.partInfo.title + ", resource: " + r.info.name);
                        if (!resourceList.Contains(r.info.name))
                            resourceList.Add(r.info.name);
                    }
                }
            }
            else
            {
                // Space center, get list of all resources in game ???
                foreach (var r in PartResourceLibrary.Instance.resourceDefinitions)
                {
                    if (!Main.common || Main.commonResources.Contains(r.name))
                        resourceList.Add(r.name);
                }
            }
            Log.Info("GetResourceList, total resources found: " + resourceList.Count);
        }

        void SaveSettings()
        {
        }

        private void OnGUI()
        {
            if (visible)
            {
                windowPosition = ClickThruBlocker.GUILayoutWindow(1987432520, windowPosition, ShowWindow, "Alert Monitors");
            }
#if true
            if (soundSelectionWindow)
            {
                soundWindowPosition = ClickThruBlocker.GUILayoutWindow(12345098, soundWindowPosition, SoundSelectionWindow, "Sound Selection");
            }
#endif
        }

        bool soundSelectionWindow = false;
        Vector2 soundFileSelScrollVector;
        string lastSelectedSoundFile = "";
        bool previewEnabled = false;

        void SoundSelectionWindow(int id)
        {
            GUIStyle toggleStyle = new GUIStyle(GUI.skin.label);

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
                        toggleStyle.normal.textColor = Color.red;
                    else
                        toggleStyle.normal.textColor = Color.green;
                }
                GUILayout.Space(60);
                if (GUILayout.Button(fileName, toggleStyle))
                {

                    lastSelectedSoundFile = fileName;
                    if (previewEnabled)
                    {
                        soundplayer.LoadNewSound(Main.SOUND_DIR + lastSelectedSoundFile, true);
                        soundplayer.SetVolume(HighLogic.CurrentGame.Parameters.CustomParams<AlertMonitor>().masterVolume);
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
#if false
            if (!soundplayer.SoundPlaying()) //If the sound isn't playing, play the sound.
            {
                if (GUILayout.Button("Play Alarm"))
                {
                    soundplayer.LoadNewSound(Main.SOUND_DIR + lastSelectedSoundFile, true);
                    soundplayer.SetVolume(HighLogic.CurrentGame.Parameters.CustomParams<AlertMonitor>().masterVolume);
                    soundplayer.PlaySound(); //Plays sound
                }
            }
            else
            {
                if (GUILayout.Button("Stop Alarm"))
                    soundplayer.StopSound();
            }
#endif


            // GUILayout.FlexibleSpace();
            GUILayout.Space(10);
            if (GUILayout.Button("OK", GUILayout.Width(90)))
            {
                resourceAlert.alarm = lastSelectedSoundFile;
                soundSelectionWindow = false;
            }
            //GUILayout.FlexibleSpace();
            GUILayout.Space(10);
            if (GUILayout.Button("Cancel", GUILayout.Width(90)))
            {
                soundSelectionWindow = false;
            }
            //GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }


        void ShowWindow(int windowId)
        {
            if (visible)
            {
                ShowResourceGUI();
                GUI.DragWindow();
            }
        }

        Vector2 scrollPos;
        ResourceMonitorDef resourceAlert;

        static GUIStyle buttonStyle;
        static GUIStyle toggleStyle;
        static GUIStyle textFieldStyle;
        static bool GUIStyleInitted = false;

        ResourceMonitorDef toDel = null;

        int previewSound = 0;

        void ShowResourceGUI()
        {
#if true
            if (!GUIStyleInitted)
            {
                buttonStyle = new GUIStyle(GUI.skin.button);
                toggleStyle = new GUIStyle(GUI.skin.toggle);
                textFieldStyle = new GUIStyle(GUI.skin.textField);
                GUIStyleInitted = true;
            }
#endif
            toDel = null;
            textFieldStyle.normal.textColor = Color.green;
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (HaystackWrapper.HaystackAvailable)
                {
                    GUILayout.BeginHorizontal(GUILayout.Width(Main.WIDTH));

                    if (GUILayout.Button("Haystack"))
                    {
                        HaystackWrapper.ButtonClick();
                    }
                    if (GUILayout.Button("Get Vessel From Haystack"))
                    {
                        Log.Info("GetVesselFromHaystack");
                        if (HaystackWrapper.SelectedVessel != null)
                        {
                            Log.Info("Vessel name: " + HaystackWrapper.SelectedVessel.vesselName);
                            JumpAndBackup.JumpToVessel(HaystackWrapper.SelectedVessel);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                var b = GUILayout.Toggle(Main.common, "Only allow common resources for new monitors");
                if (b != Main.common)
                {
                    Main.common = b;
                    GetResourceList();
                }
            }

            GUILayout.BeginHorizontal(GUILayout.Width(Main.WIDTH));
            if (GUILayout.Button("Add Resource Monitor"))
            {
                workingRMD.Add(new ResourceMonitorDef("ElectricCharge", "Alarm1", 5f, 1));

                resourceIndex = workingRMD.Count - 1; //sets index to last one
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.Label("Resource", GUILayout.Width(120));
            GUILayout.Space(65);
            GUILayout.Label("Alarm Sound", GUILayout.Width(120));


            GUILayout.Space(50);
            GUILayout.Space(35);

            GUILayout.Label("Amt or % "); // , GUILayout.Width(35));
            //GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Width(Main.WIDTH + 10));
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < workingRMD.Count; i++)
            {

                GUILayout.BeginHorizontal();
                resourceAlert = workingRMD[i];
                if (resourceAlert.Enabled)
                {
                    buttonStyle.normal.textColor = Color.green;
                    toggleStyle.normal.textColor = Color.green;
                    textFieldStyle.normal.textColor = Color.green;
                }
                else
                {
                    buttonStyle.normal.textColor = Color.red;
                    toggleStyle.normal.textColor = Color.red;
                    textFieldStyle.normal.textColor = Color.red;

                }
                int curResource = -1;

                // ****************************************************************************
                //                                                                            *
                if (GUILayout.Button("<", GUILayout.Width(20)))
                {
                    curResource = resourceList.FindIndex(s => s == resourceAlert.resname);
                    if (curResource == 0)
                        curResource = resourceList.Count - 1;
                    else
                        curResource--;

                    resourceAlert.SetResource(PartResourceLibrary.Instance.resourceDefinitions[resourceList[curResource]].displayName);
                }
                GUILayout.Label(resourceAlert.resname, textFieldStyle, GUILayout.Width(120));
                if (GUILayout.Button(">", GUILayout.Width(20)))
                {
                    curResource = resourceList.FindIndex(s => s == resourceAlert.resname);
                    if (curResource == resourceList.Count - 1)
                        curResource = 0;
                    else
                        curResource++;
                    resourceAlert.SetResource(resourceList[curResource]);
                }
                GUILayout.Space(20);
                //                                                                            *
                // ****************************************************************************

                // ****************************************************************************
                //                                                                            *
#if false
                if (GUILayout.Button("<", GUILayout.Width(20)))
                {
                    curResource = soundEntriesList.FindIndex(s => s == resourceAlert.alarm);
                    if (curResource == 0)
                        curResource = soundEntriesList.Count - 1;
                    else
                        curResource--;
                    resourceAlert.alarm = soundEntriesList[curResource];
                }
                GUILayout.Label(resourceAlert.alarm, textFieldStyle, GUILayout.Width(90));
                if (GUILayout.Button(">", GUILayout.Width(20)))
                {
                    curResource = soundEntriesList.FindIndex(s => s == resourceAlert.alarm);
                    if (curResource == soundEntriesList.Count - 1)
                        curResource = 0;
                    else
                        curResource++;
                    resourceAlert.alarm = soundEntriesList[curResource];

                    soundSelectionWindow = !soundSelectionWindow;
                    if (soundSelectionWindow)
                    {
                        lastSelectedSoundFile = resourceAlert.alarm;
                    }
                }
#else
                if (GUILayout.Button(resourceAlert.alarm, GUILayout.Width(90)))
                {
                    soundSelectionWindow = !soundSelectionWindow;
                    if (soundSelectionWindow)
                    {
                        lastSelectedSoundFile = resourceAlert.alarm;
                    }
                }
#endif
                GUILayout.Space(10);
                //GUILayout.FlexibleSpace();
                //                                                                            *
                // ****************************************************************************
                var newMonitorByPercentage = GUILayout.Toggle(resourceAlert.monitorByPercentage, "%", GUILayout.Width(30));
                if (newMonitorByPercentage )
                {
                    if (HighLogic.LoadedSceneIsFlight && !resourceAlert.monitorByPercentage)
                    {
                        //if (resourceAlert.percentage == 0 && resourceAlert.minAmt > 0)
                        {

                            var resource = PartResourceLibrary.Instance.GetDefinition(resourceAlert.resname);
                            FlightGlobals.ActiveVessel.rootPart.GetConnectedResourceTotals(resource.id, out double amount, out double maxAmount, true);
                            resourceAlert.percentage = (float)(100f * resourceAlert.minAmt / maxAmount);
                        }
                        resourceAlert.minAmt = 0;
                    }
                    float f = Math.Max(1f, Math.Min(resourceAlert.percentage,99));
                    GUILayout.Space(10);
                    GUILayout.Label(f.ToString() + "%", GUILayout.Width(21));
                    f = GUILayout.HorizontalSlider(f, 1f, 99f, GUILayout.Width(100));
                    resourceAlert.percentage = (int)f;
                }
                else
                {
                    GUILayout.Space(35);
                    if (HighLogic.LoadedSceneIsFlight && resourceAlert.monitorByPercentage)
                    {
                        if (!newMonitorByPercentage && resourceAlert.percentage > 0 && resourceAlert.minAmt == 0)
                        {

                            var resource = PartResourceLibrary.Instance.GetDefinition(resourceAlert.resname);
                            FlightGlobals.ActiveVessel.rootPart.GetConnectedResourceTotals(resource.id, out double amount, out double maxAmount, true);
                            resourceAlert.minAmt = resourceAlert.percentage / 100f * maxAmount;
                        }
                        resourceAlert.percentage = 0;
                    }

                    string f = resourceAlert.minAmt.ToString("F1");
                    f = GUILayout.TextField(f, GUILayout.Width(100));
                    if (double.TryParse(f, out double d))
                    {
                        resourceAlert.minAmt = Math.Max(0.1, d);
                    }
                }
                resourceAlert.monitorByPercentage = newMonitorByPercentage;
                resourceAlert.Enabled = GUILayout.Toggle(resourceAlert.Enabled, "", toggleStyle);
                GUILayout.Space(5);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                    toDel = resourceAlert;
                GUILayout.Space(5);
                if (previewSound != curResource || !soundplayer.SoundPlaying())
                {
                    if (GUILayout.Button("Preview", GUILayout.Width(60)))
                    {
                        previewSound = curResource;
                        Log.Info("Preview: " + Main.SOUND_DIR + resourceAlert.alarm);
                        soundplayer.LoadNewSound(Main.SOUND_DIR + resourceAlert.alarm, HighLogic.CurrentGame.Parameters.CustomParams<AlertMonitor>().resourceAlertRepetition);
                        soundplayer.SetVolume(HighLogic.CurrentGame.Parameters.CustomParams<AlertMonitor>().masterVolume);

                        soundplayer.PlaySound(true); //Plays sound
                    }
                }
                else
                {
                    if (GUILayout.Button("Stop", GUILayout.Width(60)))
                        soundplayer.StopSound();
                }
                GUILayout.EndHorizontal();

            }
            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (toDel != null)
                workingRMD.Remove(toDel);
            GUILayout.BeginHorizontal(GUILayout.Width(Main.WIDTH));

            if (HighLogic.LoadedSceneIsFlight)
            {
                if (GUILayout.Button("Close"))
                {
                    SaveSettings();
                    GuiOff();
                    soundSelectionWindow = false;
                }
            }
            else
            {
                if (GUILayout.Button("Save"))
                {
                    SaveSettings();
                    GuiOff();
                    soundSelectionWindow = false;
                }
                if (GUILayout.Button("Cancel"))
                {
                    GuiOff();
                    soundSelectionWindow = false;
                }

            }
            GUILayout.EndHorizontal();
        }

        bool ResourceValueCheck()
        {
            bool changed = false;

            if (resourceIndex < 0)
            {
                resourceIndex = 0;
                changed = true;
            }
            try
            {
                if (Int32.Parse(resourcePercentage) < 0 || Int32.Parse(resourcePercentage) > 100)
                {
                    resourcePercentage = "50";
                    changed = true;
                }
            }
            catch (FormatException e)
            {
                resourcePercentage = "50";
                changed = true;
            }

            return changed;
        }


        public Rect GetPosition()
        {
            return windowPosition;
        }

        void OnDestroy()
        {
            Log.Info("ResourceAlertWindow is being destroyed");
            toolbarControl.OnDestroy();
            Destroy(toolbarControl);
            GameEvents.onVesselChange.Remove(onVesselChange);


            SaveSettings();
        }
    }
}
