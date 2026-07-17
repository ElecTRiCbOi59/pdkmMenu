using pdkmMenu;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameNetcodeStuff;
using Unity.Netcode;

public class HostMenu : MonoBehaviour
{
    private guiBase gui;

    private static int Quota = 300;
    private static int DeadLine = 3;
    private static bool ShowSpawnItems = false;
    private static int CombinedMobIndex = 0;
    private static int ObjectIndex = 0;
    private static Vector2 scrollPosition = Vector2.zero;
    private static string itemSearch = "";

    private void Start()
    {
        gui = gameObject.AddComponent<guiBase>();
        gui.MenuColor = gui.CustomBlue;
        gui.YPercentage = 0.0f;
    }

    public void update()
    {
        if (!NetworkManager.Singleton.IsServer && !CoHostHandler.Instance.IsCoHost) return;
        ulong tid = StartOfRound.Instance.localPlayerController.actualClientId;

        gui.CurrentColumn = 1;
        gui.ButtonIndex = 0;

        // --- WORLD SETTINGS ---
        Plugin.HostSettings.Credits.Value = gui.IntTextBox(Plugin.HostSettings.Credits.Value);
        gui.AddButton(LKey.SetCredits, () => Request($"credits|{Plugin.HostSettings.Credits.Value}"));

        Quota = gui.IntTextBox(Quota);
        DeadLine = gui.IntTextBox(DeadLine);
        gui.AddButton(LKey.SetQuotaDeadline, () => Request($"setquota|{Quota}|{DeadLine}"));

        gui.AddButton(LKey.RespawnPlayers, () => Request("revive"));

        // --- HOST SPECIALS (Godmode, Test Room, etc) ---
        gui.AddButton(LKey.FriendsGodmode, () => Request("godmode"), WorldMods.FriendGodMode);
        gui.AddButton(LKey.TestRoom, () => Request("testroom"), WorldMods.TestRoomEnabled);
        gui.AddButton(LKey.EjectAll, () => Request("eject"));
        gui.AddButton(LKey.ResetLevel, () => Request("resetlevel"));

        // --- SPAWNING ---
        AddObjectSpawnButton(tid);
        gui.AddButton(LKey.DespawnObjects, () => Request("despawnall"));
        gui.AddButton(LKey.DespawnProps, () => Request("despawnallprops"));
        gui.AddButton(LKey.DespawnMobs, () => SpawnHelper.DeSpawnAllEnemies());

        AddSpawnEnemy(tid);
        gui.AddButton(LKey.KillAllMobs, () => Request("killall"));

        gui.AddButton(LKey.SpawnAllEnemies, () => Request($"spawnallmobs:{tid}"));
        gui.AddButton(LKey.SpawnAllItems, () => Request($"spawnallitems:{tid}"));

        // --- ITEM MENU ---
        gui.AddButton(LKey.SpawnItemsMenu, () => ShowSpawnItems = !ShowSpawnItems, ShowSpawnItems);

        if (ShowSpawnItems) LoadItemList(tid);

        ResetGUIState();
    }

    private void AddSpawnEnemy(ulong tid)
    {
        var enemies = SpawnHelper.GetallEnemies();
        bool active = enemies.Count > 0;
        string label = active ? enemies[Mathf.Clamp(CombinedMobIndex, 0, enemies.Count - 1)].enemyType.enemyName : "No Enemies";

        CombinedMobIndex = gui.AddSlider(0, enemies.Count - 1, CombinedMobIndex, label);
        // Note: The original had "false" for nearPlayer, so we send "there" to the processor
        gui.AddButton(LKey.SpawnEnemy, () => Request($"spawnmob|{CombinedMobIndex}|there:{tid}"), active);
    }

    private void AddObjectSpawnButton(ulong tid)
    {
        var spawnable = RoundManager.Instance.currentLevel.spawnableMapObjects;
        bool active = spawnable != null && spawnable.Length > 0;
        string label = active ? spawnable[Mathf.Clamp(ObjectIndex, 0, spawnable.Length - 1)].prefabToSpawn.name : "No Objects";

        ObjectIndex = gui.AddSlider(0, active ? spawnable.Length - 1 : 0, ObjectIndex, label, active);
        gui.AddButton(LKey.SpawnMapObject, () => Request($"spawnobj|{ObjectIndex}:{tid}"), active);
    }

    private void LoadItemList(ulong tid)
    {
        gui.CurrentColumn = 2;
        gui.ButtonIndex = 0;
        itemSearch = gui.TextBox(itemSearch);

        var filtered = StartOfRound.Instance.allItemsList.itemsList
            .Where(i => i != null && (string.IsNullOrEmpty(itemSearch) || i.itemName.ToLower().Contains(itemSearch.ToLower())))
            .ToList();

        gui.BeginIconGrid(ref scrollPosition, 8, filtered.Count, Screen.height * 0.4f);
        foreach (var item in filtered)
        {
            gui.AddGridItem(item.itemIcon.texture, item.itemName, () => Request($"spawnitem|{item.itemName}|{item.itemId}:{tid}"));
        }
        gui.EndIconGrid();
    }

    private void Request(string cmd) => CoHostHandler.Instance.RequestAction(cmd);

    private void ResetGUIState()
    {
        gui.ButtonIndex = 0;
        gui.CurrentColumn = 0;
        gui.ContainterIndex_x = 0;
        gui.ContainterIndex_y = 0;
    }
}