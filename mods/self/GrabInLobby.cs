using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdkmMenu
{
    public class GrabInLobbyMod
    {
        public static void Toggle()
        {
            Plugin.SelfSettings.ToggleConfigEntry(Plugin.SelfSettings.GrabInLobby);
            foreach (var grabbable in GrabbableObject_Patches.activeGrabbables)
            {
                if (grabbable == null) continue;
                grabbable.itemProperties.canBeGrabbedBeforeGameStart = Plugin.SelfSettings.GrabInLobby.Value;
            }
        }
    }
}
