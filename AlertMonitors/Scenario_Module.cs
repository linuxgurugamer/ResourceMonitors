using System;
using UnityEngine;
using System.Collections.Generic;

namespace AlertMonitors
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


        public override void OnSave(ConfigNode node)
        {
            try
            {
                double time = DateTime.Now.Ticks;

                ConfigNode alertMonitorsNode = new ConfigNode(Main.ALERTMONITORS);

                foreach (var rmd in defaultRMD)
                {
                    var configNode = rmd.ToConfigNode();
                    alertMonitorsNode.AddNode(Main.RESNODE, configNode);
                }

                node.AddNode(alertMonitorsNode);

                ConfigNode guiSettings = new ConfigNode(Main.GUI);
                guiSettings.AddValue(X, ResourceAlertWindow.windowPosition.x);
                guiSettings.AddValue(Y, ResourceAlertWindow.windowPosition.y);

                guiSettings.AddValue(soundX, ResourceAlertWindow.soundWindowPosition.x);
                guiSettings.AddValue(soundY, ResourceAlertWindow.soundWindowPosition.y);


                
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
                ConfigNode alertMonitorsNode = null;
                if (node.TryGetNode(Main.ALERTMONITORS, ref alertMonitorsNode))
                {
                    double time = DateTime.Now.Ticks;

                    var nodes = alertMonitorsNode.GetNodes(Main.RESNODE);
                    if (nodes != null)
                    {
                        foreach (var configNode in nodes)
                        {
                            ResourceMonitorDef rmd = ResourceMonitorDef.FromConfigNode(configNode);
                            defaultRMD.Add(rmd);
                        }
                    }
                    if (node.TryGetNode(Main.GUI, ref alertMonitorsNode))
                    {
                        if (alertMonitorsNode.HasValue(X))
                            ResourceAlertWindow.windowPosition.x = (float)Double.Parse(alertMonitorsNode.GetValue(X));
                        if (alertMonitorsNode.HasValue(Y))
                            ResourceAlertWindow.windowPosition.y = (float)Double.Parse(alertMonitorsNode.GetValue(Y));

                        if (alertMonitorsNode.HasValue(soundX))
                            ResourceAlertWindow.soundWindowPosition.x = (float)Double.Parse(alertMonitorsNode.GetValue(soundX));
                        if (alertMonitorsNode.HasValue(soundY))
                            ResourceAlertWindow.soundWindowPosition.y = (float)Double.Parse(alertMonitorsNode.GetValue(soundY));
                        

                        ResourceAlertWindow.windowPosition.height = Main.HEIGHT;
                        ResourceAlertWindow.windowPosition.width = Main.WIDTH;
                        Log.Info("ScenarioModule, x, y: " + ResourceAlertWindow.windowPosition.x + ", " + ResourceAlertWindow.windowPosition.y);
                    }

                    time = (DateTime.Now.Ticks - time) / TimeSpan.TicksPerSecond;
                    Log.Info("retrieved ScenarioModule in " + time.ToString("0.000s"));
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[KRnD] OnLoad(): " + e.ToString());
            }
        }

    }
}
