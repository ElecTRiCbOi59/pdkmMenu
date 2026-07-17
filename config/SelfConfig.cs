using BepInEx.Configuration;
using pdkmMenu;
using UnityEngine;

public class SelfConfig : ConfigCategory
{
    public ConfigEntry<bool> GodMode { get; private set; }
    public ConfigEntry<bool> InfAmmo { get; private set; }
    public ConfigEntry<bool> ShootFast { get; private set; }
    public ConfigEntry<bool> InfBattery { get; private set; }
    public ConfigEntry<bool> InfTet { get; private set; }
    public ConfigEntry<bool> InfSprint { get; private set; }
    public ConfigEntry<float> NightVisionIntensity { get; private set; }
    public ConfigEntry<bool> NightVision { get; private set; }
    public ConfigEntry<bool> GrabInLobby { get; private set; }
    public ConfigEntry<bool> Visor { get; private set; }
    public ConfigEntry<bool> LockPicker { get; private set; }
    public ConfigEntry<bool> CustomInteractions { get; private set; }
    public ConfigEntry<bool> CustomInteractDoor { get; private set; }
    public ConfigEntry<bool> CustomInteractMine { get; private set; }
    public ConfigEntry<bool> CustomInteractTurret { get; private set; }
    public ConfigEntry<bool> CustomInteractSpikeRoofTrap { get; private set; }
    public ConfigEntry<bool> HideFog { get; private set; }



    public ConfigEntry<float> GrabDistanceValue { get; private set; }
    public ConfigEntry<bool> GrabDistance { get; private set; }
    public ConfigEntry<float> PlayerSpeed { get; private set; }
    public ConfigEntry<bool> SpeedHackEnabled { get; private set; }

    public ConfigEntry<bool> ToolTips { get; private set; }
    public ConfigEntry<bool> HearAll { get; private set; }
    public ConfigEntry<bool> AlwaysShowUserNames { get; private set; }
    public ConfigEntry<bool> FastClimb { get; private set; }
    public ConfigEntry<float> FastClimbSpeed { get; private set; }
    public ConfigEntry<bool> DisplayAllExits { get; private set; }
    public ConfigEntry<bool> EnablePathGuide { get; private set; }
    public ConfigEntry<bool> DisplayMainDoorPath { get; private set; }
    public ConfigEntry<bool> DisplaySideDoorsPath { get; private set; }
    public ConfigEntry<bool> DisplayShipPath { get; private set; }
    public ConfigEntry<bool> PlayerIsTargetable { get; private set; }
    public ConfigEntry<bool> AntiQuickSand { get; private set; }



    public ConfigEntry<KeyboardShortcut> GodModeHotKey { get; private set; }
    public ConfigEntry<KeyboardShortcut> TpToEntrance { get; private set; }
    public ConfigEntry<KeyboardShortcut> TpToShip { get; private set; }
    public ConfigEntry<KeyboardShortcut> NoClipHotKey { get; private set; }
    public ConfigEntry<KeyboardShortcut> FreeCamHotKey { get; private set; }
    public ConfigEntry<KeyboardShortcut> SpectateCycleHotKey { get; private set; }
    public ConfigEntry<KeyboardShortcut> KillAllMobsHotKey { get; private set; }
    public ConfigEntry<KeyboardShortcut> KillNearestMobHotKey { get; private set; }
    public ConfigEntry<KeyboardShortcut> UnlockNearestDoorHotKey { get; private set; }
    public ConfigEntry<KeyboardShortcut> OpenNearestBigDoorHotKey { get; private set; }
    public ConfigEntry<KeyboardShortcut> ToggleAllTurretsHotKey { get; private set; }
    public ConfigEntry<KeyboardShortcut> DisableNearestTurretsHotKey { get; private set; }
    public ConfigEntry<KeyboardShortcut> SpeedHackHotKey { get; private set; }
    public ConfigEntry<KeyboardShortcut> ShootFastHotKey { get; private set; }

    public SelfConfig(ConfigFile config) : base(config) { }

