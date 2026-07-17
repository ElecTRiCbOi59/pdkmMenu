using GameNetcodeStuff;
using HarmonyLib;
using pdkmMenu;
using System.Collections.Generic;

internal static class EnemyAI_Patches
{
    public static readonly List<EnemyAI> activeEnemies = new();

    // Attach ESP to enemies when they are created.
    // Patching the base EnemyAI.Start avoids Harmony conflicts with modded enemies.
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.Start))]
    internal static class EnemyStartPatch
    {
        [HarmonyPostfix]
        private static void Postfix(EnemyAI __instance)
        {
            if (__instance == null)
            {
                return;
            }

            if (__instance.GetComponent<Enemy_ESP>() == null)
            {
                __instance.gameObject.AddComponent<Enemy_ESP>();
            }

            if (!activeEnemies.Contains(__instance))
            {
                activeEnemies.Add(__instance);
            }
        }
    }

    // Override whether the local player can be targeted by enemies.
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.PlayerIsTargetable))]
    internal static class TargetablePatch
    {
        [HarmonyPrefix]
        private static bool Prefix(
            PlayerControllerB playerScript,
            ref bool __result
        )
        {
            if (playerScript == null)
            {
                return true;
            }

            if (playerScript != StartOfRound.Instance?.localPlayerController)
            {
                return true;
            }

            if (Plugin.SelfSettings == null)
            {
                return true;
            }

            __result = Plugin.SelfSettings.PlayerIsTargetable.Value;
            return false;
        }
    }

    // Remove destroyed enemies from the active list.
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.OnDestroy))]
    internal static class CleanupPatch
    {
        [HarmonyPostfix]
        private static void Postfix(EnemyAI __instance)
        {
            if (__instance != null)
            {
                activeEnemies.Remove(__instance);
            }
        }
    }
}