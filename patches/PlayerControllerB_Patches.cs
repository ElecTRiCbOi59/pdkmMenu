using GameNetcodeStuff;
using HarmonyLib;
using pdkmMenu;
using System;
using UnityEngine;
using Unity.Netcode;

[HarmonyPatch(typeof(PlayerControllerB))]
internal class PlayerControllerB_Patches
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void Start_Postfix(PlayerControllerB __instance)
    {
        if (__instance == null) return;

        if (__instance.gameObject.GetComponent<Player_ESP>() == null)
        {
            __instance.gameObject.AddComponent<Player_ESP>();
        }
    }
    [HarmonyPatch("LateUpdate")]
    [HarmonyPostfix]
    public static void LateUpdate_Postfix(PlayerControllerB __instance)
    {
        if (__instance == null) return;

        if(Plugin.SelfSettings.AlwaysShowUserNames.Value)
        {
            if(__instance.isPlayerDead || __instance.isPlayerControlled)
            {
                __instance.ShowNameBillboard();
            }
        }
    }
    [HarmonyPatch("CheckConditionsForSinkingInQuicksand")]
    [HarmonyPrefix]
        public static bool Prefix(ref bool __result)
        {
            // If AntiQuickSand is ENABLED (Value is true)
            if (Plugin.SelfSettings.AntiQuickSand.Value)
            {
                // Force the game to think the conditions for sinking are NOT met
                __result = false;

                // Skip the original method entirely
                return false;
            }

            // If the mod is off, run the original code normally
            return true;
        }

    [HarmonyPatch("SetHoverTipAndCurrentInteractTrigger")]
    [HarmonyPrefix]
    public static void SetHoverTip_Prefix(PlayerControllerB __instance)
    {
        // Layer 21 is MapHazards. We add it to the mask so the raycast doesn't ignore it.
        // bit 0 = Layer 0 (Default - Doors)
        // bit 6 = Layer 6 (Props - Turrets/Doors)
        // bit 13 = Layer 13 (Default - Spikes)
        // bit 21 = Layer 21 (MapHazards - Mines)
        int mask = (1 << 0) | (1 << 6) | (1 << 21);
        var currentMask = (int)ReflectionUtil.ReflectField<PlayerControllerB>(__instance, "interactableObjectsMask");
        currentMask |= mask;
        ReflectionUtil.ReflectSetField<PlayerControllerB>(__instance, "interactableObjectsMask", currentMask);
    }
}