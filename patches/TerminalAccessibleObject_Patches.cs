using pdkmMenu;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;

[HarmonyPatch(typeof(TerminalAccessibleObject))]

internal class TerminalAccessibleObject_Patches
{
    public static List<TerminalAccessibleObject> activeBigDoors = new List<TerminalAccessibleObject>();

    [HarmonyPatch(typeof(TerminalAccessibleObject), "Start")]
    [HarmonyPostfix]
    public static void Start_Postfix(TerminalAccessibleObject __instance)
    {
        if (__instance == null || !__instance.isBigDoor) return;
        if (!activeBigDoors.Contains(__instance)) activeBigDoors.Add(__instance);

        string interactKey = IngamePlayerSettings.Instance.playerInput.actions.FindAction("Interact").controls[0].displayName;
        CustomInteractions.SetupTrigger(__instance.gameObject, $"Toggle Door : [{interactKey}]", (player) => {
            bool isPoweredOn = (bool)ReflectionUtil.ReflectField<TerminalAccessibleObject>(__instance, "isPoweredOn");
            if (!isPoweredOn)
            {
                __instance.OnPowerSwitch(true);
            }
            __instance.SetDoorToggleLocalClient();
        }, isDoor: true);
    }
}