
using HaystackReContinued;

namespace ResourceMonitors
{
    public static class HaystackWrapper
    {

        public delegate void Callback(Vessel vessel);
        static Callback CallbackFunction;
        static public void SetCallback(Callback x)
        {
            CallbackFunction = x;
        }
        static bool? _HaystackAvailable = null;
        static public bool HaystackAvailable
        {
            get
            {
                if (_HaystackAvailable == null)
                    _HaystackAvailable = HasMod("HaystackReContinued");
                return (bool)_HaystackAvailable;
            }
        }
        static public Vessel SelectedVessel
        {
            get
            {
                if (HaystackAvailable)
                    return _SelectedVessel;
                else
                    return null;
            }
        }
        static public Vessel _SelectedVessel { get { return HaystackReContinued.API.SelectedVessel; } }

        static public bool IsVisible {  get { return HaystackReContinued.API.IsVisible(); } }

        static public void ButtonClick()
        {
            if (HaystackAvailable)
                _ButtonClick();
        }
        static public void _ButtonClick()
        {
            if (HaystackReContinued.API.fetch != null)
            {
                Main.Log.Info("ResourceMonitors.ButtonClick");
                HaystackReContinued.API.ButtonClick();

                HaystackReContinued.API.SetVisibility(VesselType.Debris, false);
                HaystackReContinued.API.SetVisibility(VesselType.SpaceObject, false);
                HaystackReContinued.API.SetVisibility(VesselType.Unknown, false);
                HaystackReContinued.API.SetVisibility(VesselType.Probe, true);
                HaystackReContinued.API.SetVisibility(VesselType.Relay, true);
                HaystackReContinued.API.SetVisibility(VesselType.Rover, true);
                HaystackReContinued.API.SetVisibility(VesselType.Lander, true);
                HaystackReContinued.API.SetVisibility(VesselType.Ship, true);
                HaystackReContinued.API.SetVisibility(VesselType.Plane, true);
                HaystackReContinued.API.SetVisibility(VesselType.Station, true);
                HaystackReContinued.API.SetVisibility(VesselType.Base, true);
                HaystackReContinued.API.SetVisibility(VesselType.EVA, false);
                HaystackReContinued.API.SetVisibility(VesselType.Flag, false);
                HaystackReContinued.API.SetVisibility(VesselType.DeployedScienceController, true);
                HaystackReContinued.API.SetVisibility(VesselType.DeployedSciencePart, true);

                if (HighLogic.CurrentGame.Parameters.CustomParams<RM_1>().snapHaystack)
                    HaystackReContinued.API.SetPosition(ResourceAlertWindow.windowPosition.x + ResourceAlertWindow.windowPosition.width, ResourceAlertWindow.windowPosition.y);
            }
        }

        private static bool HasMod(string modIdent)
        {
            foreach (AssemblyLoader.LoadedAssembly a in AssemblyLoader.loadedAssemblies)
            {
                if (modIdent == a.name)
                    return true;
            }
            return false;
        }

#if false

        public static Assembly haystackAssembly;
        static bool haystackAvailable = false;

        /// <summary>
        /// Verify the loaded assembly meets a minimum version number.
        /// </summary>
        /// <param name="name">Assembly name</param>
        /// <param name="version">Minium version</param>
        /// <param name="silent">Silent mode</param>
        /// <returns>The assembly if the version check was successful.  If not, logs and error and returns null.</returns>
        public static Assembly VerifyAssemblyVersion(string name, string version, bool silent = false)
        {
            Log.Info("Entering VerifyAssemblyVersion");
            // Logic courtesy of DMagic
            var assembly = AssemblyLoader.loadedAssemblies.SingleOrDefault(a => a.assembly.GetName().Name == name);
            if (assembly != null)
            {
                string receivedStr;

                // First try the informational version
                var ainfoV = Attribute.GetCustomAttribute(assembly.assembly, typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute;
                if (ainfoV != null)
                {
                    receivedStr = ainfoV.InformationalVersion;
                }
                // If that fails, use the product version
                else
                {
                    receivedStr = FileVersionInfo.GetVersionInfo(assembly.assembly.Location).ProductVersion;
                }

                System.Version expected = ParseVersion(version);
                System.Version received = ParseVersion(receivedStr);

                if (received >= expected)
                {
                    Log.Info("Version check for '" + name + "' passed.  Minimum required is " + version + ", version found was " + receivedStr);
                    return assembly.assembly;
                }
                else
                {
                    Log.Error("Version check for '" + name + "' failed!  Minimum required is " + version + ", version found was " + receivedStr);
                    return null;
                }
            }
            else
            {
                Log.Error("Couldn't find assembly for '" + name + "'!");
                return null;
            }
        }

        private static System.Version ParseVersion(string version)
        {
            Match m = Regex.Match(version, @"^[vV]?(\d+)(.(\d+)(.(\d+)(.(\d+))?)?)?");
            int major = m.Groups[1].Value.Equals("") ? 0 : Convert.ToInt32(m.Groups[1].Value);
            int minor = m.Groups[3].Value.Equals("") ? 0 : Convert.ToInt32(m.Groups[3].Value);
            int build = m.Groups[5].Value.Equals("") ? 0 : Convert.ToInt32(m.Groups[5].Value);
            int revision = m.Groups[7].Value.Equals("") ? 0 : Convert.ToInt32(m.Groups[7].Value);

            return new System.Version(major, minor, build, revision);
        }

        /// <summary>
        /// Verifies that the Historian version the player has is compatible.
        /// </summary>
        /// <returns>Whether the check passed.</returns>
        public static bool VerifyHaystackVersion()
        {
            string minVersion = "0.5.6.0";
            if (haystackAssembly == null)
            {

                haystackAssembly = VerifyAssemblyVersion("HaystackContinued", minVersion);
                haystackAvailable = (haystackAssembly != null);
            }
            return haystackAssembly != null;
        }

        public static bool set_m_Active()
        {
            if (!haystackAvailable)
            {
                return false;
            }

            try
            {
                Type calledType = Type.GetType("HaystackReContinued.API,API");
                if (calledType != null)
                {
                    MonoBehaviour HistorianRef = (MonoBehaviour)UnityEngine.Object.FindObjectOfType(calledType); //assumes only one instance of class Historian exists as this command returns first instance found, also must inherit MonoBehavior for this command to work. Getting a reference to your Historian object another way would work also.
                    if (HistorianRef != null)
                    {
                        MethodInfo myMethod = calledType.GetMethod("set_m_Active", BindingFlags.Instance | BindingFlags.Public);

                        if (myMethod != null)
                        {
                            myMethod.Invoke(HistorianRef, null);
                            return true;
                        }
                        else
                        {
                            Log.Info("set_m_Active not available in Historian");
                            haystackAvailable = false;
                            return false;
                        }
                    }
                    else
                    {
                        Log.Info("HistorianRef  failed");
                        haystackAvailable = false;
                        return false;
                    }
                }
                Log.Info("calledtype failed");
                haystackAvailable = false;
                return false;
            }
            catch (Exception e)
            {
                Log.Info("Error calling type: " + e);
                haystackAvailable = false;
                return false;
            }
        }
#endif
    }
}