using BepInEx.Configuration;
using pdkmMenu;
using UnityEngine;

public class HostConfig : ConfigCategory
{

    public ConfigEntry<int> Credits { get; private set; }
    public ConfigEntry<KeyboardShortcut> SetCreditsHotKey { get; private set; }
    public ConfigEntry<KeyboardShortcut> ToggleFriendGodMode { get; private set; }


    public HostConfig(ConfigFile config) : base(config) { }

    protected override void BindConfigs()
    {
        Credits = Bind("Host", "Credits", 1000, "Amount of credits to set");
        SetCreditsHotKey = Bind("Host", "SetCreditsHotKey", new KeyboardShortcut(KeyCode.None), "ESP Toggle Hotkey");
        ToggleFriendGodMode = Bind("Host", "ToggleFriendGodMode", new KeyboardShortcut(KeyCode.None), "Toggle godmode for friends");


    }


    public override void CheckKeys()
    {
        if (Plugin.HostSettings.SetCreditsHotKey.Value.IsDown()) WorldMods.SetCredits(Credits.Value);
        if (Plugin.HostSettings.ToggleFriendGodMode.Value.IsDown()) WorldMods.GiveFriendsGodMode();
    }
}
