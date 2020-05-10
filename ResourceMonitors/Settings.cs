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

    public class AlertMonitor : GameParameters.CustomParameterNode
    {
        public override string Title { get { return ""; } } // Column header
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Alert Monitors"; } }
        public override string DisplaySection { get { return "Alert Monitors"; } }
        public override int SectionOrder { get { return 1; } }
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

        [GameParameters.CustomParameterUI("Snap Haystack to window when called",
            toolTip ="Only useful when Haystack is installed, will snap the Haystack window to the right edge")]
        public bool snapHaystack = true;

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            return true; //otherwise return true
        }

        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            return true;
        }

        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }

    }
}
