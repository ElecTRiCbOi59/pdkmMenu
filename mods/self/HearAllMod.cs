using GameNetcodeStuff;
using HarmonyLib;
using pdkmMenu;
using System.Collections.Generic;
using UnityEngine;

namespace pdkmMenu
{
    [HarmonyPatch(typeof(StartOfRound), "UpdatePlayerVoiceEffects")]
    internal class VoiceCheatPatch
    {
        private struct VoiceAudioState
        {
            public float SpatialBlend;
            public AudioRolloffMode RolloffMode;
            public float MinDistance;
            public float MaxDistance;
            public AnimationCurve CustomRolloffCurve;
            public bool? Set2D;
        }

        private static readonly Dictionary<int, VoiceAudioState> OriginalVoiceStates = new Dictionary<int, VoiceAudioState>();
        private static bool WasHearAllEnabled;

        [HarmonyPostfix]
        public static void Postfix(StartOfRound __instance)
        {
            if (__instance == null || __instance.shipIsLeaving) return;

            bool hearAll = Plugin.SelfSettings.HearAll.Value;

            if (!hearAll)
            {
                if (!WasHearAllEnabled) return;

                for (int i = 0; i < __instance.allPlayerScripts.Length; i++)
                {
                    PlayerControllerB player = __instance.allPlayerScripts[i];

                    if (player == null || player == GameNetworkManager.Instance.localPlayerController)
                        continue;

                    if (player.currentVoiceChatAudioSource != null)
                    {
                        RestoreGlobalVoice(player);
                    }
                }

                OriginalVoiceStates.Clear();
                WasHearAllEnabled = false;
                return;
            }

            WasHearAllEnabled = true;

            for (int i = 0; i < __instance.allPlayerScripts.Length; i++)
            {
                PlayerControllerB player = __instance.allPlayerScripts[i];

                if (player == null || player == GameNetworkManager.Instance.localPlayerController)
                    continue;

                if (player.currentVoiceChatAudioSource != null)
                {
                    ApplyGlobalVoice(player);
                }
            }
        }

        private static void ApplyGlobalVoice(PlayerControllerB player)
        {
            AudioSource audio = player.currentVoiceChatAudioSource;
            CaptureOriginalState(player, audio);

            if (audio.TryGetComponent(out AudioLowPassFilter lowPass)) lowPass.enabled = false;
            if (audio.TryGetComponent(out AudioHighPassFilter highPass)) highPass.enabled = false;

            // Keep voice 3D so direction and distance cues still work.
            audio.spatialBlend = 1f;
            audio.rolloffMode = AudioRolloffMode.Custom;
            audio.minDistance = 1f;
            audio.maxDistance = 100000f;
            // Fade with distance but keep a floor so remote players are always audible.
            float audioLevelAtMaxDistance = 0.1f; // Adjust as needed for balance
            audio.SetCustomCurve(AudioSourceCurveType.CustomRolloff, AnimationCurve.Linear(0f, 1f, 1f, audioLevelAtMaxDistance));
            if (player.currentVoiceChatIngameSettings != null)
            {
                player.currentVoiceChatIngameSettings.set2D = false;
            }

            if (SoundManager.Instance != null)
            {
                int clientId = (int)player.playerClientId;
                float[] pitchTargets = SoundManager.Instance.playerVoicePitchTargets;
                if (pitchTargets != null && clientId >= 0 && clientId < pitchTargets.Length)
                {
                    pitchTargets[clientId] = 1f;
                    SoundManager.Instance.SetPlayerPitch(1f, clientId);
                }
            }
        }

        private static void CaptureOriginalState(PlayerControllerB player, AudioSource audio)
        {
            int audioId = audio.GetInstanceID();
            if (OriginalVoiceStates.ContainsKey(audioId))
                return;

            AnimationCurve originalCurve = null;
            if (audio.rolloffMode == AudioRolloffMode.Custom)
            {
                AnimationCurve curve = audio.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
                if (curve != null)
                {
                    originalCurve = new AnimationCurve(curve.keys);
                }
            }

            VoiceAudioState state = new VoiceAudioState
            {
                SpatialBlend = audio.spatialBlend,
                RolloffMode = audio.rolloffMode,
                MinDistance = audio.minDistance,
                MaxDistance = audio.maxDistance,
                CustomRolloffCurve = originalCurve,
                Set2D = player.currentVoiceChatIngameSettings != null ? player.currentVoiceChatIngameSettings.set2D : (bool?)null
            };

            OriginalVoiceStates[audioId] = state;
        }

        private static void RestoreGlobalVoice(PlayerControllerB player)
        {
            AudioSource audio = player.currentVoiceChatAudioSource;
            int audioId = audio.GetInstanceID();
            if (!OriginalVoiceStates.TryGetValue(audioId, out VoiceAudioState state))
                return;

            audio.spatialBlend = state.SpatialBlend;
            audio.rolloffMode = state.RolloffMode;
            audio.minDistance = state.MinDistance;
            audio.maxDistance = state.MaxDistance;
            if (state.RolloffMode == AudioRolloffMode.Custom && state.CustomRolloffCurve != null)
            {
                audio.SetCustomCurve(AudioSourceCurveType.CustomRolloff, state.CustomRolloffCurve);
            }

            if (player.currentVoiceChatIngameSettings != null && state.Set2D.HasValue)
            {
                player.currentVoiceChatIngameSettings.set2D = state.Set2D.Value;
            }
        }
    }
}
