using System;
using System.Collections.Generic;
using VesselModuleSaveFramework;
using System.Collections;
using UnityEngine;
using System.Linq;
using KSP_Log;

namespace ResourceMonitors
{
    class AlertMonitors_Module : VesselModuleSave
    {

        internal List<ResourceMonitorDef> rmdList = new List<ResourceMonitorDef>();
        bool Paused = false;

        [KSPField(isPersistant = true)]
        bool initted = false;

        bool NotA_Vessel { get { if (Vessel.vesselName.Length < 3) return false; return Vessel.vesselName.Substring(0, 3) == "Ast"; } }
        bool NoCommand {  get { return (Vessel.FindPartModuleImplementing<ModuleCommand>() == null) ; } }


        public override void VSMStart()
        {
            if (NotA_Vessel)
                return;
            if (NoCommand)
                return;

            Main.Log.Info("VSMStart, vessel: " + Vessel.name + ", vesselName: " + Vessel.vesselName + ", vessel.missionTime: " + vessel.missionTime + ", vessel.launchTime: " + vessel.launchTime + ", Planetarium.time: " + Planetarium.fetch.time);
            Main.Log.Info("Name from component: " + this.GetComponent<Vessel>().vesselName);

            GameEvents.onGamePause.Add(OnPause);
            GameEvents.onGameUnpause.Add(OnUnpause);

            if (vessel.missionTime == 0 && Vessel.isActiveVessel && HighLogic.CurrentGame.Parameters.CustomParams<RM_1>().applyCommonToAll)
            {
                ResourceAlertWindow.fetch.GetResourceList(Vessel);

                ResourceAlertWindow.AddCommonResourceMonitors();
            }
            StartCoroutine(MonitorThread());
        }

        public void Destroy()
        {
            GameEvents.onGamePause.Remove(OnPause);
            GameEvents.onGameUnpause.Remove(OnUnpause);
        }


        void OnPause()
        {
            Paused = true;
            if (vessel.loaded)
            {
                for (int i = 0; i < rmdList.Count; i++)
                {
                    if (rmdList[i].soundplayer != null)
                    {
                        if (rmdList[i].soundplayer.SoundPlaying())
                        {
                            rmdList[i].wasPlaying = true;
                            rmdList[i].soundplayer.StopSound();
                        }
                    }
                }
            }
        }

        void OnUnpause()
        {
            Paused = false;

            if (Vessel.loaded)
            {

                if (rmdList == null)
                    Main.Log.Info("rmdList is null, vessel: " + Vessel.name);

                for (int i = 0; i < rmdList.Count; i++)
                {
                    if (rmdList[i].soundplayer != null)
                    {
                        if (rmdList[i].wasPlaying)
                            rmdList[i].soundplayer.PlaySound();
                        rmdList[i].wasPlaying = false;
                    }
                }
            }
        }

        public override ConfigNode VSMSave(ConfigNode node)
        {
            if (NotA_Vessel)
                return node;
            try
            {
                double time = DateTime.Now.Ticks;

                ConfigNode alertMonitorsNode = new ConfigNode(vessel.id.ToString());

                foreach (var rmd in rmdList)
                {
                    var configNode = rmd.ToConfigNode();
                    alertMonitorsNode.AddNode(Main.RESNODE, configNode);
                }

                //node.AddNode(alertMonitorsNode);

                time = (DateTime.Now.Ticks - time) / TimeSpan.TicksPerSecond;
                Main.Log.Info("saved ScenarioModule in " + time.ToString("0.000s"));
                return alertMonitorsNode;
            }
            catch (Exception e)
            {
                Main.Log.Error("OnSave(): " + e.ToString());
            }

            return node;
        }

