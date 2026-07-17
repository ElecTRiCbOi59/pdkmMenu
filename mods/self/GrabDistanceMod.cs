using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdkmMenu
{
    public class GrabDistanceMod
    {
        public static void UpdateMod()
        {
            if (!Plugin.SelfSettings.GrabDistance.Value)
            {
                if(GameNetworkManager.Instance.localPlayerController != null)
                {
                    GameNetworkManager.Instance.localPlayerController.grabDistance = 5f;
                }
                return;
            }
            if (GameNetworkManager.Instance.localPlayerController == null) return;
            GameNetworkManager.Instance.localPlayerController.grabDistance = Plugin.SelfSettings.GrabDistanceValue.Value;
        }
    }
}
