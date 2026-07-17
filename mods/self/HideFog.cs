using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using HarmonyLib;

namespace pdkmMenu
{
    internal class HideFog
    {
        // Use HashSets to track objects, only store original state in one place
        private static Dictionary<LocalVolumetricFog, bool> FogLibrary = new Dictionary<LocalVolumetricFog, bool>();
        private static Dictionary<Volume, bool> VolumeLibrary = new Dictionary<Volume, bool>();

        [HarmonyPatch(typeof(LocalVolumetricFog), "Awake")]
        [HarmonyPostfix]
        private static void RecordFog(LocalVolumetricFog __instance) => RegisterFog(__instance);

        private static void RegisterFog(LocalVolumetricFog fog)
        {
            if (fog == null) return;
            if (!FogLibrary.ContainsKey(fog))
            {
                // If mod is ON, assume default is TRUE. If mod is OFF, save actual.
                bool defaultState = Plugin.SelfSettings.HideFog.Value ? true : fog.enabled;
                FogLibrary.Add(fog, defaultState);
            }
            if (Plugin.SelfSettings.HideFog.Value) fog.enabled = false;
        }

        private static void RegisterVolume(Volume vol)
        {
            if (vol == null) return;
            if (!(vol.name.Contains("VolumeMain") || vol.name.Contains("StormVolume"))) return;

            if (!VolumeLibrary.ContainsKey(vol))
            {
                bool defaultState = Plugin.SelfSettings.HideFog.Value ? true : vol.enabled;
                VolumeLibrary.Add(vol, defaultState);
            }
            if (Plugin.SelfSettings.HideFog.Value) vol.enabled = false;
        }

        public static void UpdateFogState()
        {
            bool hide = Plugin.SelfSettings.HideFog.Value;

            // CLEANUP NULLS FIRST (Crucial to prevent crashes)
            var deadFogs = FogLibrary.Keys.Where(k => k == null).ToList();
            foreach (var dead in deadFogs) FogLibrary.Remove(dead);

            var deadVols = VolumeLibrary.Keys.Where(k => k == null).ToList();
            foreach (var dead in deadVols) VolumeLibrary.Remove(dead);

            if (hide)
            {
                foreach (var fog in FogLibrary.Keys) if (fog != null) fog.enabled = false;
                foreach (var vol in VolumeLibrary.Keys) if (vol != null) vol.enabled = false;
            }
            else
            {
                // Restore from saved values
                foreach (var entry in FogLibrary) if (entry.Key != null) entry.Key.enabled = entry.Value;
                foreach (var entry in VolumeLibrary) if (entry.Key != null) entry.Key.enabled = entry.Value;
            }
        }

        public static void ResetData()
        {
            FogLibrary.Clear();
            VolumeLibrary.Clear();

            // Use 'false' for includeInactive if crashing persists
            var fogs = Object.FindObjectsOfType<LocalVolumetricFog>(false);
            foreach (var fog in fogs) RegisterFog(fog);

            var volumes = Object.FindObjectsOfType<Volume>(false);
            foreach (var vol in volumes) RegisterVolume(vol);
        }
    }
}