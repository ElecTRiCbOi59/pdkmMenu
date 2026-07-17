using HarmonyLib;
using pdkmMenu;
using System.Collections.Generic;
using UnityEngine;

[HarmonyPatch(typeof(SpikeRoofTrap))]
internal class SpikeRoofTrap_Patches
{
    public static List<SpikeRoofTrap> activeSpikeRoofTraps = new List<SpikeRoofTrap>();

    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void Start_Postfix(SpikeRoofTrap __instance)
    {
        if (__instance == null) return;

        if (!activeSpikeRoofTraps.Contains(__instance))
            activeSpikeRoofTraps.Add(__instance);

        if (__instance.gameObject.GetComponent<SpikeRoofTrap_ESP>() == null)
            __instance.gameObject.AddComponent<SpikeRoofTrap_ESP>();

        GameObject target = __instance.gameObject;

        Transform customMeshTransform = target.transform.Find("CustomInteractMesh");
        GameObject targetContainer;

        if (customMeshTransform == null)
        {
            targetContainer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            targetContainer.name = "CustomInteractMesh";
            targetContainer.transform.SetParent(target.transform, false);
            var col = targetContainer.GetComponent<BoxCollider>();
            if (col != null) col.isTrigger = true;
            var renderer = targetContainer.GetComponent<MeshRenderer>();
            if (renderer != null) renderer.enabled = false;
            targetContainer.layer = 21;
        }
        else
        {
            targetContainer = customMeshTransform.gameObject;
        }

        string interactKey = IngamePlayerSettings.Instance.playerInput.actions.FindAction("Interact").controls[0].displayName;

        CustomInteractions.SetupTrigger(targetContainer, $"Toggle Spike Trap : [{interactKey}]", (player) => {
            __instance.SpikeTrapSlam();
            __instance.SpikeTrapSlamServerRpc((int)player.playerClientId);
        }, isSpikeRoofTrap: true);
    }
}