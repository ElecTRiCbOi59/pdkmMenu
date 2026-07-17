using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdkmMenu
{
    public class SpeedHackMod
    {
        public static void UpdateMod()
        {
            if (!Plugin.SelfSettings.SpeedHackEnabled.Value)
            {
                StartOfRound.Instance.localPlayerController.movementSpeed = (float)Plugin.SelfSettings.PlayerSpeed.DefaultValue;
                return;
            }
            StartOfRound.Instance.localPlayerController.movementSpeed = Plugin.SelfSettings.PlayerSpeed.Value;

        }
    }
}
