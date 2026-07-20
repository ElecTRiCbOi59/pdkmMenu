using pdkmMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class HostMenu : MonoBehaviour
{
    private guiBase gui;

    private static int Quota = 300;
    private static int DeadLine = 3;
    private static bool ShowSpawnItems;
    private static int CombinedMobIndex;
    private static int ObjectIndex;
    private static Vector2 itemScrollPosition = Vector2.zero;
    private static string itemSearch = "";
    private static Item selectedItem;
    private static GUIStyle itemScrollbarStyle;
    private static GUIStyle itemScrollbarThumbStyle;

    private void Start()
    {
        gui = gameObject.AddComponent<guiBase>();
        gui.MenuColor = gui.CustomBlue;
        gui.YPercentage = 0.0f;
    }

    public void update()
    {
        if (!NetworkManager.Singleton.IsServer && !CoHostHandler.Instance.IsCoHost)
        {
            return;
        }

        ulong tid = StartOfRound.Instance.localPlayerController.actualClientId;

        if (ShowSpawnItems)
        {
            DrawItemSpawner(tid);
            ResetGUIState();
            return;
        }

        DrawHostControls(tid);
        ResetGUIState();
    }

    private void DrawHostControls(ulong tid)
    {
        gui.CurrentColumn = 1;
        gui.ButtonIndex = 0;

        Plugin.HostSettings.Credits.Value = gui.IntTextBox(Plugin.HostSettings.Credits.Value);
        gui.AddButton(LKey.SetCredits, () => Request($"credits|{Plugin.HostSettings.Credits.Value}"));

        Quota = gui.IntTextBox(Quota);
        DeadLine = gui.IntTextBox(DeadLine);
        gui.AddButton(LKey.SetQuotaDeadline, () => Request($"setquota|{Quota}|{DeadLine}"));

        gui.AddButton(LKey.RespawnPlayers, () => Request("revive"));
        gui.AddButton(LKey.FriendsGodmode, () => Request("godmode"), WorldMods.FriendGodMode);
        gui.AddButton(LKey.TestRoom, () => Request("testroom"), WorldMods.TestRoomEnabled);
        gui.AddButton(LKey.EjectAll, () => Request("eject"));
        gui.AddButton(LKey.ResetLevel, () => Request("resetlevel"));

        AddObjectSpawnButton(tid);
        gui.AddButton(LKey.DespawnObjects, () => Request("despawnall"));
        gui.AddButton(LKey.DespawnProps, () => Request("despawnallprops"));
        gui.AddButton(LKey.DespawnMobs, () => SpawnHelper.DeSpawnAllEnemies());

        AddSpawnEnemy(tid);
        gui.AddButton(LKey.KillAllMobs, () => Request("killall"));
        gui.AddButton(LKey.SpawnAllEnemies, () => Request($"spawnallmobs:{tid}"));
        gui.AddButton(LKey.SpawnAllItems, () => Request($"spawnallitems:{tid}"));

        gui.AddButton(LKey.SpawnItemsMenu, OpenItemSpawner);
    }

    private void OpenItemSpawner()
    {
        ShowSpawnItems = true;
        itemScrollPosition = Vector2.zero;
    }

    public void ResetItemSpawner()
    {
        ShowSpawnItems = false;
        selectedItem = null;
        itemSearch = "";
        itemScrollPosition = Vector2.zero;
        GUI.FocusControl(null);
    }

    private void DrawItemSpawner(ulong tid)
    {
        MenuTheme.EnsureInitialised();

        Rect content = guiBase.ContentRect;
        const float topRowHeight = 40f;
        const float searchHeight = 40f;
        const float actionHeight = 50f;
        const float gap = 10f;
        const float spawnButtonWidth = 210f;

        Rect backButtonRect = new(0f, 0f, 170f, topRowHeight);
        if (GUI.Button(backButtonRect, "◀  Host Controls", MenuTheme.ButtonStyle))
        {
            ResetItemSpawner();
            return;
        }

        GUI.Label(
            new Rect(186f, 4f, content.width - 186f, 30f),
            "ITEM SPAWNER",
            MenuTheme.SectionHeaderStyle
        );

        float searchY = topRowHeight + gap;
        GUI.Label(
            new Rect(2f, searchY, content.width, 18f),
            "SEARCH ITEMS",
            MenuTheme.SectionHeaderStyle
        );

        Rect searchRect = new(0f, searchY + 22f, content.width, searchHeight);
        GUI.SetNextControlName("ItemSpawnerSearch");
        itemSearch = GUI.TextField(
            searchRect,
            itemSearch,
            50,
            MenuTheme.TextFieldStyle
        );

        if (string.IsNullOrEmpty(itemSearch) && GUI.GetNameOfFocusedControl() != "ItemSpawnerSearch")
        {
            GUI.Label(
                new Rect(searchRect.x + 12f, searchRect.y, searchRect.width - 24f, searchRect.height),
                "Search by item name...",
                MenuTheme.MutedLabelStyle
            );
        }

        float actionY = searchRect.yMax + gap;
        Rect actionRect = new(0f, actionY, content.width, actionHeight);
        MenuTheme.DrawPanel(actionRect, MenuTheme.Surface);

        string selectedLabel = selectedItem == null
            ? "No item selected"
            : $"Selected: {selectedItem.itemName}";

        GUI.Label(
            new Rect(actionRect.x + 14f, actionRect.y + 13f, actionRect.width - spawnButtonWidth - 42f, 24f),
            selectedLabel,
            selectedItem == null ? MenuTheme.MutedLabelStyle : MenuTheme.LabelStyle
        );

        Rect spawnButtonRect = new(
            actionRect.xMax - spawnButtonWidth - 8f,
            actionRect.y + 8f,
            spawnButtonWidth,
            actionRect.height - 16f
        );

        bool canSpawn = selectedItem != null;
        string spawnLabel = canSpawn
            ? "Spawn Item"
            : "Select an item to spawn";

        GUI.enabled = canSpawn;
        if (GUI.Button(spawnButtonRect, spawnLabel, canSpawn ? MenuTheme.ActiveButtonStyle : MenuTheme.ButtonStyle))
        {
            SpawnSelectedItem(tid);
        }
        GUI.enabled = true;

        float listY = actionRect.yMax + gap;
        Rect listRect = new(0f, listY, content.width, Mathf.Max(40f, content.height - listY));

        List<Item> items = GetFilteredItems();
        DrawItemList(listRect, items);
    }

    private List<Item> GetFilteredItems()
    {
        List<Item> allItems = StartOfRound.Instance.allItemsList.itemsList
            .Where(item => item != null && item.spawnPrefab != null)
            .OrderBy(item => item.itemName)
            .ThenBy(item => item.itemId)
            .ToList();

        if (selectedItem != null && !allItems.Contains(selectedItem))
        {
            selectedItem = null;
        }

        string search = itemSearch.Trim();

        return allItems
            .Where(item => string.IsNullOrEmpty(search)
                || item.itemName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
            .ToList();
    }

    private void DrawItemList(Rect listRect, IReadOnlyList<Item> items)
    {
        const int columns = 3;
        const float itemHeight = 44f;
        const float horizontalGap = 8f;
        const float verticalGap = 8f;
        const float scrollbarWidth = 8f;

        int rowCount = Mathf.CeilToInt(items.Count / (float)columns);
        float itemStride = itemHeight + verticalGap;
        float contentHeight = Mathf.Max(listRect.height, rowCount * itemStride);
        float contentWidth = Mathf.Max(0f, listRect.width - scrollbarWidth - 10f);
        float itemWidth = Mathf.Max(
            0f,
            (contentWidth - (horizontalGap * (columns - 1))) / columns
        );

        EnsureItemScrollbarStyles();

        GUIStyle previousScrollbarStyle = GUI.skin.verticalScrollbar;
        GUIStyle previousScrollbarThumbStyle = GUI.skin.verticalScrollbarThumb;
        GUI.skin.verticalScrollbar = itemScrollbarStyle;
        GUI.skin.verticalScrollbarThumb = itemScrollbarThumbStyle;

        itemScrollPosition = GUI.BeginScrollView(
            listRect,
            itemScrollPosition,
            new Rect(0f, 0f, contentWidth, contentHeight),
            false,
            contentHeight > listRect.height,
            GUIStyle.none,
            itemScrollbarStyle,
            MenuTheme.ScrollViewBackgroundStyle
        );

        if (items.Count == 0)
        {
            GUI.Label(
                new Rect(0f, 10f, contentWidth, 30f),
                "No items match your search.",
                MenuTheme.MutedLabelStyle
            );
        }

        for (int index = 0; index < items.Count; index++)
        {
            Item item = items[index];
            int row = index / columns;
            int column = index % columns;
            bool isSelected = selectedItem == item;

            Rect itemRect = new(
                column * (itemWidth + horizontalGap),
                row * itemStride,
                itemWidth,
                itemHeight
            );

            GUIStyle style = isSelected
                ? MenuTheme.ActiveButtonStyle
                : MenuTheme.ButtonStyle;

            GUIContent content = new(item.itemName, item.itemName);
            if (GUI.Button(itemRect, content, style))
            {
                selectedItem = item;
            }
        }

        GUI.EndScrollView();

        GUI.skin.verticalScrollbar = previousScrollbarStyle;
        GUI.skin.verticalScrollbarThumb = previousScrollbarThumbStyle;
    }

    private static void EnsureItemScrollbarStyles()
    {
        if (itemScrollbarStyle != null && itemScrollbarThumbStyle != null)
        {
            return;
        }

        itemScrollbarStyle = new GUIStyle(MenuTheme.VerticalScrollbarStyle)
        {
            fixedWidth = 8f,
            margin = new RectOffset(5, 0, 3, 3)
        };

        itemScrollbarThumbStyle = new GUIStyle(GUI.skin.verticalScrollbarThumb)
        {
            fixedWidth = 8f,
            fixedHeight = 48f,
            border = new RectOffset(4, 4, 4, 4)
        };
    }

    private void AddSpawnEnemy(ulong tid)
    {
        var enemies = SpawnHelper.GetallEnemies();
        bool active = enemies.Count > 0;
        string label = active
            ? enemies[Mathf.Clamp(CombinedMobIndex, 0, enemies.Count - 1)].enemyType.enemyName
            : "No Enemies";

        CombinedMobIndex = gui.AddSlider(0, enemies.Count - 1, CombinedMobIndex, label);
        gui.AddButton(LKey.SpawnEnemy, () => Request($"spawnmob|{CombinedMobIndex}|there:{tid}"), active);
    }

    private void AddObjectSpawnButton(ulong tid)
    {
        var spawnable = RoundManager.Instance.currentLevel.spawnableMapObjects;
        bool active = spawnable != null && spawnable.Length > 0;
        string label = active
            ? spawnable[Mathf.Clamp(ObjectIndex, 0, spawnable.Length - 1)].prefabToSpawn.name
            : "No Objects";

        ObjectIndex = gui.AddSlider(0, active ? spawnable.Length - 1 : 0, ObjectIndex, label, active);
        gui.AddButton(LKey.SpawnMapObject, () => Request($"spawnobj|{ObjectIndex}:{tid}"), active);
    }

    private void SpawnSelectedItem(ulong tid)
    {
        if (selectedItem == null)
        {
            return;
        }

        Request($"spawnitem|{selectedItem.itemName}|{selectedItem.itemId}:{tid}");
        LogToChat.LogLocal($"Spawned <color=#ffbf00>{selectedItem.itemName}</color>");
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