// All methods in this file are copied in large part from Kerbal Alarm Clock, and as such
// are covered by the KAC license:

//The MIT License(MIT)
//
//Copyright(c) 2014, David Tregoning
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

namespace ResourceMonitors
{
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    internal class JumpAndBackup : MonoBehaviour
    {
        private static Vessel vesselToJumpTo = null;

        void Start()
        {
            StartCoroutine(MonitorThread());
        }
        IEnumerator MonitorThread()
        {
            WaitForSeconds wfs = new WaitForSeconds(1f);

            while (true)
            {
                if (vesselToJumpTo != null)
                {
                    FlightGlobals.SetActiveVessel(vesselToJumpTo);
                    vesselToJumpTo = null;                    
                }
                yield return wfs;
            }
        }

        internal static Boolean JumpToVessel(Vessel vTarget)
        {
            Boolean blnJumped = true;
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (BackupSaves() || !HighLogic.CurrentGame.Parameters.CustomParams<AlertMonitor>().CancelFlightModeJumpOnBackupFailure)
                    vesselToJumpTo = vTarget;

                //if(FlightGlobals.SetActiveVessel(vTarget))
                //{
                //    FlightInputHandler.SetNeutralControls();
                //}
                else
                {
                    Log.Info("Not Switching - unable to backup saves");
                    ShowBackupFailedWindow("Not Switching - unable to backup saves");
                    blnJumped = false;
                }
            }
            else
            {
                Log.Info("Switching in by Save");

                int intVesselidx = getVesselIdx(vTarget);
                if (intVesselidx < 0)
                {
                    LogFormatted("Couldn't find the index for the vessel {0}({1})", vTarget.vesselName, vTarget.id.ToString());
                    ShowBackupFailedWindow("Not Switching - unable to find vessel index");
                    blnJumped = false;
                }
                else
                {
                    try
                    {
                        if (BackupSaves())
                        {
                            String strret = GamePersistence.SaveGame("AMJumpToShip", HighLogic.SaveFolder, SaveMode.OVERWRITE);
                            Game tmpGame = GamePersistence.LoadGame(strret, HighLogic.SaveFolder, false, false);
                            FlightDriver.StartAndFocusVessel(tmpGame, intVesselidx);
                            //if (tmpAlarm.PauseGame)
                            //FlightDriver.SetPause(false);
                            //tmpGame.Start();
                        }
                        else
                        {
                            LogFormatted("Not Switching - unable to backup saves");
                            ShowBackupFailedWindow("Not Switching - unable to backup saves");
                            blnJumped = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogFormatted("Unable to save/load for jump to ship: {0}", ex.Message);
                        ShowBackupFailedWindow("Not Switching - failed in loading new position");
                        blnJumped = false;
                    }
                }
            }
            return blnJumped;
        }

        internal static String SavePath;
        internal static String PathApp = KSPUtil.ApplicationRootPath.Replace("\\", "/");

        internal static Boolean BackupSaves()
        {
            JumpAndBackup.SavePath = string.Format("{0}saves/{1}", JumpAndBackup.PathApp, HighLogic.SaveFolder);

            if (!HighLogic.CurrentGame.Parameters.CustomParams<AlertMonitor>().BackupSaves)
            {
                return true;
            }

            Boolean blnReturn = false;
            LogFormatted("Backing up saves");

            if (!System.IO.Directory.Exists(SavePath))
            {
                LogFormatted("Saves Path not found: {0}");
            }
            else
            {
                if (!System.IO.File.Exists(String.Format("{0}/persistent.sfs", SavePath)))
                {
                    LogFormatted("Persistent.sfs file not found: {0}/persistent.sfs", SavePath);
                }
                else
                {
                    try
                    {
                        System.IO.File.Copy(String.Format("{0}/persistent.sfs", SavePath),
                                            String.Format("{0}/zAMBACKUP{1:yyyyMMddHHmmss}-persistent.sfs", SavePath, DateTime.Now),
                                            true);
                        LogFormatted("Backed Up Persistent.sfs as: {0}/zAMBACKUP{1:yyyyMMddHHmmss}-persistent.sfs", SavePath, DateTime.Now);

                        //Now go for the quicksave
                        if (System.IO.File.Exists(String.Format("{0}/quicksave.sfs", SavePath)))
                        {
                            System.IO.File.Copy(String.Format("{0}/quicksave.sfs", SavePath),
                                                String.Format("{0}/zAMBACKUP{1:yyyyMMddHHmmss}-quicksave.sfs", SavePath, DateTime.Now),
                                                true);
                            LogFormatted("Backed Up quicksave.sfs as: {0}/zAMBACKUP{1:yyyyMMddHHmmss}-quicksave.sfs", SavePath, DateTime.Now);
                        }
                        blnReturn = true;

                        PurgeOldBackups();

                    }
                    catch (Exception ex)
                    {
                        LogFormatted("Unable to backup: {0}/persistent.sfs\r\n\t{1}", SavePath, ex.Message);
                    }
                }
            }

            return blnReturn;
        }

        private static void PurgeOldBackups()
        {
            PurgeOldBackups("persistent.sfs");
            PurgeOldBackups("quicksave.sfs");
        }

        private static void PurgeOldBackups(String OriginalName)
        {
            //Now delete any old ones greater than the list to keep
            List<System.IO.FileInfo> SaveBackups = new System.IO.DirectoryInfo(SavePath).GetFiles(string.Format("KACBACKUP*{0}", OriginalName)).ToList<System.IO.FileInfo>();
            SaveBackups.AddRange(new System.IO.DirectoryInfo(SavePath).GetFiles(string.Format("zAMBACKUP*{0}", OriginalName)).ToList<System.IO.FileInfo>());

            LogFormatted("{0} KACBackup...{1} Saves found", SaveBackups.Count, OriginalName);

            List<System.IO.FileInfo> SaveBackupsToDelete = SaveBackups.OrderByDescending(fi => fi.CreationTime).Skip(HighLogic.CurrentGame.Parameters.CustomParams<AlertMonitor>().BackupSavesToKeep).ToList<System.IO.FileInfo>();
            LogFormatted("{0} KACBackup...{1} Saves to purge", SaveBackupsToDelete.Count, OriginalName);
            for (int i = SaveBackupsToDelete.Count - 1; i >= 0; i--)
            {
                LogFormatted("\tDeleting {0}", SaveBackupsToDelete[i].Name);

                //bin the loadmeta if it exists too
                string loadmetaFile = SaveBackupsToDelete[i].DirectoryName + "/" + System.IO.Path.GetFileNameWithoutExtension(SaveBackupsToDelete[i].FullName) + ".loadmeta";
                if (System.IO.File.Exists(loadmetaFile))
                {
                    System.IO.File.Delete(loadmetaFile);
                }

                SaveBackupsToDelete[i].Delete();
            }
        }

        internal static String _AssemblyName
        { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name; } }

