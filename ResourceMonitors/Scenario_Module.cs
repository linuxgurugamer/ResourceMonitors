using System;
using UnityEngine;
using System.Collections.Generic;

namespace ResourceMonitors
{
    // This class handels load- and save-operations.
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.EDITOR, GameScenes.FLIGHT, GameScenes.TRACKSTATION, GameScenes.SPACECENTER)]
    class Scenario_Module : ScenarioModule
    {

        internal static List<ResourceMonitorDef> defaultRMD = new List<ResourceMonitorDef>();

        const string X = "x";
        const string Y = "y";

        const string soundX = "soundx";
        const string soundY = "soundy";

        const string resourceX = "resourceX";
        const string resourceY = "resourceY";


        public override void OnSave(ConfigNode node)
        {
            try
            {
                double time = DateTime.Now.Ticks;

                ConfigNode resourceMonitorsNode = new ConfigNode(Main.MODNAME);

                foreach (var rmd in defaultRMD)
                {
                    var configNode = rmd.ToConfigNode();
                    resourceMonitorsNode.AddNode(Main.RESNODE, configNode);
                }

                node.AddNode(resourceMonitorsNode);

                ConfigNode guiSettings = new ConfigNode(Main.GUIName);
                guiSettings.AddValue(X, ResourceAlertWindow.windowPosition.x);
                guiSettings.AddValue(Y, ResourceAlertWindow.windowPosition.y);

                guiSettings.AddValue(soundX, ResourceAlertWindow.soundWindowPosition.x);
                guiSettings.AddValue(soundY, ResourceAlertWindow.soundWindowPosition.y);

                guiSettings.AddValue(resourceX, ResourceAlertWindow.resourceWindowPosition.x);
                guiSettings.AddValue(resourceY, ResourceAlertWindow.resourceWindowPosition.y);



                node.AddNode(guiSettings);

                time = (DateTime.Now.Ticks - time) / TimeSpan.TicksPerSecond;
                Log.Info("saved ScenarioModule in " + time.ToString("0.000s"));
            }
            catch (Exception e)
            {
                Log.Error("OnSave(): " + e.ToString());
            }
        }

        public override void OnLoad(ConfigNode node)
        {
            try
            {
                defaultRMD.Clear();
                ConfigNode resourceMonitorsNode = null;
                if (node.TryGetNode(Main.MODNAME, ref resourceMonitorsNode))
                {
                    double time = DateTime.Now.Ticks;

                    var nodes = resourceMonitorsNode.GetNodes(Main.RESNODE);
                    if (nodes != null)
                    {
                        foreach (var configNode in nodes)
                        {
                            ResourceMonitorDef rmd = ResourceMonitorDef.FromConfigNode(configNode);
                            defaultRMD.Add(rmd);
                        }
                    }
                    if (node.TryGetNode(Main.GUIName, ref resourceMonitorsNode))
                    {
                        if (resourceMonitorsNode.HasValue(X))
                            ResourceAlertWindow.windowPosition.x = (float)Double.Parse(resourceMonitorsNode.GetValue(X));
                        if (resourceMonitorsNode.HasValue(Y))
                            ResourceAlertWindow.windowPosition.y = (float)Double.Parse(resourceMonitorsNode.GetValue(Y));

                        if (resourceMonitorsNode.HasValue(soundX))
                            ResourceAlertWindow.soundWindowPosition.x = (float)Double.Parse(resourceMonitorsNode.GetValue(soundX));
                        if (resourceMonitorsNode.HasValue(soundY))
                            ResourceAlertWindow.soundWindowPosition.y = (float)Double.Parse(resourceMonitorsNode.GetValue(soundY));


                        if (resourceMonitorsNode.HasValue(resourceX))
                            ResourceAlertWindow.resourceWindowPosition.x = (float)Double.Parse(resourceMonitorsNode.GetValue(resourceX));
                        if (resourceMonitorsNode.HasValue(resourceY))
                            ResourceAlertWindow.resourceWindowPosition.y = (float)Double.Parse(resourceMonitorsNode.GetValue(resourceY));



                        ResourceAlertWindow.windowPosition.height = Main.HEIGHT;
                        ResourceAlertWindow.windowPosition.width = Main.WIDTH;

                        ResourceAlertWindow.soundWindowPosition.height = Main.HEIGHT;
                        ResourceAlertWindow.soundWindowPosition.width = Main.SEL_WIN_WIDTH;

                        ResourceAlertWindow.resourceWindowPosition.height = Main.HEIGHT;
                        ResourceAlertWindow.resourceWindowPosition.width = Main.SEL_WIN_WIDTH;


                    }

                    time = (DateTime.Now.Ticks - time) / TimeSpan.TicksPerSecond;
                    Log.Info("retrieved ScenarioModule in " + time.ToString("0.000s"));
                }
                else
                {
                    ResetDefaultRMD();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[KRnD] OnLoad(): " + e.ToString());
            }
        }

        public static void ResetDefaultRMD()
        {
            if (defaultRMD == null)
                defaultRMD = new List<ResourceMonitorDef>();
            else
                defaultRMD.Clear();
            if (Main.initialDefaultRMD != null)
                foreach (var a in Main.initialDefaultRMD)
                    defaultRMD.Add(new ResourceMonitorDef(a));
        }
    }
}
