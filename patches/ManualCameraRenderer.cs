using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using GameNetcodeStuff;

namespace pdkmMenu
{
    [HarmonyPatch(typeof(ManualCameraRenderer), "MeetsCameraEnabledConditions")]
    public class CameraConditionsPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ManualCameraRenderer __instance, ref bool __result, PlayerControllerB player)
        {
            if ( __instance.overrideRadarCameraOnAlways)
            {
                __result = true;  // Force the return value to be true
                return false;     // Skip the original method entirely
            }

            return true; // Run the original method if the mod isn't active
        }
    }
}
