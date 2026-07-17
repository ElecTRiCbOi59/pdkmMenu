using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace pdkmMenu
{
    public class FastClimbMod
    {
        private static float? originalClimbSpeed = null;
        public static void UpdateMod()
        {
            if (StartOfRound.Instance?.localPlayerController == null) return;
            var local = StartOfRound.Instance.localPlayerController;
            if (originalClimbSpeed == null)
            {
                originalClimbSpeed = local.climbSpeed;
            }

            if (Plugin.SelfSettings.FastClimb.Value)
            {
                local.climbSpeed = Plugin.SelfSettings.FastClimbSpeed.Value;
            }
            else
            {
                if (originalClimbSpeed.HasValue)
                {
                    local.climbSpeed = originalClimbSpeed.Value;
                }
            }
        }
    }
}
