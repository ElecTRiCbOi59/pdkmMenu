using BepInEx.Configuration;
using pdkmMenu;
using UnityEngine;

public class AntiCheatConfig : ConfigCategory
{
    public ConfigEntry<bool> AntiCheat { get; private set; }
    public ConfigEntry<bool> AntiMessageSpoof { get; private set; }


    public AntiCheatConfig(ConfigFile config) : base(config) { }

    protected override void BindConfigs()
    {
        AntiCheat = Bind("AntiCheat", "AntiCheat", false, "Enable or Disable all anticheat features.");
        AntiMessageSpoof = Bind("AntiCheat", "AntiMessageSpoof", false, "Show warning for when other player spoofs a message");

    }


    public override void CheckKeys()
    {
        if (Plugin.WorldSettings.StartMatchHotKey.Value.IsDown()) WorldMods.StartMatch();
    }

    public bool IsEnabled(ConfigEntry<bool>entry)
    {
        return AntiCheat.Value && entry.Value;
    }
}
