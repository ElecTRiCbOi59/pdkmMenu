using HarmonyLib;
using pdkmMenu;
using System.Collections.Generic;
using UnityEngine;

[HarmonyPatch(typeof(Landmine))]
internal class Landmine_Patches
{
    public static List<Landmine> activeMines = new List<Landmine>();

    [HarmonyPatch(typeof(Landmine), "Start")]
    [HarmonyPostfix]
    public static void Start_Postfix(Landmine __instance)
    {
        if (__instance == null) return;

        if (!activeMines.Contains(__instance)) activeMines.Add(__instance);

        if (__instance.gameObject.GetComponent<Landmine_ESP>() == null)
            __instance.gameObject.AddComponent<Landmine_ESP>();

        string interactKey = IngamePlayerSettings.Instance.playerInput.actions.FindAction("Interact").controls[0].displayName;
        CustomInteractions.SetupTrigger(__instance.gameObject, $"Detonate Mine : [{interactKey}]", (player) => {
            __instance.ExplodeMineServerRpc();
        }, isMine: true);
    }
}