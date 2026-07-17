using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdkmMenu
{
    [HarmonyPatch] // Crucial: This tells Harmony to look inside this class for patches
    internal class BridgeFall
    {
        // Fix for Adamance (Type 2)
        [HarmonyPatch(typeof(BridgeTriggerType2), "AddToBridgeInstabilityServerRpc")]
        [HarmonyPrefix]
        public static bool StopAdamanceBridge()
        {
            // If CanBridgeFall is false, we return false to block the fall logic.
            return Plugin.WorldSettings.CanBridgeFall.Value;
        }

        // Fix for Vow (Durability)
        [HarmonyPatch(typeof(BridgeTrigger), "Update")]
        [HarmonyPrefix]
        public static void KeepBridgeStable(BridgeTrigger __instance)
        {
            // Logic Fix: If the bridge should NOT fall, keep durability at 1.
            if (!Plugin.WorldSettings.CanBridgeFall.Value)
            {
                __instance.bridgeDurability = 1f;
            }
        }
    }
}