    protected override void BindConfigs()
    {
        GodMode = Bind("Self", "GodMode", false, "Enable GodMode");
        PlayerIsTargetable = Bind("Self", "PlayerIsTargetable", true, "can enemies target you");
        AntiQuickSand = Bind("Self", "AnitiQuickSand", false, "enable anti quick sand");
        DisplayAllExits = Bind("Self", "DisplayAllExits", false, "Display paths to all possible exits (true) or only the main entrance/nearest (false).");
        EnablePathGuide = Bind("Self", "EnablePathGuide", false, "Master toggle for enabling/disabling the path guide altogether.");
        DisplayMainDoorPath = Bind("Self", "DisplayMainDoorPath", true, "Display path to the main entrance.");
        DisplaySideDoorsPath = Bind("Self", "DisplaySideDoorsPath", true, "Display paths to fire exits / secondary entrances.");
        DisplayShipPath = Bind("Self", "DisplayShipPath", true, "Display path to the ship.");
        InfAmmo = Bind("Self", "InfAmmo", false, "Enable InfAmmo");
        ShootFast = Bind("Self", "ShootFast", false, "Hold mouse 1 to shoot shotgun");
        InfBattery = Bind("Self", "InfBattery", false, "Enable InfBattery");
        InfTet = Bind("Self", "InfTet", false, "Enable InfTet");
        InfSprint = Bind("Self", "InfSprint", false, "Enable InfSprint");
        HideFog = Bind("Self", "HideFog", false, "Enable HideFog");

        NightVisionIntensity = Bind("Self", "NightVisionIntensity", 1.0f, "NightVision Intensity");
        NightVision = Bind("Self", "NightVision", false, "Enable NightVision");
        GrabInLobby = Bind("Self", "GrabInLobby", true, "Pick up anything in lobby.");
        Visor = Bind("Self", "Visor", true, "Toggle visor");
        LockPicker = Bind("Self", "LockPicker", false, "When enabled pressing e will unlock any door");
        CustomInteractions = Bind("Self", "CustomInteractrions", false, "Allows interacting with Landmines, turrents, big doors");
        CustomInteractDoor = Bind("Self", "CustomInteractDoor", true, "allows interacting with bigdoors");
        CustomInteractMine = Bind("Self", "CustomInteractMine", true, "allows interacting with mines");
        CustomInteractTurret = Bind("Self", "CustomInteractTurret", true, "allows interacting with turrets");
        CustomInteractSpikeRoofTrap = Bind("Self", "CustomInteractSpikeRoofTrap", true, "allows interacting with SpikeRoofTrap");

        GrabDistanceValue = Bind("Self", "GrabDistanceValue", 5.0f, "The distance you can grab");
        GrabDistance = Bind("Self", "GrabDistance", false, "Enable GrabDistance");

        PlayerSpeed = Bind("Self", "PlayerSpeed", 5.0f, "The speed you can go");
        SpeedHackEnabled = Bind("Self", "SpeedHackEnabled", false, "Enables speed hack");
        SpeedHackHotKey = Bind("Self", "SpeedHackHotKey", new KeyboardShortcut(KeyCode.None), "");



        ToolTips = Bind("Self", "ToolTips", true, "Display controls for items in top left");
        HearAll = Bind("Self", "HearAll", false, "Enable HearAll");
        AlwaysShowUserNames = Bind("Self", "AlwaysShowUserNames", false, "Force show username above head.");
        FastClimb = Bind("Self", "FastClimb", false, "Climb Fast.");
        FastClimbSpeed = Bind("Self", "FastClimbSpeed", 12.0f, "Climb speed when FastClimb is enabled");



        GodModeHotKey = Bind("Self", "GodModeHotKey", new KeyboardShortcut(KeyCode.None), "");
        TpToEntrance = Bind("Self", "TpToEntrance", new KeyboardShortcut(KeyCode.None), "");
        TpToShip = Bind("Self", "TpToShip", new KeyboardShortcut(KeyCode.None), "");

        NoClipHotKey = Bind("Self", "NoClipHotKey", new KeyboardShortcut(KeyCode.None), "NoClip Toggle Hotkey");

        FreeCamHotKey = Bind("Self", "FreeCamHotKey", new KeyboardShortcut(KeyCode.None), "FreeCam Toggle Hotkey");
        SpectateCycleHotKey = Bind("Self", "SpectateCycleHotKey", new KeyboardShortcut(KeyCode.Tab), "NOT IMPLEMENTED YET. Spectate Cycle Hotkey");

        KillAllMobsHotKey = Bind("Self", "KillAllMobsHotKey", new KeyboardShortcut(KeyCode.None), "");
        KillNearestMobHotKey = Bind("Self", "KillNearestMobHotKey", new KeyboardShortcut(KeyCode.None), "");

        UnlockNearestDoorHotKey = Bind("Self", "UnlockNearestDoorHotKey", new KeyboardShortcut(KeyCode.None), "");
        OpenNearestBigDoorHotKey = Bind("Self", "OpenNearestBigDoorHotKey", new KeyboardShortcut(KeyCode.None), "");

        ToggleAllTurretsHotKey = Bind("Self", "DisableAllTurretsHotKey", new KeyboardShortcut(KeyCode.None), "");
        DisableNearestTurretsHotKey = Bind("Self", "DisableNearestTurretsHotKey", new KeyboardShortcut(KeyCode.None), "");

        ShootFastHotKey = Bind("Self", "ShootFastHotKey", new KeyboardShortcut(KeyCode.None), "Key to toggle fastshooting");


    }


    public override void CheckKeys()
    {
        //if (NoClipKey.Value.IsDown()) ToggleConfigEntry(GodMode);
        if (GodModeHotKey.Value.IsDown()) GodModeMod.ToggleGodMode();
        if (TpToEntrance.Value.IsDown()) TeleportMod.TeleportToEntrance();
        if (TpToShip.Value.IsDown()) TeleportMod.TeleportToShip();

        if (FreeCamHotKey.Value.IsDown()) FreeCamMod.IsFreeCam = !FreeCamMod.IsFreeCam;
        if (NoClipHotKey.Value.IsDown()) NoClipMod.IsNoClip = !NoClipMod.IsNoClip;


        if (KillAllMobsHotKey.Value.IsDown()) WorldMods.KillAllMobs();
        if (KillNearestMobHotKey.Value.IsDown()) WorldMods.KillAllMobs(StartOfRound.Instance.localPlayerController);

        if (UnlockNearestDoorHotKey.Value.IsDown()) WorldMods.UnlockAllDoors(StartOfRound.Instance.localPlayerController);
        if (OpenNearestBigDoorHotKey.Value.IsDown()) WorldMods.ToggleDoors(StartOfRound.Instance.localPlayerController);

        if (ToggleAllTurretsHotKey.Value.IsDown()) WorldMods.ToggleTurrets();
        if (DisableNearestTurretsHotKey.Value.IsDown()) WorldMods.ChangeNearestTurret(StartOfRound.Instance.localPlayerController, false);


        if (SpeedHackHotKey.Value.IsDown()) ToggleConfigEntry(SpeedHackEnabled) ;

    }
}
