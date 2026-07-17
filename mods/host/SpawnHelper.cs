using GameNetcodeStuff;
using pdkmMenu;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class SpawnHelper 
{
    private static List<SpawnableEnemyWithRarity> allEnemies = new List<SpawnableEnemyWithRarity>();
    private static List<GameObject> spawnedObjects = new List<GameObject>();

    public static List<SpawnableEnemyWithRarity> GetallEnemies()
    {
        allEnemies.Clear();

        // The QuickMenuManager holds a reference to a 'level' that contains every enemy for testing
        QuickMenuManager quickMenu = StartOfRound.Instance.localPlayerController.quickMenuManager;

        if (quickMenu != null && quickMenu.testAllEnemiesLevel != null)
        {
            SelectableLevel allEnemiesLevel = quickMenu.testAllEnemiesLevel;
            // Add standard indoor enemies
            if (allEnemiesLevel.Enemies != null)
                allEnemies.AddRange(allEnemiesLevel.Enemies);

            // Add daytime enemies
            if (allEnemiesLevel.DaytimeEnemies != null)
                allEnemies.AddRange(allEnemiesLevel.DaytimeEnemies);

            // Add outside enemies
            if (allEnemiesLevel.OutsideEnemies != null)
                allEnemies.AddRange(allEnemiesLevel.OutsideEnemies);
        }

        // If the list is STILL empty (e.g. menu hasn't initialized), 
        // fall back to the current level's list so the menu doesn't break.
        if (allEnemies.Count == 0 && StartOfRound.Instance.currentLevel != null)
        {
            allEnemies.AddRange(StartOfRound.Instance.currentLevel.Enemies);
            allEnemies.AddRange(StartOfRound.Instance.currentLevel.OutsideEnemies);
            allEnemies.AddRange(StartOfRound.Instance.currentLevel.DaytimeEnemies);
        }

        return allEnemies;
    }
    public static void SpawnEnemy(int index, PlayerControllerB player, bool safe = true)
    {
        if (allEnemies.Count == 0 || index >= allEnemies.Count)
        {
            Plugin.Logger.LogWarning("Invalid enemy index or no enemies available");
            return;
        }

        Vector3 spawnPosition = Vector3.zero;
        if (safe == false)
        {
            spawnPosition = player.transform.position;
        }
        else
        {
            spawnPosition = FindClosestSpawnPoint(player.transform.position);
        }

        if (spawnPosition == Vector3.zero)
        {
            Plugin.Logger.LogWarning("Failed to find valid spawn point");
            return;
        }

        GameObject enemy = UnityEngine.Object.Instantiate(
            allEnemies[index].enemyType.enemyPrefab,
            spawnPosition,
            Quaternion.identity
        );

        enemy.GetComponentInChildren<NetworkObject>().Spawn(true);
        RoundManager.Instance.SpawnedEnemies.Add(enemy.GetComponent<EnemyAI>());
    }
    public static void DeSpawnAllEnemies()
    {
        foreach (EnemyAI enemy in EnemyAI_Patches.activeEnemies)
        {
            if (enemy != null && enemy.gameObject != null)
            {
                NetworkObject netObj = enemy.gameObject.GetComponentInChildren<NetworkObject>();
                if (netObj != null)
                {
                    RoundManager.Instance.DespawnEnemyOnServer(netObj);
                }
            }
        }
        RoundManager.Instance.SpawnedEnemies.Clear();
    }
    private static Vector3 FindClosestSpawnPoint(Vector3 target)
    {
        float minDistance = Mathf.Infinity;
        GameObject closestNode = null;

        // Merge inside and outside AI nodes
        GameObject[] allNodes = RoundManager.Instance.outsideAINodes.Concat(RoundManager.Instance.insideAINodes).ToArray();

        foreach (GameObject navNode in allNodes)
        {
            float distance = Vector3.Distance(target, navNode.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestNode = navNode;
            }
        }

        return closestNode?.transform.position ?? Vector3.zero;
    }
    public static void SpawnObject(int index, Vector3 p)
    {
        // Instantiate the object
        GameObject spawnedObject = UnityEngine.Object.Instantiate<GameObject>(RoundManager.Instance.currentLevel.spawnableMapObjects[index].prefabToSpawn, p, Quaternion.identity, StartOfRound.Instance.propsContainer);

        // Keep track of the spawned object
        spawnedObjects.Add(spawnedObject);

        // Spawn the object (if it's a networked object)
        NetworkObject networkObject = spawnedObject.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn(false);
        }
    }
    public static void DespawnObjects()
    {
        // Iterate through the spawned objects and despawn them
        foreach (GameObject spawnedObject in spawnedObjects)
        {
            NetworkObject networkObject = spawnedObject.GetComponent<NetworkObject>();

            // Check if it's a networked object and despawn it
            if (networkObject != null)
            {
                networkObject.Despawn(true);
            }
            else
            {
                // If it's not a networked object, simply destroy it
                UnityEngine.Object.Destroy(spawnedObject);
            }
        }
        // Clear the list of spawned objects
        spawnedObjects.Clear();
    }
    public static void SpawnAllEnemiesInGrid(PlayerControllerB player, float columnSpacing = 5.0f, float rowSpacing = 5.0f, int enemiesPerRow = 5)
    {
        if (allEnemies == null || allEnemies.Count == 0)
        {
            Plugin.Logger.LogWarning("No enemies in the list to spawn.");
            return;
        }

        if (!RoundManager.Instance.NetworkManager.IsHost) return;

        // Start 5 meters in front of player
        Vector3 startPos = player.transform.position + (player.transform.forward * 5f);

        for (int i = 0; i < allEnemies.Count; i++)
        {
            int column = i % enemiesPerRow;
            int row = i / enemiesPerRow;

            // Calculate offset based on facing direction
            Vector3 offset = (player.transform.right * (column * columnSpacing)) +
                             (player.transform.forward * (row * rowSpacing));

            Vector3 spawnPosition = startPos + offset;
            GameObject enemyPrefab = allEnemies[i].enemyType.enemyPrefab;

            GameObject enemyInstance = UnityEngine.Object.Instantiate(
                enemyPrefab,
                spawnPosition,
                Quaternion.identity
            );

            EnemyAI ai = enemyInstance.GetComponent<EnemyAI>();
            NetworkObject netObj = enemyInstance.GetComponentInChildren<NetworkObject>();

            if (netObj != null) netObj.Spawn(true);

            if (ai != null)
            {
                RoundManager.Instance.SpawnedEnemies.Add(ai);
                ai.isOutside = !player.isInsideFactory;
            }
        }

        Plugin.Logger.LogInfo($"Spawned {allEnemies.Count} enemies in a grid.");
    }
    public static void SpawnAllItemsInGrid(PlayerControllerB player, float columnSpacing = 1.5f, float rowSpacing = 2.0f, int itemsPerRow = 10)
    {
        List<Item> list = StartOfRound.Instance.allItemsList.itemsList;
        if (list == null || list.Count == 0) return;

        if (!RoundManager.Instance.NetworkManager.IsHost) return;

        Vector3 startPos = player.transform.position + (player.transform.forward * 2f);

        for (int i = 0; i < list.Count; i++)
        {
            Item item = list[i];
            if (item == null || item.spawnPrefab == null) continue;

            int column = i % itemsPerRow;
            int row = i / itemsPerRow;

            // Use the new rowSpacing parameter
            Vector3 offset = (player.transform.right * (column * columnSpacing)) +
                             (player.transform.forward * (row * rowSpacing));

            Vector3 spawnPosition = startPos + offset;

            GameObject itemObj = UnityEngine.Object.Instantiate(
                item.spawnPrefab,
                spawnPosition,
                Quaternion.identity,
                StartOfRound.Instance.propsContainer
            );

            GrabbableObject grabbable = itemObj.GetComponent<GrabbableObject>();
            if (grabbable != null)
            {
                grabbable.fallTime = 0f;
                int scrapValue = (int)(UnityEngine.Random.Range(item.minValue + 25, item.maxValue + 35) * RoundManager.Instance.scrapValueMultiplier);
                grabbable.SetScrapValue(scrapValue);
            }

            NetworkObject netObj = itemObj.GetComponent<NetworkObject>();
            if (netObj != null) netObj.Spawn(false);
        }

        Plugin.Logger.LogInfo($"Spawned {list.Count} items in a grid.");
    }
}
