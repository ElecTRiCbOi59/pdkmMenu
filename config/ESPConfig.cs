using BepInEx.Configuration;
using pdkmMenu;
using UnityEngine;

public class ESPConfig : ConfigCategory
{
    public ConfigEntry<bool> ESP { get; private set; }
    public ConfigEntry<bool> Item_Label { get; private set; }
    public ConfigEntry<bool> Player_Label { get; private set; }
    public ConfigEntry<bool> Enemy_Label { get; private set; }
    public ConfigEntry<bool> Traps_Label { get; private set; }
    public ConfigEntry<bool> Doors_Label { get; private set; }
    public ConfigEntry<KeyboardShortcut> ESPHotKey { get; private set; }

    public ConfigEntry<bool> Item_Auras { get; private set; }
    public ConfigEntry<bool> Player_Auras { get; private set; }
    public ConfigEntry<bool> Enemy_Auras { get; private set; }
    public ConfigEntry<bool> Traps_Auras { get; private set; }

    public ConfigEntry<float> Player_AurasColorHue { get; private set; }
    public ConfigEntry<float> Enemy_AurasColorHue { get; private set; }
    public ConfigEntry<float> Item_AurasColorHue { get; private set; }
    public ConfigEntry<float> Traps_AurasColorHue { get; private set; }
    public ConfigEntry<float> AurasOpacity { get; private set; }

    public ESPConfig(ConfigFile config) : base(config) { }

    protected override void BindConfigs()
    {
        ESP = Bind("ESP", "ESP", false, "Enable ESP");
        ESPHotKey = Bind("ESP", "ESPHotKey", new KeyboardShortcut(KeyCode.None), "ESP Toggle Hotkey");

        Item_Label = Bind("ESP", "Item Label", true, "Show item labels in ESP");
        Player_Label = Bind("ESP", "Player Label", true, "Show player labels in ESP");
        Enemy_Label = Bind("ESP", "Enemy Label", true, "Show enemy labels in ESP");
        Traps_Label = Bind("ESP", "Traps Label", true, "Show traps labels in ESP");
        Doors_Label = Bind("ESP", "Doors Label", true, "Show doors labels in ESP");

        Item_Auras = Bind("ESP", "Item Auras", true, "Show item Auras");
        Player_Auras = Bind("ESP", "Player Auras", true, "Show player Auras");
        Enemy_Auras = Bind("ESP", "Enemy Auras", true, "Show enemy Auras");
        Traps_Auras = Bind("ESP", "Traps Auras", true, "Show traps Auras");

        AurasOpacity = Bind("ESP", "Auras Opacity", 0.5f, "Opacity for Auras");
        Player_AurasColorHue = Bind("ESP", "Player Auras Color Hue", 0.0f, "Hue for player Auras color");
        Enemy_AurasColorHue = Bind("ESP", "Enemy Auras Color Hue", 0.0f, "Hue for enemy Auras color");
        Item_AurasColorHue = Bind("ESP", "Item Auras Color Hue", 0.33f, "Hue for item Auras color");
        Traps_AurasColorHue = Bind("ESP", "Traps Auras Color Hue", 0.16f, "Hue for traps Auras color");
    }


    public override void CheckKeys()
    {
        if (Plugin.ESPSettings.ESPHotKey.Value.IsDown()) ToggleConfigEntry(ESP);
    }
}
