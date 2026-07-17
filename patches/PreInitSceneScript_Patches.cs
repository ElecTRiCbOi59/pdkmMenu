using HarmonyLib;
using pdkmMenu;

[HarmonyPatch(typeof(PreInitSceneScript))]
internal class PreInitSceneScript_Patches
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void Start_Postfix(PreInitSceneScript __instance)
    {
        if (__instance == null) return;
        if (Plugin.MenuSettings.AutoStart.Value)
        {
            IngamePlayerSettings.Instance.LoadSettingsFromPrefs();
            __instance.ChooseLaunchOption(Plugin.MenuSettings.AutoStartOnline.Value);
        }
    }
}