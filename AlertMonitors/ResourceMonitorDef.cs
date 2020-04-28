﻿using KSP.IO;
using KSP.UI.Screens.Settings.Controls.Values;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace AlertMonitors
{
    internal class ResourceMonitorDef
    {
        const string RESNAME = "resname";
        const string PERCENTAGE = "percentage";
        const string MINAMT = "minAmt";
        const string ALARM = "alarm";
        const string ENABLED = "Enabled";
        const string RESOURCENAME = "resourceName";


        internal string resname;
        internal float percentage; // in file 0-100 (percentage)
        internal double minAmt;
        internal string alarm;
        internal bool Enabled = true;
        internal PartResourceDefinition prd;
        internal AlertSoundPlayer soundplayer = null;
        internal bool wasPlaying = false;

        internal bool alarmSounding = false;
        internal long alarmStartTime = 0;

        internal ResourceMonitorDef()        { }
        internal ResourceMonitorDef(string resname, string alarm, float percentage, double minAmt)
        {
            this.alarm = alarm;
            this.percentage = percentage;
            this.minAmt = minAmt;

            if (!SetResource(resname))
                Log.Error("Resource can't be set: " + resname);
            if (!InitSoundplayer())
                Log.Error("Soundplayer can't be initialized");
        }

        internal bool SetResource(string resname)
        {
            this.resname = resname;
            this.prd = PartResourceLibrary.Instance.GetDefinition(resname);
            return (prd != null);
        }

        bool InitSoundplayer()
        {
            soundplayer = new AlertSoundPlayer();
            if (soundplayer != null)
                soundplayer.Initialize(resname);
            return (soundplayer != null);
        }

        internal ConfigNode ToConfigNode()
        {
            var configNode = new ConfigNode();
            configNode.AddValue(RESNAME, this.resname);
            configNode.AddValue(PERCENTAGE, this.percentage);

            configNode.AddValue(MINAMT, this.minAmt);
            configNode.AddValue(ALARM, this.alarm);
            configNode.AddValue(ENABLED, this.Enabled);

            configNode.AddValue(RESOURCENAME, this.prd.name);
           
            return configNode;
        }
        internal static ResourceMonitorDef FromConfigNode(ConfigNode configNode)
        {
            ResourceMonitorDef rmd = new ResourceMonitorDef();
            rmd.resname = configNode.GetValue(RESNAME);
            rmd.percentage = float.Parse(configNode.GetValue(PERCENTAGE));
            rmd.minAmt = double.Parse(configNode.GetValue(MINAMT));
            rmd.alarm = configNode.GetValue(ALARM);
            rmd.Enabled = bool.Parse(configNode.GetValue(ENABLED));
            rmd.SetResource(rmd.resname);
            rmd.InitSoundplayer();
            return rmd;
        }
        public override string ToString()
        {
            return "resname: " + resname +
                ", percentage: " + percentage +
                ", minAmt: " + minAmt + 
                ", alarm: " + alarm +
                ", Enabled: " + Enabled               ;
        }
    }
}
