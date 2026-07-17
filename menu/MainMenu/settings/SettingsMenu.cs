using pdkmMenu;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    private guiBase settingsMenuGUI;

    private void Start()
    {
        settingsMenuGUI = gameObject.AddComponent<guiBase>();
        settingsMenuGUI.MenuColor = settingsMenuGUI.CustomBlue;
        settingsMenuGUI.XPercentage = 0.07f;
        settingsMenuGUI.YPercentage = 0.0f;
    }

    public void update()
    {
        settingsMenuGUI.ButtonIndex = 0; // Reset index
        settingsMenuGUI.AddButton(LKey.AutoStart, () => { Plugin.MenuSettings.AutoStart.Value = !Plugin.MenuSettings.AutoStart.Value; }, Plugin.MenuSettings.AutoStart.Value);
        settingsMenuGUI.AddButton(LKey.AutoStartOnline, () => { Plugin.MenuSettings.AutoStartOnline.Value = !Plugin.MenuSettings.AutoStartOnline.Value; }, Plugin.MenuSettings.AutoStartOnline.Value);
    }
}