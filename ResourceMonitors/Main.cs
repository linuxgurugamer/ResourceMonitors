using KSP.IO;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace ResourceMonitors
{
    public class Main
    {
        internal const string SOUND_DIR = "ResourceMonitors/Sounds/";
        internal const string ALERTMONITORS = "ResourceMonitors";
        internal const string RESNODE = "Resource";
        internal const string GUI = "gui";

        internal const int alarmLength = 10;

        internal const int WIDTH = 590;
        internal const int HEIGHT = 300;

        internal const int SOUND_WIDTH = 120;

        internal static bool common = false;
        internal static List<string> commonResources = new List<string>() { "ElectricCharge", "LiquidFuel", "Oxidizer", "MonoPropellant", "XenonGas" };

        void LoadDefaults()
        {

        }

    }
}
