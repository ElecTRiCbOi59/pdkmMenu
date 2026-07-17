using BepInEx.Configuration;
using pdkmMenu;
using UnityEngine;

public class MenuConfig : ConfigCategory
{
    public ConfigEntry<KeyboardShortcut> OpenMenu { get; set; }
    public ConfigEntry<KeyboardShortcut> OpenSelfMenu { get; set; }
    public ConfigEntry<KeyboardShortcut> OpenPlayersMenu { get; set; }
    public ConfigEntry<KeyboardShortcut> OpenCamerasMenu { get; set; }
    public ConfigEntry<KeyboardShortcut> OpenworldMenu { get; set; }
    public ConfigEntry<KeyboardShortcut> OpenhostMenu { get; set; }
    public ConfigEntry<KeyboardShortcut> ToggleCursor { get; set; }
    public ConfigEntry<bool> AutoStart { get; set; }
    public ConfigEntry<bool> AutoStartOnline { get; set; }
    public ConfigEntry<bool> ShowFPS { get; set; }
    public ConfigEntry<string> Language { get; set; }


    public MenuConfig(ConfigFile config) : base(config) { }

    protected override void BindConfigs()
    {
        OpenMenu = Plugin.configFile.Bind("Menu", "OpenMenu", new KeyboardShortcut(KeyCode.M, KeyCode.LeftShift), "https://docs.unity3d.com/6000.0/Documentation/ScriptReference/KeyCode.html. Example M + LeftShift");
        ToggleCursor = Plugin.configFile.Bind("Menu", "ToggleCursor", new KeyboardShortcut(KeyCode.LeftAlt), "");
        OpenSelfMenu = Plugin.configFile.Bind("Menu", "OpenSelfMenu", new KeyboardShortcut(KeyCode.None), "");
        OpenPlayersMenu = Plugin.configFile.Bind("Menu", "OpenPlayersMenu", new KeyboardShortcut(KeyCode.None), "");
        OpenCamerasMenu = Plugin.configFile.Bind("Menu", "OpenCamerasMenu", new KeyboardShortcut(KeyCode.None), "");
        OpenworldMenu = Plugin.configFile.Bind("Menu", "OpenworldMenu", new KeyboardShortcut(KeyCode.None), "");
        OpenhostMenu = Plugin.configFile.Bind("Menu", "OpenhostMenu", new KeyboardShortcut(KeyCode.None), "");

        AutoStart = Plugin.configFile.Bind("Menu", "AutoStart", false, "True will enable auto start");
        ShowFPS = Plugin.configFile.Bind("Menu", "ShowFPS", false, "ShowFPS");
        AutoStartOnline = Plugin.configFile.Bind("Menu", "AutoStartOnline", true, "True auto start online. false auto start lan");
        Language = Plugin.configFile.Bind("Menu", "Language", "en", "Localization language file name (example: en, es, fr).");
    }
    public override void CheckKeys()
    {
        if (ToggleCursor.Value.IsDown()) ToggleCursorFunc();
    }
    private static bool ShowCursor = false;
    public static void ToggleCursorFunc()
    {
        ShowCursor = !ShowCursor;
        if (StartOfRound.Instance == null || StartOfRound.Instance.localPlayerController == null) return;
        QuickMenuManager quickMenuManager = StartOfRound.Instance.localPlayerController.quickMenuManager;
        if (quickMenuManager != null)
        {
            //QuickMenuManagerInstance.menuContainer.SetActive(true);
            if (ShowCursor)
            {
                Cursor.lockState = CursorLockMode.None;
                if (!StartOfRound.Instance.localPlayerUsingController)
                {
                    Cursor.visible = true;
                }

                quickMenuManager.isMenuOpen = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
                if (!StartOfRound.Instance.localPlayerUsingController)
                {
                    Cursor.visible = false;
                }

                quickMenuManager.isMenuOpen = false;
            }
        }
    }
}
