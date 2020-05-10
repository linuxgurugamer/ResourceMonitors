using UnityEngine;
using ToolbarControl_NS;

namespace ResourceMonitors
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
