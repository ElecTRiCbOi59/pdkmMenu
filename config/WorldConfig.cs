using BepInEx.Configuration;
using pdkmMenu;
using UnityEngine;

public class WorldConfig : ConfigCategory
{
    public ConfigEntry<bool> CanBridgeFall { get; private set; }
    public ConfigEntry<bool> SpamMessage { get; private set; }
    public ConfigEntry<int> SpamMessageIndex { get; private set; }
    public ConfigEntry<int> SpamMessageRotation { get; private set; }


    public ConfigEntry<KeyboardShortcut> StartMatchHotKey { get; private set; }
    public ConfigEntry<KeyboardShortcut> KillAllEnemyHotKey { get; private set; }
    

    public WorldConfig(ConfigFile config) : base(config) { }

    protected override void BindConfigs()
    {
        StartMatchHotKey = Bind("World", "StartMatchHotKey", new KeyboardShortcut(KeyCode.None), "StartMatch HotKey");
        KillAllEnemyHotKey = Bind("World", "KillAllEnemyHotKey", new KeyboardShortcut(KeyCode.None), "KillAllEnemyHotKey");
        CanBridgeFall = Bind("World", "CanBridgeFall", true, "CanBridgeFall");

        SpamMessage = Bind("World", "SpamMessage", false, "CanBridgeFall");
        SpamMessageIndex = Bind("World", "SpamMessageIndex", 6, "emoji spam index");
        SpamMessageRotation = Bind("World", "SpamMessageRotation", 10, "emoji spam rotation step");
    }


    public override void CheckKeys()
    {
        if (Plugin.WorldSettings.StartMatchHotKey.Value.IsDown()) WorldMods.StartMatch();
        if (Plugin.WorldSettings.KillAllEnemyHotKey.Value.IsDown()) WorldMods.KillAllMobs();
    }
}
