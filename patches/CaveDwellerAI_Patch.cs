using HarmonyLib;
using UnityEngine;
using pdkmMenu;
using System.Collections;

namespace pdkmMenu
{
    [HarmonyPatch(typeof(CaveDwellerAI))]
    internal class CaveDwellerAI_Patch
    {
        // We patch the method that triggers the growth
        [HarmonyPatch("StartTransformationAnim")]
        [HarmonyPostfix]
        public static void StartTransformationAnim_Postfix(CaveDwellerAI __instance)
        {
            // Start a small helper coroutine to wait for the mesh swap
            __instance.StartCoroutine(WaitForAdultMesh(__instance));
        }

        private static IEnumerator WaitForAdultMesh(CaveDwellerAI maneater)
        {
            yield return new WaitForSeconds(0.6f);

            Aura aura = maneater.GetComponent<Aura>();
            if (aura != null)
            {
                aura.RefreshGhosts();
            }
        }
    }
}
