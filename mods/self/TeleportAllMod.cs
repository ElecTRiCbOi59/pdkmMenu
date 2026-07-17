using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace pdkmMenu
{
    public static class TeleportAllMod
    {
        public static IEnumerator TeleportAllGrabbableObjectsToPlayer()
        {
            PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;

            foreach (GrabbableObject grabbableObject in GrabbableObject_Patches.activeGrabbables)
            {
                if (grabbableObject == null) continue; 
                if (grabbableObject.GetComponent<PhysicsProp>() != null && (grabbableObject.GetComponent<PhysicsProp>().isHeld || grabbableObject.isHeldByEnemy))
                {
                    continue; // Skip if held by player or enemy
                }
                NetworkObject networkObject = grabbableObject.GetComponent<NetworkObject>();
                if (networkObject == null) continue; 
                NetworkObjectReference networkObjectRef = new NetworkObjectReference(networkObject);
                
                // Drop any item currently held by the player
                if (localPlayer.isHoldingObject)
                {
                    localPlayer.DropAllHeldItemsAndSync(localPlayer.transform.position, localPlayer.localItemHolder.position, localPlayer.localItemHolder.eulerAngles, localPlayer.playerEye.position, localPlayer.playerEye.eulerAngles);
                    yield return new WaitUntil(() => !localPlayer.isHoldingObject); // Wait until local player is no longer holding an item
                }
                
                ReflectionUtil.ReflectSetField<PlayerControllerB>(localPlayer, "currentlyGrabbingObject", grabbableObject);
                ReflectionUtil.ReflectMethod<PlayerControllerB>(localPlayer, "GrabObjectServerRpc", new object[] { networkObjectRef });

                // Start the GrabObject coroutine and wait for it to complete
                yield return localPlayer.StartCoroutine((IEnumerator)ReflectionUtil.ReflectMethod<PlayerControllerB>(localPlayer, "GrabObject"));

                // After GrabObject coroutine completes, the item should be grabbed.
                // We still need to wait for server sync on currentlyHeldObjectServer
                yield return new WaitUntil(() => localPlayer.currentlyHeldObjectServer == grabbableObject);
                
                // Drop the grabbed grabbableObject
                if (localPlayer.isHoldingObject) // Check if actually holding before attempting to drop
                {
                    localPlayer.DropAllHeldItemsAndSync(localPlayer.transform.position, localPlayer.localItemHolder.position, localPlayer.localItemHolder.eulerAngles, localPlayer.playerEye.position, localPlayer.playerEye.eulerAngles);
                    yield return new WaitUntil(() => !localPlayer.isHoldingObject); // Wait until local player is no longer holding an item
                }
            }
        }
    }
}
