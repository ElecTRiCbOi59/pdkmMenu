using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[HarmonyPatch(typeof(EntranceTeleport))]
internal class EntranceTeleport_Patches
{
    public static List<EntranceTeleport> activeEntranceTeleports = new List<EntranceTeleport>();
    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    public static void Awake_Postfix(EntranceTeleport __instance)
    {
        if (__instance == null) return;
        if (!activeEntranceTeleports.Contains(__instance))
        {
            activeEntranceTeleports.Add(__instance);
        }
        __instance.gameObject.AddComponent<Door_ESP>();
    }
}