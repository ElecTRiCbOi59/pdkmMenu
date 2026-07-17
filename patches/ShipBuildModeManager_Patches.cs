//using System;
//using System.Collections.Generic;
//using HarmonyLib;
//using Unity.Collections;
//using Unity.Netcode;
//using UnityEngine;

//[HarmonyPatch(typeof(ShipBuildModeManager))]
//internal class ShipBuildModeManager_Patches
//{
//    private static readonly Dictionary<ulong, DateTime> lastRequestTime = new Dictionary<ulong, DateTime>();
//    private static readonly TimeSpan cooldown = TimeSpan.FromSeconds(1); // Adjust the cooldown period as needed

//    [HarmonyPatch("__rpc_handler_861494715")]
//    [HarmonyPrefix]
//    public static bool Start_Prefix(NetworkBehaviour target, ref FastBufferReader reader, __RpcParams rpcParams)
//    {
//        ulong senderClientId = rpcParams.Server.Receive.SenderClientId;
//        DateTime currentTime = DateTime.UtcNow;

//        // Check if the client has made a recent request
//        if (lastRequestTime.TryGetValue(senderClientId, out DateTime lastTime))
//        {
//            if (currentTime - lastTime < cooldown)
//            {
//                int originalPosition = (int)reader.Position;
//                Vector3 newPosition;
//                reader.ReadValueSafe(out newPosition);
//                Vector3 newRotation;
//                reader.ReadValueSafe(out newRotation);
//                NetworkObjectReference objectRef;
//                reader.ReadValueSafe<NetworkObjectReference>(out objectRef, default(FastBufferWriter.ForNetworkSerializable));
//                int playerWhoMoved;
//                ByteUnpacker.ReadValueBitPacked(reader, out playerWhoMoved);

//                // Reject the request if it's too soon
//                //need to modify the data so that the orginal position is set as the new position so the rate limited client is not desynced. 
//                // Create a new buffer with modified data
//                FastBufferWriter writer = new FastBufferWriter(1024, Allocator.Temp);
//                writer.WriteValueSafe(newPosition);
//                writer.WriteValueSafe(newRotation);
//                writer.WriteValueSafe(objectRef, default(FastBufferWriter.ForNetworkSerializable));
//                BytePacker.WriteValueBitPacked(writer, playerWhoMoved);

//                return false;
//            }
//        }

//        // Update the last request time
//        lastRequestTime[senderClientId] = currentTime;
//        return true;
//    }
//}