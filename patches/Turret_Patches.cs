using HarmonyLib;
using pdkmMenu;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[HarmonyPatch(typeof(Turret))]
internal class Turret_Patches
{
    public static List<Turret> activeTurrets = new List<Turret>();

    [HarmonyPatch(typeof(Turret), "Start")]
    [HarmonyPostfix]
    public static void Start_Postfix(Turret __instance)
    {
        if (__instance == null) return;
        if (!activeTurrets.Contains(__instance)) activeTurrets.Add(__instance);

        __instance.gameObject.AddComponent<Turret_ESP>();


        string interactKey = IngamePlayerSettings.Instance.playerInput.actions.FindAction("Interact").controls[0].displayName;
        CustomInteractions.SetupTrigger(__instance.gameObject, $"Toggle Turret : [{interactKey}]", (player) => {
            __instance.ToggleTurretServerRpc(!__instance.turretActive);
        }, isTurret: true);
    }
}