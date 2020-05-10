using System;
using UnityEngine;
using System.Collections;

namespace ResourceMonitors
{
    class SafeStopTimeWarp : MonoBehaviour
    {
        static internal Vessel vessel { get; set; } = null;
        public void Start()
        {
            StartCoroutine(MonitorThread());
            //ScreenMessages.PostScreenMessage("Stopping Time Warp", 5);
        }

        public static float UpdateInterval = 0.1F;

        IEnumerator MonitorThread()
        {
            WaitForSeconds wfs = new WaitForSeconds(0.2f);

            Int32 intRate = TimeWarp.fetch.warpRates.Length - 1;
            while (intRate > 0 && (TimeWarp.fetch.warpRates[intRate]) > TimeWarp.CurrentRate)
            {
                intRate -= 1;
            }

            while (intRate >= 0)
            {

                TimeWarp.fetch.Mode = TimeWarp.Modes.HIGH;
                //Make sure we cancel autowarp if its engaged
                TimeWarp.fetch.CancelAutoWarp();
                TimeWarp.SetRate(intRate, false);

                float SecondsTillNextUpdate;
                float dWarpRate = 1;
                if (TimeWarp.fetch != null)
                {
                    dWarpRate = TimeWarp.CurrentRate;
                }
                SecondsTillNextUpdate = UpdateInterval * intRate;

                yield return new WaitForSeconds(SecondsTillNextUpdate);
                intRate--;                
            }
            JumpAndBackup.JumpToVessel(vessel);
            Destroy(this);
        }

    }
}
