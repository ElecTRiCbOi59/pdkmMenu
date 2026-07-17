using pdkmMenu;
using UnityEngine;

internal class WorldMenu : MonoBehaviour
{
    private guiBase worldMenuGUI;
    private WorldConfig worldConfig;

    private void Start()
    {
        worldMenuGUI = gameObject.AddComponent<guiBase>();
        worldMenuGUI.MenuColor = worldMenuGUI.CustomBlue;
        worldMenuGUI.YPercentage = 0.0f;
        worldConfig = Plugin.WorldSettings;
    }

    private static int planetIndex = 0;
    private static string message = "Hi";
    private static int audio_index = 0;

    private static int ItemCost = 0;

    public void update()
    {
        worldMenuGUI.CurrentColumn = 1;
        worldMenuGUI.ButtonIndex = 0;
        if (StartOfRound.Instance == null) return;

        message = worldMenuGUI.TextBox(message);
        worldMenuGUI.AddButton(LKey.SignalMessage, () => { WorldMods.SendSignalMessage(message); });
        worldMenuGUI.AddButton(LKey.ServerMessage, () => { LogToChat.SendServerMessage(message); });
        worldMenuGUI.AddButton(LKey.Emoji, () => { worldConfig.ToggleConfigEntry(worldConfig.SpamMessage); }, worldConfig.SpamMessage.Value);
        worldConfig.SpamMessageRotation.Value = worldMenuGUI.AddSlider(0, 360, worldConfig.SpamMessageRotation.Value, worldConfig.SpamMessageRotation.Value.ToString());
        worldConfig.SpamMessageIndex.Value = worldMenuGUI.AddSlider(0, 17, worldConfig.SpamMessageIndex.Value, worldConfig.SpamMessageIndex.Value.ToString());

        worldMenuGUI.AddButton(LKey.ForceStart, () => { WorldMods.StartMatch(); });
        worldMenuGUI.AddButton(LKey.ForceEnd, () => { WorldMods.EndMatch(); });
        ItemCost = (int)worldMenuGUI.AddSlider(0, 100, ItemCost, ItemCost.ToString());
        worldMenuGUI.AddButton(LKey.SetPrice, () => { WorldMods.SetSale(ItemCost); });
        PlanetButton();

        worldMenuGUI.CurrentColumn = 2;
        worldMenuGUI.ButtonIndex = 0;
        worldMenuGUI.AddButton(LKey.UnlockAll, () => { WorldMods.UnlockAll(); });
        worldMenuGUI.AddButton(LKey.StoreAll, () => { WorldMods.StoreAll(); });
        worldMenuGUI.AddButton(LKey.Organize, () => { WorldMods.ReadPosFile(); });
        worldMenuGUI.AddButton(LKey.SaveLayout, () => { WorldMods.SaveTransformsToFile(); });
        worldMenuGUI.AddButton(LKey.MaxBestiary, () => { WorldMods.MaxBestiary(); });
        worldMenuGUI.AddButton(LKey.FactoryLights, () => { WorldMods.FlickerTog = !WorldMods.FlickerTog; }, WorldMods.FlickerTog);

        worldMenuGUI.CurrentColumn = 3;
        worldMenuGUI.ButtonIndex = 0;
        worldMenuGUI.AddButton(LKey.ToggleItems, () => { WorldMods.ToggleItems(); }, WorldMods.TogItems);
        WorldMods.interval = worldMenuGUI.AddSlider(0, 1, WorldMods.interval, WorldMods.interval.ToString());
        worldMenuGUI.AddButton(LKey.ShotGuns, () => { WorldMods.ToggleShotGuns(); }, WorldMods.ShotGunToggle);

        SoundButton();
        worldMenuGUI.AddButton(LKey.KillAllMobs, () => { WorldMods.KillAllMobs(); });
        worldMenuGUI.AddButton(LKey.DespawnMobs, () => { SpawnHelper.DeSpawnAllEnemies(); });
        worldMenuGUI.AddButton(LKey.FixSteam, () => { WorldMods.FixSteamPipes(); });
        worldMenuGUI.AddButton(LKey.ToggleDoors, () => { WorldMods.ToggleDoors(); }, WorldMods.doorToggle);
        worldMenuGUI.AddButton(LKey.DisableTurrets, () => { WorldMods.ToggleTurrets(); }, !WorldMods.turretToggle);
        worldMenuGUI.AddButton(LKey.ExplodeMine, () => { WorldMods.ExplodeAllMines(); });
        worldMenuGUI.AddButton(LKey.UnlockAllDoors, () => { WorldMods.UnlockAllDoors(); });
        worldMenuGUI.AddButton(LKey.FactoryLights, () => { WorldMods.ToggelFactoryLights(); }, WorldMods.FactoryLights);
        worldMenuGUI.AddButton(LKey.CanBridgeFall, () => { worldConfig.ToggleConfigEntry(worldConfig.CanBridgeFall); }, worldConfig.CanBridgeFall.Value);

        //if (SoundManager.Instance != null && SoundManager.Instance.currentLevelAmbience != null)
        //{
        //    string label = "";
        //    label = SoundManager.Instance.currentLevelAmbience.insideAmbience[insideIndex0].GetName();
        //    insideIndex0 = worldMenuGUI.AddSlider(0, SoundManager.Instance.currentLevelAmbience.insideAmbience.Length-1, insideIndex0, label);
        //    worldMenuGUI.AddButton("insideAmbience", () => SoundManager.Instance.PlayAmbienceClipServerRpc(0, insideIndex0, 1.0f, false));

        //    label = SoundManager.Instance.currentLevelAmbience.insideAmbienceInsanity[insideIndex1].audioClip.GetName();
        //    insideIndex1 = worldMenuGUI.AddSlider(0, SoundManager.Instance.currentLevelAmbience.insideAmbienceInsanity.Length-1, insideIndex1, label);
        //    worldMenuGUI.AddButton("insideAmbienceInsanity", () => SoundManager.Instance.PlayAmbienceClipServerRpc(0, insideIndex1, 1.0f, true));
        //    //
        //    label = SoundManager.Instance.currentLevelAmbience.outsideAmbience[outsideIndex0].GetName();
        //    outsideIndex0 = worldMenuGUI.AddSlider(0, SoundManager.Instance.currentLevelAmbience.outsideAmbience.Length - 1, outsideIndex0, label);
        //    worldMenuGUI.AddButton("outsideAmbience", () => SoundManager.Instance.PlayAmbienceClipServerRpc(1, outsideIndex0, 1.0f, false));

        //    label = SoundManager.Instance.currentLevelAmbience.outsideAmbienceInsanity[outsideIndex1].audioClip.GetName();
        //    outsideIndex1 = worldMenuGUI.AddSlider(0, SoundManager.Instance.currentLevelAmbience.outsideAmbienceInsanity.Length - 1, outsideIndex1, label);
        //    worldMenuGUI.AddButton("outsideAmbienceInsanity", () => SoundManager.Instance.PlayAmbienceClipServerRpc(1, outsideIndex1, 1.0f, true));
        //    //
        //    label = SoundManager.Instance.currentLevelAmbience.shipAmbience[shipIndex0].GetName();
        //    shipIndex0 = worldMenuGUI.AddSlider(0, SoundManager.Instance.currentLevelAmbience.shipAmbience.Length - 1, shipIndex0, label);
        //    worldMenuGUI.AddButton("shipAmbience", () => SoundManager.Instance.PlayAmbienceClipServerRpc(2, shipIndex0, 1.0f, false));

        //    label = SoundManager.Instance.currentLevelAmbience.shipAmbienceInsanity[shipIndex1].audioClip.GetName();
        //    shipIndex1 = worldMenuGUI.AddSlider(0, SoundManager.Instance.currentLevelAmbience.shipAmbienceInsanity.Length - 1, shipIndex1, label);
        //    worldMenuGUI.AddButton("shipAmbienceInsanity", () => SoundManager.Instance.PlayAmbienceClipServerRpc(2, shipIndex1, 1.0f, true));
        //}

        //0 SoundManager.Instance.currentLevelAmbience.insideAmbienceInsanity.Length;
        //0 SoundManager.Instance.currentLevelAmbience.insideAmbience.Length;

        //1 SoundManager.Instance.currentLevelAmbience.outsideAmbienceInsanity.Length;
        //1 SoundManager.Instance.currentLevelAmbience.outsideAmbience.Length;

        //2 SoundManager.Instance.currentLevelAmbience.shipAmbienceInsanity.Length;
        //2 SoundManager.Instance.currentLevelAmbience.shipAmbience.Length;

        //3
        //3

        //SoundManager.Instance.currentLevelAmbience.insideAmbience.Length;
        //SoundManager.Instance.currentLevelAmbience.insideAmbienceInsanity.Length;

        //worldMenuGUI.AddButton("HangerDoor", () => { StartOfRound.Instance.shipDoorsAnimator.SetBool("Closed", !StartOfRound.Instance.shipDoorsAnimator.GetBool("Closed")); }, StartOfRound.Instance.shipDoorsAnimator.GetBool("Closed") ? true : false);

        //RoundManager.Instance.playersManager.shipDoorsAnimator.SetBool("Closed", true);
        //IngamePlayerSettings.Instance.SetLaunchInOnlineMode(true)
        //UngamePlayerSettings.Instance.settings.startInOnlineMode

        worldMenuGUI.CurrentColumn = 0;
        worldMenuGUI.ButtonIndex = 0;
    }

