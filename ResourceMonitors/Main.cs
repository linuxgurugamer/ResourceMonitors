using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP;
using UnityEngine;
using System.IO;
using ToolbarControl_NS;
using Steamworks;
using UnityEngine.UI;

namespace ResourceMonitors
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class Main : MonoBehaviour
    {
        internal static string modDir;
        internal const string MODNAME = "ResourceMonitors";
        internal static string dataDir;
        internal const string SOUND_DIR = MODNAME + "/Sounds/";
        internal static string dataFile;
        internal const string RESNODE = "Resource";
        internal const string GUIName = "gui";

        internal const string DEF_NODENAME = "ResourceAlerts";
        internal const string DEF_DEFAULT_NODENAME = "DefaultResources";
        internal const string DEF_COMMON_NODENAME = "CommonResources";

        internal const string VAL_RESNAME = "resourceName";
        internal const string VAL_MONITOR = "monitorByPercentage";
        internal const string VAL_PERCENT = "percentage";
        internal const string VAL_AMT = "minAmt";
        internal const string VAL_ALARM = "alarm";
        internal const string VAL_ENABLED = "Enabled";


        internal const string ARP_ICONS = "TriggerTech/KSPAlternateResourcePanel/Icons";
        internal const string ALT_ARP_ICONS = "ARPIcons/Icons";
        internal static Dictionary<string, Texture2D> icons = null;
        internal static bool iconsAvailable = false;

        internal static List<ResourceMonitorDef> initialDefaultRMD;

        internal const int alarmLength = 10;

        //internal const int WIDTH = 590;
        internal static int WIDTH = 50;
        internal const int HEIGHT = 300;

        internal const int SEL_WIN_WIDTH = 230;

        internal static bool common = true;
        internal static string[] commonResources = null; // = new string[] { "ElectricCharge", "LiquidFuel", "Oxidizer", "MonoPropellant", "XenonGas" };

        //
        // GUI stuff here
        //
        internal static GUIStyle buttonStyle;
        internal static GUIStyle toggleStyle;
        internal static GUIStyle redButtonStyle;
        internal static GUIStyle labelStyle;
        internal static GUIStyle textFieldStyle;

        internal static GUIStyle btnCompactStyle;
        internal static GUIStyle toggleCompactStyle;
        internal static GUIStyle redButtonCompactStyle;
        internal static GUIStyle labelCompactStyle;
        internal static GUIStyle textFieldCompactStyle;
        internal static GUIStyle horSliderStyle, horSliderCompactStyle;
        internal static GUIStyle thumbStyle, thumbCompactStyle;

        internal static GUIStyle lStyle = null;
        internal static GUIStyle lCompactStyle = null;

        static internal bool skinInitialized = false;

        internal static void InitStyles()
        {
            buttonStyle = new GUIStyle(GUI.skin.button);
            toggleStyle = new GUIStyle(GUI.skin.toggle);
            horSliderStyle = new GUIStyle(GUI.skin.horizontalSlider);
            thumbStyle = new GUIStyle(GUI.skin.horizontalScrollbarThumb);

            redButtonStyle = new GUIStyle(GUI.skin.button);
            redButtonStyle.fontStyle = FontStyle.Bold;
            redButtonStyle.normal.textColor = Color.red;

            labelStyle = new GUIStyle(GUI.skin.label);
            textFieldStyle = new GUIStyle(GUI.skin.textField);

            int compactFontSize = GUI.skin.font.fontSize * 3 / 4;

            btnCompactStyle = new GUIStyle(GUI.skin.button);
            btnCompactStyle.padding = new RectOffset(0, 0, 0, 0);
            btnCompactStyle.margin = new RectOffset(0, 0, 0, 0);
            btnCompactStyle.overflow = new RectOffset(0, 0, 0, 0);
            //btnCompactStyle.imagePosition = ImagePosition.ImageOnly;
            //btnCompactStyle.border = new RectOffset(0, 0, 0, 0);
            btnCompactStyle.fixedHeight = 16;
            btnCompactStyle.fontSize = compactFontSize;

            float toggleCompactHeight = toggleStyle.fixedHeight * .75f;

            toggleCompactStyle = new GUIStyle(GUI.skin.toggle);
            toggleCompactStyle.padding = new RectOffset(0, 0, 0, 0);
            toggleCompactStyle.margin = new RectOffset(0, 0, 0, 0);
            toggleCompactStyle.overflow = new RectOffset(0, 0, 0, 0);
            toggleCompactStyle.alignment = TextAnchor.UpperLeft;
            toggleCompactStyle.fixedHeight = toggleCompactStyle.fixedWidth = toggleCompactHeight; ;
            toggleCompactStyle.fontSize = compactFontSize;

            //Log.Info("slider height: " + horSliderStyle.fixedHeight + " - " + horSliderStyle.stretchHeight);
            //Log.Info("Thumb height: " + thumbStyle.fixedHeight + " - " + thumbStyle.stretchHeight);
            horSliderCompactStyle = new GUIStyle(GUI.skin.horizontalSlider);
            horSliderCompactStyle.fixedHeight = horSliderStyle.fixedHeight * 3/4;
            thumbCompactStyle = new GUIStyle(GUI.skin.horizontalScrollbarThumb);
            thumbCompactStyle.fixedHeight = thumbCompactStyle.fixedHeight * 3/4;

            redButtonCompactStyle = new GUIStyle(GUI.skin.button);
            redButtonCompactStyle.fontStyle = FontStyle.Bold;
            redButtonCompactStyle.normal.textColor = Color.red;
            redButtonCompactStyle.fixedHeight = 16;

            labelCompactStyle = new GUIStyle(GUI.skin.label);
            labelCompactStyle.fixedHeight = 16;
            labelCompactStyle.fontSize = compactFontSize;

            textFieldCompactStyle = new GUIStyle(GUI.skin.textField);
            textFieldCompactStyle.fixedHeight = 16;
            textFieldCompactStyle.fontSize = compactFontSize;

            lStyle = new GUIStyle(labelStyle);
            lCompactStyle = new GUIStyle(labelStyle);
            lCompactStyle.fixedHeight = 16;
            lCompactStyle.fontSize = compactFontSize;
            Main.skinInitialized = true;

        }

        void OnGUI()
        {
            if (skinInitialized)
                return;
            InitStyles();


            modDir = KSPUtil.ApplicationRootPath + "GameData/";
            dataDir = modDir + MODNAME + "/PluginData/";
            dataFile = dataDir + "Defaults.cfg";

            LoadDefaults();

            string[] iconList = null;
            string[] altIconList = null;
            Texture2D loadedIcon;
            icons = new Dictionary<string, Texture2D>();
            if (Directory.Exists(modDir + ARP_ICONS))
            {
                iconList = Directory.GetFiles(modDir + ARP_ICONS);
                foreach (var iconpath in iconList)
                {
                    loadedIcon = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                    //var icon = ToolbarControl.LoadImageFromFile(ref loadedIcon, s);
                    if (!ToolbarControl.LoadImageFromFile(ref loadedIcon, iconpath))
                    {
                        Log.Error("Error loading icon from file: " + iconpath);
                    }
                    else
                    {
                        var s1 = System.IO.Path.GetFileName(iconpath);
                        icons.Add(System.IO.Path.GetFileName(iconpath).Substring(0, s1.Length - 4), loadedIcon);
                    }
                }

                if (Directory.Exists(modDir + ALT_ARP_ICONS))
                {
                    altIconList = Directory.GetFiles(modDir + ALT_ARP_ICONS);
                    foreach (var iconpath in altIconList)
                    {
                        loadedIcon = new Texture2D(2, 2);
                        if (!ToolbarControl.LoadImageFromFile(ref loadedIcon, iconpath))
                        {
                            Log.Error("Error loading icon from file: " + iconpath);
                        }
                        else
                        {
                            var s1 = System.IO.Path.GetFileName(iconpath);
                            s1 = s1.Substring(0, s1.Length - 4);
                            if (icons.ContainsKey(s1))
                            {
                                icons.Remove(s1);
                            }

                            icons.Add(s1, loadedIcon);

                        }
                    }

                }
            }
            foreach (var i in icons.Keys)
                Log.Info("icon.key: " + i);
            if (icons != null)
                Log.Info("Number of icons found: " + icons.Count());
            skinInitialized = true;
            Destroy(this);
        }

        void LoadDefaults()
        {
            Log.Info("LoadDefaults, file: " + dataFile + " exists: " + File.Exists(dataFile).ToString());
            if (!File.Exists(dataFile))
                return;
            initialDefaultRMD = new List<ResourceMonitorDef>();
            ConfigNode defaults = ConfigNode.Load(dataFile);
            ConfigNode r = defaults.GetNode(DEF_NODENAME);

            if (r.HasNode(DEF_DEFAULT_NODENAME))
            {
                ConfigNode resAlertnode = r.GetNode(DEF_DEFAULT_NODENAME);
                var resNodes = resAlertnode.GetNodes(RESNODE);
                foreach (var resNode in resNodes)
                {
                    ResourceMonitorDef defaultResource = new ResourceMonitorDef();

                    resNode.TryGetValue(VAL_RESNAME, ref defaultResource.resname);
                    if (!defaultResource.SetResource(defaultResource.resname))
                        Log.Error("defaultResource.resname ("+defaultResource.resname + ") not set correctly");
                    resNode.TryGetValue(VAL_MONITOR, ref defaultResource.monitorByPercentage);
                    resNode.TryGetValue(VAL_PERCENT, ref defaultResource.percentage);
                    resNode.TryGetValue(VAL_AMT, ref defaultResource.minAmt);
                    resNode.TryGetValue(VAL_ALARM, ref defaultResource.alarm);
                    resNode.TryGetValue(VAL_ENABLED, ref defaultResource.Enabled);

                    initialDefaultRMD.Add(defaultResource);

                    Log.Info("Default resource loaded: " + defaultResource.resname);
                }
            }
            if (r.HasNode(DEF_COMMON_NODENAME))
            {
                ConfigNode common = r.GetNode(DEF_COMMON_NODENAME);
                string[] resourceNodes = common.GetValues("resource");
                if (resourceNodes.Length > 0)
                {
                    commonResources = resourceNodes;
                    foreach (var rrr in commonResources)
                    {
                        Log.Info("CommonResource loaded: " + rrr);
                    }
                }
            }
        }

        internal static void WriteDefaults()
        {

        }

    }
}
