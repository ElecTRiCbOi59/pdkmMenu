using HarmonyLib;
using pdkmMenu;

[HarmonyPatch(typeof(RoundManager), "FinishGeneratingNewLevelClientRpc")]
public static class RoundManagerCleanupPatch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        // 1. Clear and re-scan the new level
        HideFog.ResetData();

        // 2. Ensure current preference is applied
        HideFog.UpdateFogState();
    }
}