        public override void VSMLoad(ConfigNode node)
        {
            if (NotA_Vessel)
                return;

            if (!(vessel.missionTime == 0 && Vessel.isActiveVessel && HighLogic.CurrentGame.Parameters.CustomParams<RM_1>().applyCommonToAll))
                rmdList.Clear();

            if (!initted)
            {
               // rmdList.Add(new ResourceMonitorDef("ElectricCharge", "Alarm1", 5f, 1));

                initted = true;
            }

            try
            {
                Main.Log.Info("VSMLoad, configNode: " + node.ToString());
                ConfigNode alertMonitorsNode = null;
                alertMonitorsNode = node;
                    double time = DateTime.Now.Ticks;

                    var nodes = alertMonitorsNode.GetNodes(Main.RESNODE);
                    if (nodes != null)
                    {
                    Main.Log.Info("VSMLoad, nodes.Count: " + nodes.Count());
                        foreach (var configNode in nodes)
                        {
                            ResourceMonitorDef rmd = ResourceMonitorDef.FromConfigNode(configNode);
                            rmdList.Add(rmd);
                        }
                    }

                    time = (DateTime.Now.Ticks - time) / TimeSpan.TicksPerSecond;
                Main.Log.Info("retrieved ScenarioModule in " + time.ToString("0.000s"));
            }
            catch (Exception e)
            {
                Main.Log.Error("[KRnD] OnLoad(): " + e.ToString());
            }

        }

        IEnumerator MonitorThread()
        {
            Main.Log.Info("MonitorThread");
            WaitForSeconds wfs = new WaitForSeconds(1f);

            while (true)
            {
                bool soundActive = HighLogic.CurrentGame.Parameters.CustomParams<RM_2>().soundToggle;

                if (!Paused)
                {
                    for (int i = 0; i < rmdList.Count; i++)
                    {                        
                        rmdList[i].soundplayer.SetVolume(HighLogic.CurrentGame.Parameters.CustomParams<RM_2>().masterVolume);
                        bool lowResource = false;

                        if (GetResourceAmt(rmdList[i].prd.id, out double max, out double cur))
                        {
                            if (rmdList[i].monitorByPercentage &&  rmdList[i].percentage > 0 && cur / max <= rmdList[i].percentage / 100f)
                                lowResource = true;

                            if (!rmdList[i].monitorByPercentage && rmdList[i].minAmt > 0 && cur <= rmdList[i].minAmt)
                                lowResource = true;
                            if (lowResource && soundActive)
                                SoundAlarm(i);
                            else
                                StopAlarm(i);
                        }
                    }
                }
                yield return wfs;
            }
        }

        bool GetResourceAmt(int resid, out double max, out double cur)
        {
            max = cur = 0;
            bool b = false;

            foreach (var p in Vessel.Parts)
            {
                foreach (PartResource r in p.Resources)
                {
                    if (r.info.id == resid && r.flowState)
                    {
                        max += r.maxAmount;
                        cur += r.amount;
                        b = true;
                    }
                }
            }
            return b;
        }

        void SoundAlarm(int i)
        {
            if (!rmdList[i].alarmSounding)
            {
                ScreenMessages.PostScreenMessage("Low Resource Detected for: " + rmdList[i].resname, 5);
                rmdList[i].soundplayer.LoadNewSound(Main.SOUND_DIR + rmdList[i].alarm, HighLogic.CurrentGame.Parameters.CustomParams<RM_1>().resourceAlertRepetition);
                rmdList[i].soundplayer.SetVolume(HighLogic.CurrentGame.Parameters.CustomParams<RM_2>().masterVolume);
                rmdList[i].soundplayer.PlaySound();
                rmdList[i].alarmStartTime = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
                rmdList[i].alarmSounding = true;
                SafeStopTimeWarp.vessel = this.vessel;
                gameObject.AddComponent<SafeStopTimeWarp>();
            }
            else
            {
                if ((DateTime.Now.Ticks / TimeSpan.TicksPerSecond) - rmdList[i].alarmStartTime > Main.alarmLength)
                {
                    rmdList[i].soundplayer.StopSound();
                }
                else
                {
                    if (rmdList[i].soundplayer != null && !rmdList[i].soundplayer.SoundPlaying()) //If the sound isn't playing, play the sound.
                    {
                        if (rmdList[i].soundplayer.altSoundCount-- > 0)
                            rmdList[i].soundplayer.PlaySound();
                    }
                }
            }
        }

        void StopAlarm(int i)
        {
            rmdList[i].soundplayer.StopSound();
            rmdList[i].alarmSounding = false;
        }
    }
}

