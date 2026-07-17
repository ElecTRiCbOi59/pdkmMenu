using HarmonyLib;
using pdkmMenu;
using System.Collections.Generic;

[HarmonyPatch(typeof(DepositItemsDesk))]
internal class DepositItemsDesk_Patches
{
    public static DepositItemsDesk depositItemsDesk = null;

    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void Start_Postfix(DepositItemsDesk __instance)
    {
        if (__instance == null) return;
        depositItemsDesk = __instance;
    }
}