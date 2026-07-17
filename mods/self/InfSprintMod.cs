using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdkmMenu
{
    public class InfSprintMod
    {
        public static void UpdateMod()
        {
            if(!Plugin.SelfSettings.InfSprint.Value) return;
            if(GameNetworkManager.Instance.localPlayerController == null) return;
            GameNetworkManager.Instance.localPlayerController.sprintMeter = 100.0f;
        }
    }
}
