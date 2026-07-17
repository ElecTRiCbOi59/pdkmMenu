using GameNetcodeStuff;
using HarmonyLib;
using pdkmMenu;
using UnityEngine;

[HarmonyPatch(typeof(DoorLock))]
public class DoorLock_Patches
{
    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    static void UpdatePatch(DoorLock __instance)
    {
        if (!Plugin.SelfSettings.LockPicker.Value) return;
        if (!__instance.isLocked) return;

        PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;
        if (localPlayer == null) return;

        // Check if the player's crosshair is on the door's interaction trigger
        bool isLookingAtTrigger = localPlayer.hoveringOverTrigger == __instance.doorTrigger ||
                                 (__instance.doorTriggerB != null && localPlayer.hoveringOverTrigger == __instance.doorTriggerB);

        if (isLookingAtTrigger)
        {
            // Change the 'Locked' text to let you know the mod is active
            __instance.doorTrigger.disabledHoverTip = "[E] Force Unlock Door";
            if (__instance.doorTriggerB != null) __instance.doorTriggerB.disabledHoverTip = "[E] Force Unlock Door";

            // Use .triggered to detect the interaction press
            var interactAction = IngamePlayerSettings.Instance.playerInput.actions.FindAction("Interact");

            if (interactAction != null && interactAction.triggered)
            {
                // Unlocks and syncs with all other players
                __instance.UnlockDoorSyncWithServer();

                // Clear the tip immediately so it doesn't flicker 'Locked' for a frame
                HUDManager.Instance.ChangeControlTip(0, "");
            }
        }
    }
}