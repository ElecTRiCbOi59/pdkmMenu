using HarmonyLib;
using Unity.Netcode;

[HarmonyPatch(typeof(NetworkBehaviour), "OnDestroy")]
internal class NetworkBehaviour_Patches
{
    [HarmonyPostfix]
    private static void OnDestroy_Postfix(NetworkBehaviour __instance)
    {
        if (__instance == null) return;

        // Clean up Grabbables
        if (__instance is GrabbableObject grabbable)
        {
            GrabbableObject_Patches.activeGrabbables.Remove(grabbable);
        }

        // Clean up Turrets
        if (__instance is Turret turret)
        {
            Turret_Patches.activeTurrets.Remove(turret);
        }

        // Clean up Mines
        if (__instance is Landmine mine)
        {
            Landmine_Patches.activeMines.Remove(mine);
        }

        // Clean up Big Doors
        if (__instance is TerminalAccessibleObject door)
        {
            TerminalAccessibleObject_Patches.activeBigDoors.Remove(door);
        }
        // Clean up SpikeRoofTrap
        if (__instance is SpikeRoofTrap spikeRoofTrap)
        {
            SpikeRoofTrap_Patches.activeSpikeRoofTraps.Remove(spikeRoofTrap);
        }
        // Clean up SpikeRoofTrap
        if (__instance is EntranceTeleport EntranceTeleport)
        {
            EntranceTeleport_Patches.activeEntranceTeleports.Remove(EntranceTeleport);
        }
    }
}