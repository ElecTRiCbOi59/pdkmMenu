using MonoMod.RuntimeDetour;
using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace pdkmMenu.mods.worldMods
{
    internal static class SpawnDoor
    {
        // Spawns a big door by cloning an existing big door prefab from the scene.
        // Must be called on the server/host so the NetworkObject is spawned for clients.

        public static void dHelper()
        {
            var p = StartOfRound.Instance.localPlayerController;
            var pos = p.transform.position;
            var rot = p.transform.rotation;
            bool open = true;

            SpawnBigDoor(pos, rot, open);
        }

        public static void spawnAllThings()
        {
            //for (int i = 0; i < Unity.Netcode.NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs.Count; i++)
            //{
            //    var p = Unity.Netcode.NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs[i];
            //    UnityEngine.Debug.Log("NET: " + (p.Prefab != null ? p.Prefab.name : "<null>"));
            //}

            var list = Unity.Netcode.NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs; var p = StartOfRound.Instance.localPlayerController; var origin = p.transform.position; int cols = 10;
            float spacing = 4f; for (int i = 0; i < list.Count; i++)
            {
                var prefab = list[i].Prefab; 
                if (prefab == null) continue;
                //TestRoom
                if (prefab.name.ToLower().Contains("testroom")) continue;
                int row = i / cols;
                int col = i % cols;
                var pos = origin + new UnityEngine.Vector3(col * spacing, 0f, row * spacing);
                var go = UnityEngine.Object.Instantiate(prefab, pos, UnityEngine.Quaternion.identity);
                var net = go.GetComponent<Unity.Netcode.NetworkObject>(); 
                if (net != null) 
                    net.Spawn();
            }

        }
        public static TerminalAccessibleObject SpawnBigDoor(Vector3 position, Quaternion rotation, bool open = false, int? codeIndex = null, Transform parent = null)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
            {
                Debug.LogError("SpawnBigDoor: NetworkManager not listening.");
                return null;
            }

            if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost)
            {
                Debug.LogError("SpawnBigDoor: Must be called on server/host.");
                return null;
            }

            TerminalAccessibleObject template = UnityEngine.Object
                .FindObjectsOfType<TerminalAccessibleObject>()
                .FirstOrDefault(t => t != null && t.isBigDoor);

            if (template == null)
            {
                Debug.LogError("SpawnBigDoor: No existing big door found to clone.");
                return null;
            }

            GameObject instance = UnityEngine.Object.Instantiate(template.gameObject, position, rotation, parent);
            TerminalAccessibleObject door = instance.GetComponent<TerminalAccessibleObject>();
            if (door == null)
            {
                Debug.LogError("SpawnBigDoor: Spawned object missing TerminalAccessibleObject.");
                UnityEngine.Object.Destroy(instance);
                return null;
            }

            NetworkObject netObj = instance.GetComponent<NetworkObject>();
            if (netObj == null)
            {
                Debug.LogError("SpawnBigDoor: Spawned object missing NetworkObject.");
                UnityEngine.Object.Destroy(instance);
                return null;
            }

            // Spawn on network so clients receive it.
            try
            {
                netObj.Spawn(destroyWithScene: true);
            }
            catch
            {
                netObj.Spawn();
            }

            // Assign code after spawn so RPCs work.
            if (codeIndex.HasValue && RoundManager.Instance != null)
            {
                door.SetCodeTo(codeIndex.Value);
            }
            else if (RoundManager.Instance != null && RoundManager.Instance.possibleCodesForBigDoors != null && RoundManager.Instance.possibleCodesForBigDoors.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, RoundManager.Instance.possibleCodesForBigDoors.Length);
                door.SetCodeTo(randomIndex);
            }

            door.SetDoorOpen(open);
            door.SetDoorOpenClientRpc(open);

            return door;
        }
    }
}
