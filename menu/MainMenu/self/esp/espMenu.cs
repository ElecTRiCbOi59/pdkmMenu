using pdkmMenu;
using BepInEx.Configuration;
using System;
using UnityEngine;

public class EspMenu : MonoBehaviour
{
    private guiBase espMenuGUI;

    private void Start()
    {
        espMenuGUI = gameObject.AddComponent<guiBase>();
        espMenuGUI.MenuColor = espMenuGUI.CustomBlue;
        espMenuGUI.YPercentage = 0.0f;
    }

    private void CheckKeys()
    {
        if (Plugin.ESPSettings.ESPHotKey.Value.IsDown()) UpdateEsp();
    }

    private void UpdateEsp()
    {
        Plugin.ESPSettings.ESP.Value = !Plugin.ESPSettings.ESP.Value;
    }

    public void update()
    {
        CheckKeys();
        var esp = Plugin.ESPSettings;
        var self = Plugin.SelfSettings;

        espMenuGUI.CurrentColumn = 3;
        espMenuGUI.ButtonIndex = 0;

        espMenuGUI.AddButton(LKey.MasterESP, () => esp.ToggleConfigEntry(esp.ESP), esp.ESP.Value);

        espMenuGUI.AddButton(LKey.Labels, () => { }, false);
        espMenuGUI.AddButton(LKey.ItemLabel, () => esp.ToggleConfigEntry(esp.Item_Label), esp.Item_Label.Value);
        espMenuGUI.AddButton(LKey.PlayerLabel, () => esp.ToggleConfigEntry(esp.Player_Label), esp.Player_Label.Value);
        espMenuGUI.AddButton(LKey.EnemyLabel, () => esp.ToggleConfigEntry(esp.Enemy_Label), esp.Enemy_Label.Value);
        espMenuGUI.AddButton(LKey.TrapsLabel, () => esp.ToggleConfigEntry(esp.Traps_Label), esp.Traps_Label.Value);
        espMenuGUI.AddButton(LKey.DoorsLabel, () => esp.ToggleConfigEntry(esp.Doors_Label), esp.Doors_Label.Value);

        espMenuGUI.AddButton(LKey.Auras, () => { }, false);
        espMenuGUI.AddButton(LKey.ItemAuras, () => esp.ToggleConfigEntry(esp.Item_Auras), esp.Item_Auras.Value);
        espMenuGUI.AddButton(LKey.PlayerAuras, () => esp.ToggleConfigEntry(esp.Player_Auras), esp.Player_Auras.Value);
        espMenuGUI.AddButton(LKey.EnemyAuras, () => esp.ToggleConfigEntry(esp.Enemy_Auras), esp.Enemy_Auras.Value);
        espMenuGUI.AddButton(LKey.TrapsAuras, () => esp.ToggleConfigEntry(esp.Traps_Auras), esp.Traps_Auras.Value);

        espMenuGUI.AddButton(LKey.Aurasettings, () => { }, false);

        // Sliders automatically use CurrentColumn for their X position now
        esp.AurasOpacity.Value = espMenuGUI.AddSlider(0f, 1f, esp.AurasOpacity.Value, $"Opacity: {esp.AurasOpacity.Value:F2}");
        esp.Player_AurasColorHue.Value = espMenuGUI.AddSlider(0f, 1f, esp.Player_AurasColorHue.Value, $"Player Hue: {esp.Player_AurasColorHue.Value:F2}");
        esp.Enemy_AurasColorHue.Value = espMenuGUI.AddSlider(0f, 1f, esp.Enemy_AurasColorHue.Value, $"Enemy Hue: {esp.Enemy_AurasColorHue.Value:F2}");
        esp.Item_AurasColorHue.Value = espMenuGUI.AddSlider(0f, 1f, esp.Item_AurasColorHue.Value, $"Item Hue: {esp.Item_AurasColorHue.Value:F2}");
        esp.Traps_AurasColorHue.Value = espMenuGUI.AddSlider(0f, 1f, esp.Traps_AurasColorHue.Value, $"Traps Hue: {esp.Traps_AurasColorHue.Value:F2}");

        // Path Guide Settings
        espMenuGUI.AddButton(LKey.PathGuide, () => { }, false);
        espMenuGUI.AddButton(LKey.EnablePathGuide, () => self.ToggleConfigEntry(self.EnablePathGuide), self.EnablePathGuide.Value);

        if (self.EnablePathGuide.Value)
        {
            espMenuGUI.AddButton(LKey.MainDoorPath, () => self.ToggleConfigEntry(self.DisplayMainDoorPath), self.DisplayMainDoorPath.Value);
            espMenuGUI.AddButton(LKey.SideDoorsPath, () => self.ToggleConfigEntry(self.DisplaySideDoorsPath), self.DisplaySideDoorsPath.Value);
            espMenuGUI.AddButton(LKey.ShipPath, () => self.ToggleConfigEntry(self.DisplayShipPath), self.DisplayShipPath.Value);
        }

        // Reset state for other UI elements
        espMenuGUI.ButtonIndex = 0;
        espMenuGUI.CurrentColumn = 0;
    }
}