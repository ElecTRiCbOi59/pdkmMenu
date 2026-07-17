using GameNetcodeStuff;
using HarmonyLib;
using pdkmMenu;
using System;
using Unity.Collections;
using Unity.Netcode;

[HarmonyPatch(typeof(HUDManager))]
internal class HUDManager_Patches
{
    [HarmonyPatch("__rpc_handler_2930587515")]
    [HarmonyPrefix]
    public static bool Start_Prefix(NetworkBehaviour target, ref FastBufferReader reader, __RpcParams rpcParams)
    {
        try
        {
            // Store original position
            int originalPosition = (int)reader.Position;

            // Read the original values
            bool hasMessage;
            reader.ReadValueSafe(out hasMessage, default(FastBufferWriter.ForPrimitives));

            string originalChatMessage = null;
            if (hasMessage)
            {
                reader.ReadValueSafe(out originalChatMessage, false);
            }

            int originalPlayerId;
            ByteUnpacker.ReadValueBitPacked(reader, out originalPlayerId);

            // Log original values
            //Plugin.Logger.LogInfo($"Original data - Message: '{originalChatMessage ?? "null"}', PlayerID: {originalPlayerId}");

            // Modify values as needed
            string modifiedMessage = originalChatMessage;
            int modifiedPlayerId = originalPlayerId; // Whatever player ID you want to use\
#pragma warning disable Harmony003
            ulong sender = rpcParams.Server.Receive.SenderClientId;
#pragma warning restore Harmony003
            if (Plugin.AntiCheatSettings.IsEnabled(Plugin.AntiCheatSettings.AntiMessageSpoof))
            {

                foreach (PlayerControllerB player in Plugin.GetRealPlayerScripts())
                {
                    if (StartOfRound.Instance.localPlayerController.actualClientId == sender)
                    {
                        break;
                    }
                    if (player.actualClientId == sender)
                    {
                        if (originalPlayerId != (int)player.actualClientId)
                        {
                            //<color =#0000FF></color>

                            string Preface = $"<color=#ff0000>WARNING\nSpoof attemped as:</color> <color=#198f00>{StartOfRound.Instance.allPlayerScripts[originalPlayerId].playerUsername}</color>";
                            LogToChat.SendServerMessage(Preface);
                            modifiedPlayerId = (int)player.playerClientId;
                        }
                    }
                }
            }
            

            // Create a new buffer with modified data
            FastBufferWriter writer = new FastBufferWriter(1024, Allocator.Temp);

            // Write the modified data in the same format
            writer.WriteValueSafe(hasMessage, default(FastBufferWriter.ForPrimitives));
            if (hasMessage)
            {
                writer.WriteValueSafe(modifiedMessage, false);
            }
            BytePacker.WriteValueBitPacked(writer, modifiedPlayerId);

            // Create a new reader from the writer's data
            writer.Seek(0);
            FastBufferReader newReader = new FastBufferReader(writer, Allocator.Temp);

            // Replace the original reader with our modified one
            reader = newReader;

            // Log the modification
            //Plugin.Logger.LogInfo($"Modified data - Message: '{modifiedMessage ?? "null"}', PlayerID: {modifiedPlayerId}");

            // Important: Don't dispose the writer or newReader here as they're still being used

            // Continue with the original method using our modified data
            return true;
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"Exception in Prefix Patch: {ex}");
            return true;
        }
    }
}