    private void PlanetButton()
    {
        string label = "temp";
        bool active = false;
        if (StartOfRound.Instance.levels == null || StartOfRound.Instance.levels.Length == 0)
        {
            label = "error";
            active = false;
        }
        else
        {
            // Ensure planetIndex is within valid range
            planetIndex = Mathf.Clamp(planetIndex, 0, StartOfRound.Instance.levels.Length - 1);
            label = StartOfRound.Instance.levels[planetIndex].PlanetName;
            active = (!StartOfRound.Instance.travellingToNewLevel && StartOfRound.Instance.inShipPhase);
        }

        planetIndex = worldMenuGUI.AddSlider(0, StartOfRound.Instance.levels.Length - 1, planetIndex, label, active);
        worldMenuGUI.AddButton(LKey.CyclePlanet, () => { WorldMods.ChangePlanet(planetIndex); }, active);
    }

    private void SoundButton()
    {
        Terminal terminal = ReflectionUtil.ReflectField<HUDManager>(HUDManager.Instance, "terminalScript") as Terminal;
        audio_index = worldMenuGUI.AddSlider(0, terminal.syncedAudios.Length - 1, audio_index, Mathf.RoundToInt(audio_index).ToString());
        worldMenuGUI.AddButton(LKey.PlaySound, () =>
        {
            terminal.PlayTerminalAudioServerRpc(audio_index);
        });
    }
}