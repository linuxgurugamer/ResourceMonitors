using UnityEngine;
using ToolbarControl_NS;

namespace AlertMonitors
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(ResourceAlertWindow.MODID, ResourceAlertWindow.MODNAME);
        }
    }
}
