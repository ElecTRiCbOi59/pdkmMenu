using GameNetcodeStuff;
using HarmonyLib;
using pdkmMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[HarmonyPatch(typeof(GrabbableObject))]
internal class GrabbableObject_Patches
{
    public static List<GrabbableObject> activeGrabbables = new List<GrabbableObject>();

    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void Start_Postfix(GrabbableObject __instance)
    {
        if (__instance == null) return;
        activeGrabbables.Add(__instance);
        __instance.gameObject.AddComponent<Item_ESP>();
       if(Plugin.SelfSettings.GrabInLobby.Value)
        {
            __instance.itemProperties.canBeGrabbedBeforeGameStart = true;
        }
    }
}