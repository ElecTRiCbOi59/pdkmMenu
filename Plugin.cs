using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace pdkmMenu;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    internal static Harmony HarmonyInstance = new(MyPluginInfo.PLUGIN_NAME);

    public static ConfigFile configFile;

    public static AntiCheatConfig AntiCheatSettings { get; private set; }
    public static ESPConfig ESPSettings { get; private set; }
    public static MenuConfig MenuSettings { get; private set; }
    public static SelfConfig SelfSettings { get; private set; }
    public static WorldConfig WorldSettings { get; private set; }
    public static HostConfig HostSettings { get; private set; }

    private float fpsDeltaTime;
    private GUIStyle fpsStyle;

    private static readonly List<PlayerControllerB> cachedPlayers = new();

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        configFile = Config;

        AntiCheatSettings = new AntiCheatConfig(Config);
        ESPSettings = new ESPConfig(Config);
        MenuSettings = new MenuConfig(Config);
        SelfSettings = new SelfConfig(Config);
        WorldSettings = new WorldConfig(Config);
        HostSettings = new HostConfig(Config);

        Localization.Initialize();

        HarmonyInstance.PatchAll();

        Logger.LogInfo(
            $"Plugin {MyPluginInfo.PLUGIN_GUID} {MyPluginInfo.PLUGIN_VERSION} loaded."
        );
    }

    private void Start()
    {
        GameObject mainMenuGameObject = new("MainMenu_Go");

        DontDestroyOnLoad(mainMenuGameObject);

        mainMenuGameObject.AddComponent<MainMenu>();

        Spectate.Initialize();
    }

    private void OnDestroy()
    {
        Logger.LogInfo("Unloading Plugin");

        HarmonyInstance.UnpatchSelf();
    }

    private void Update()
    {
        fpsDeltaTime += (Time.unscaledDeltaTime - fpsDeltaTime) * 0.1f;

        CheckKeys();
        UpdateMods();
        Spectate.Update();

        if (
            WorldSpacePathGuide.Instance == null
            && SelfSettings.EnablePathGuide.Value
        )
        {
            PathGuideBootstrapper.Initialize();
        }
    }

    private void OnGUI()
    {
        if (!MenuSettings.ShowFPS.Value)
        {
            return;
        }

        if (fpsStyle == null)
        {
            fpsStyle = new GUIStyle
            {
                alignment = TextAnchor.UpperRight,
                fontSize = 18,
            };

            fpsStyle.normal.textColor = Color.white;
        }

        float fps = 1f / fpsDeltaTime;
        string text = $"FPS: {fps:0}";

        Rect rect = new(Screen.width - 120, 10, 110, 25);

        GUI.Label(rect, text, fpsStyle);
    }

    private void CheckKeys()
    {
        ESPSettings.CheckKeys();
        MenuSettings.CheckKeys();
        SelfSettings.CheckKeys();
    }

    private void UpdateMods()
    {
        if (StartOfRound.Instance == null)
        {
            return;
        }

        if (GameNetworkManager.Instance == null)
        {
            return;
        }

        PlayerMods.UpdateMod();
        SelfMods.UpdateMod();
        WorldMods.UpdateMod();
    }

    public static List<PlayerControllerB> GetRealPlayerScripts()
    {
        cachedPlayers.Clear();

        if (StartOfRound.Instance == null)
        {
            return cachedPlayers;
        }

        PlayerControllerB[] scripts = StartOfRound.Instance.allPlayerScripts;

        for (int i = 0; i < scripts.Length; i++)
        {
            if (scripts[i].isPlayerDead || scripts[i].isPlayerControlled)
            {
                cachedPlayers.Add(scripts[i]);
            }
        }

        return cachedPlayers;
    }
}