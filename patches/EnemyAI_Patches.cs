using GameNetcodeStuff;
using HarmonyLib;
using pdkmMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

internal class EnemyAI_Patches
{
    public static List<EnemyAI> activeEnemies = new List<EnemyAI>();

    // --- PATCH 1: The Universal Start Tracker ---
    [HarmonyPatch] // No specific type here because TargetMethods handles it
    internal class UniversalStartTracker
    {
        [HarmonyTargetMethods]
        static IEnumerable<MethodBase> TargetMethods()
        {
            return AccessTools.AllTypes()
                .Where(t => t.IsSubclassOf(typeof(EnemyAI)) && !t.IsAbstract)
                .Select(t => AccessTools.Method(t, "Start"))
                .Where(m => m != null);
        }

        [HarmonyPostfix]
        static void Postfix(EnemyAI __instance)
        {
            if (__instance == null) return;

            if (__instance.gameObject.GetComponent<Enemy_ESP>() == null)
            {
                __instance.gameObject.AddComponent<Enemy_ESP>();
            }

            if (!activeEnemies.Contains(__instance))
            {
                activeEnemies.Add(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(EnemyAI), "PlayerIsTargetable")]
    internal class TargetablePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(PlayerControllerB playerScript, ref bool __result)
        {
            if (playerScript == null) return true;
            if (playerScript != StartOfRound.Instance?.localPlayerController) return true;

            __result = Plugin.SelfSettings.PlayerIsTargetable.Value;
            return false;
        }
    }

    // --- PATCH 3: Cleanup ---
    [HarmonyPatch(typeof(EnemyAI), "OnDestroy")]
    internal class CleanupPatch
    {
        [HarmonyPostfix]
        public static void Postfix(EnemyAI __instance)
        {
            activeEnemies.Remove(__instance);
        }
    }
}