        /// <summary>
        /// Some Structured logging to the debug file
        /// </summary>
        /// <param name="Message">Text to be printed - can be formatted as per String.format</param>
        /// <param name="strParams">Objects to feed into a String.format</param>
        internal static void LogFormatted(String Message, params object[] strParams)
        {
            Message = String.Format(Message, strParams);                  // This fills the params into the message
            String strMessageLine = String.Format("{0},{2},{1}",
                DateTime.Now, Message,
                _AssemblyName);                                           // This adds our standardised wrapper to each line
            Log.Info(strMessageLine);                        // And this puts it in the log
        }

        #region "BackupFailed Message"
        internal static void ShowBackupFailedWindow(String Message)
        {
            PopupDialog.SpawnPopupDialog
               (
                   new Vector2(0.5f, 0.5f),
                   new Vector2(0.5f, 0.5f),
                   "failedBackup",
                   "Failed Backup",
                   Message,
                   "OK",
                   false,
                   HighLogic.UISkin
               );
        }



        #endregion

        private static int getVesselIdx(Vessel vtarget)
        {
            for (int i = 0; i < FlightGlobals.Vessels.Count; i++)
            {
                if (FlightGlobals.Vessels[i].id == vtarget.id)
                {
                    LogFormatted("Found Target idx={0} ({1})", i, vtarget.id.ToString());
                    return i;
                }
            }
            return -1;
        }
    }
}
