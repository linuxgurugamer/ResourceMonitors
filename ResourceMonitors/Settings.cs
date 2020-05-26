using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;



namespace ResourceMonitors
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings
    // HighLogic.CurrentGame.Parameters.CustomParams<RM_1>().

    public class RM_1 : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Behaviour"; } } // Column header
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Resource Monitors"; } }
        public override string DisplaySection { get { return "Resource Monitors"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }


        [GameParameters.CustomIntParameterUI("Resource alert count", minValue = 1, maxValue = 99,
           toolTip = "Number of times to repeat the resource alert when resource drops low")]
        public int resourceAlertRepetition = 7;

        [GameParameters.CustomParameterUI("Alarm Enabled")]
        public bool collisionEnabled = true;

        [GameParameters.CustomParameterUI("Backup save before jumping to vessel")]
        public bool BackupSaves = true;

        [GameParameters.CustomParameterUI("Cancel Flight Mode Jump on Backup Failure")]
        public bool CancelFlightModeJumpOnBackupFailure = false;

        [GameParameters.CustomIntParameterUI("Backup saves to keep", minValue = 3, maxValue = 50,
           toolTip = "How many saves to keep")]
        public int BackupSavesToKeep = 3;

        [GameParameters.CustomParameterUI("Use Haystack if available",
            toolTip = "Only useful when Haystack is installed, enable/disable the Haystack interface")]
        public bool useHaystackIfAvailable = true;

        [GameParameters.CustomParameterUI("Snap Haystack to window when called",
                    toolTip = "Only useful when Haystack is installed, will snap the Haystack window to the right edge")]
        public bool snapHaystack = true;

        [GameParameters.CustomParameterUI("Apply common resource monitors to all new vessels",
            toolTip = "Only monitors for resources on the vessel will be added")]
        public bool applyCommonToAll = false;



        public override bool Enabled(MemberInfo member, GameParameters parameters) { return true; }
        public override bool Interactible(MemberInfo member, GameParameters parameters) { return true; }

        public override IList ValidValues(MemberInfo member) { return null; }

    }

    public class RM_2 : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Display & Audio"; } } // Column header
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Resource Monitors"; } }
        public override string DisplaySection { get { return "Resource Monitors"; } }
        public override int SectionOrder { get { return 2; } }
        public override bool HasPresets { get { return false; } }

        [GameParameters.CustomParameterUI("Sound Toggle")]
        public bool soundToggle = true;

        float masterVol = 0.5f;
        [GameParameters.CustomFloatParameterUI("Master Volume (%)", displayFormat = "N0", minValue = 0, maxValue = 100, stepCount = 1, asPercentage = false)]
        public float masterVolume
        {
            get { return masterVol * 100; }
            set { masterVol = value / 100.0f; }
        }

        float previewVol = 0.20f;
        [GameParameters.CustomFloatParameterUI("Preview Volume (%)", displayFormat = "N0", minValue = 0, maxValue = 100, stepCount = 1, asPercentage = false)]
        public float previewVolume
        {
            get { return previewVol * 100; }
            set { previewVol = value / 100.0f; }
        }


        [GameParameters.CustomParameterUI("Alternate skin")]
        public bool altSkin = true;

        [GameParameters.CustomParameterUI("Compact display")]
        public bool compact = false;

        [GameParameters.CustomParameterUI("Use text only for resources")]
        public bool useTextOnly = false;

        [GameParameters.CustomParameterUI("Use icons only for resources",
            toolTip = "Only works when either ARP or ARP Icons are installed")]
        public bool useIconsOnly = false;

        [GameParameters.CustomParameterUI("Use icons and text for resources",
            toolTip = "Only works when either ARP or ARP Icons are installed")]
        public bool useIconsAndText = true;

        [GameParameters.CustomParameterUI("Hide window when game is paused",
            toolTip = "Set to false when looking at the alternative skin & sizes")]
        public bool hideWhenPaused = true;



        bool oldAltSkin = false;
        bool initted = false;

        bool oldUseTextOnly = false;
        bool oldUseIconsOnly = false;
        bool oldUseIconsAndText = false;
        bool oldCompact = false;

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            if (!initted)
            {
                oldAltSkin = altSkin;

                oldUseTextOnly = useTextOnly;
                oldUseIconsOnly = useIconsOnly;
                oldUseIconsAndText = useIconsAndText;
                oldCompact = compact;
                initted = true;
                Main.skinInitialized = true;
            }
            //Main.skinInitialized = (oldAltSkin == altSkin && oldCompact == compact &&
            //    useTextOnly == oldUseTextOnly && useIconsOnly == oldUseIconsOnly &&  useIconsAndText  == oldUseIconsAndText);

            if (useTextOnly && !oldUseTextOnly)
            {
                oldUseTextOnly = true;
                oldUseIconsOnly = oldUseIconsAndText = false;
                useIconsOnly = useIconsAndText = false;
                Main.skinInitialized = false;
            }
            else
            if (useIconsOnly && !oldUseIconsOnly)
            {
                oldUseIconsOnly = true;
                oldUseTextOnly = oldUseIconsAndText = false;
                useTextOnly = useIconsAndText = false;
                Main.skinInitialized = false;
            }
            else
            if (useIconsAndText && !oldUseIconsAndText)
            {
                oldUseIconsAndText = true;
                oldUseTextOnly = oldUseIconsOnly = false;
                useTextOnly = useIconsOnly = false;
                Main.skinInitialized = false;
            }
            if (!useTextOnly && !useIconsOnly && !useIconsAndText)
            {
                useTextOnly = oldUseIconsOnly;
                useIconsAndText = oldUseIconsAndText;
                useIconsOnly = oldUseIconsOnly;
                Main.skinInitialized = false;
            }

            return true; //otherwise return true
        }


        public override bool Interactible(MemberInfo member, GameParameters parameters) { return true; }

        public override IList ValidValues(MemberInfo member) { return null; }

    }

    public class RM_3 : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Reset Common Monitors to defaults"; } } // Column header
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Resource Monitors"; } }
        public override string DisplaySection { get { return "Resource Monitors"; } }
        public override int SectionOrder { get { return 3; } }
        public override bool HasPresets { get { return false; } }

        [GameParameters.CustomParameterUI("Reset Common Resource Monitors",
            toolTip ="The Common Resource Monitors will be reset when the game returns to the Space Center")]
        public bool resetCommon = false;

       
        public override bool Enabled(MemberInfo member, GameParameters parameters)        {            return true;        }

        public override bool Interactible(MemberInfo member, GameParameters parameters) { return true; }

        public override IList ValidValues(MemberInfo member) { return null; }

    }
}
