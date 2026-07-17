using pdkmMenu;
using UnityEngine;

public class SelfMenu : MonoBehaviour
{
    private guiBase selfMenuGUI;
    private EspMenu espMenu;
    private SelfConfig selfConfig;

    public enum SelfMenuPages
    {
        None,
        esp
    }

    public SelfMenuPages currentSubmenu = SelfMenuPages.None;

    private void Start()
    {
        selfMenuGUI = gameObject.AddComponent<guiBase>();
        selfMenuGUI.MenuColor = selfMenuGUI.CustomBlue;
        selfMenuGUI.YPercentage = 0.0f;
        espMenu = gameObject.AddComponent<EspMenu>();
        selfConfig = Plugin.SelfSettings;
    }

    private static int ChallengeVal = int.MaxValue;

    public void update()
    {
        // --- COLUMN 1 ---
        selfMenuGUI.CurrentColumn = 1;
        selfMenuGUI.ButtonIndex = 0;
        selfMenuGUI.AddButton(LKey.TeleportAllItems, () => { StartCoroutine(TeleportAllMod.TeleportAllGrabbableObjectsToPlayer()); });
        selfMenuGUI.AddButton(LKey.GodMode, () => { selfConfig.ToggleConfigEntry(selfConfig.GodMode); }, selfConfig.GodMode.Value);
        selfMenuGUI.AddButton(LKey.IsTargetable, () => { selfConfig.ToggleConfigEntry(selfConfig.PlayerIsTargetable); }, selfConfig.PlayerIsTargetable.Value);
        selfMenuGUI.AddButton(LKey.InfAmmo, () => { selfConfig.ToggleConfigEntry(selfConfig.InfAmmo); }, selfConfig.InfAmmo.Value);
        selfMenuGUI.AddButton(LKey.ShootFast, () => { selfConfig.ToggleConfigEntry(selfConfig.ShootFast); }, selfConfig.ShootFast.Value);
        selfMenuGUI.AddButton(LKey.InfBattery, () => { selfConfig.ToggleConfigEntry(selfConfig.InfBattery); }, selfConfig.InfBattery.Value);
        selfMenuGUI.AddButton(LKey.InfTet, () => { selfConfig.ToggleConfigEntry(selfConfig.InfTet); }, selfConfig.InfTet.Value);
        selfMenuGUI.AddButton(LKey.InfSprint, () => { selfConfig.ToggleConfigEntry(selfConfig.InfSprint); }, selfConfig.InfSprint.Value);
        selfMenuGUI.AddButton(LKey.AntiQuickSand, () => { selfConfig.ToggleConfigEntry(selfConfig.AntiQuickSand); }, selfConfig.AntiQuickSand.Value);
        selfMenuGUI.AddButton(LKey.HearAll, () => { selfConfig.ToggleConfigEntry(selfConfig.HearAll); }, selfConfig.HearAll.Value);
        selfMenuGUI.AddButton(LKey.Visor, () => { selfConfig.ToggleConfigEntry(selfConfig.Visor); }, selfConfig.Visor.Value);
        selfMenuGUI.AddButton(LKey.FastClimb, () => { selfConfig.ToggleConfigEntry(selfConfig.FastClimb); }, selfConfig.FastClimb.Value);
        selfMenuGUI.AddButton(LKey.GrabInLobby, () => { selfConfig.ToggleConfigEntry(selfConfig.GrabInLobby); }, selfConfig.GrabInLobby.Value);
        selfConfig.GrabDistanceValue.Value = selfMenuGUI.AddSlider(1.0f, 500.0f, selfConfig.GrabDistanceValue.Value, selfConfig.GrabDistanceValue.Value.ToString("0.###"), selfConfig.GrabDistance.Value, 0f, true);
        selfMenuGUI.AddButton(LKey.GrabDistance, () => { selfConfig.ToggleConfigEntry(selfConfig.GrabDistance); }, selfConfig.GrabDistance.Value);
        selfConfig.NightVisionIntensity.Value = selfMenuGUI.AddSlider(1.0f, 50.0f, selfConfig.NightVisionIntensity.Value, selfConfig.NightVisionIntensity.Value.ToString("0.###"), selfConfig.NightVision.Value, 0f, true);
        selfMenuGUI.AddButton(LKey.NightVision, () => { selfConfig.ToggleConfigEntry(selfConfig.NightVision); }, selfConfig.NightVision.Value);
        selfConfig.PlayerSpeed.Value = selfMenuGUI.AddSlider(1.0f, 50.0f, selfConfig.PlayerSpeed.Value, selfConfig.PlayerSpeed.Value.ToString("0.###"), selfConfig.SpeedHackEnabled.Value, 0f, true);
        selfMenuGUI.AddButton(LKey.SpeedHack, () => { selfConfig.ToggleConfigEntry(selfConfig.SpeedHackEnabled); }, selfConfig.SpeedHackEnabled.Value);

        // --- COLUMN 2 ---
        selfMenuGUI.CurrentColumn = 2;
        selfMenuGUI.ButtonIndex = 0; // Reset Y-axis for the new column
        selfMenuGUI.AddButton(LKey.ESP, () => SetSubMenu(SelfMenuPages.esp), currentSubmenu == SelfMenuPages.esp);
        selfMenuGUI.AddButton(LKey.NoClip, () => { NoClipMod.IsNoClip = !NoClipMod.IsNoClip; }, NoClipMod.IsNoClip);
        selfMenuGUI.AddButton(LKey.FreeCam, () => { FreeCamMod.IsFreeCam = !FreeCamMod.IsFreeCam; }, FreeCamMod.IsFreeCam);
        selfMenuGUI.AddButton(LKey.TeleportToEntrance, () => { TeleportMod.TeleportToEntrance(); });
        selfMenuGUI.AddButton(LKey.TeleportToShip, () => { TeleportMod.TeleportToShip(); });
        selfMenuGUI.AddButton(LKey.Recall, () => { TeleportMod.Recall(); });
        selfMenuGUI.AddButton(LKey.Mark, () => { TeleportMod.MarkPos(); });
        selfMenuGUI.AddButton(LKey.TpToMark, () => { TeleportMod.TeleportToMarkPos(); });

        ChallengeVal = selfMenuGUI.IntTextBox(ChallengeVal);
        selfMenuGUI.AddButton(LKey.SubmitChallengeScrap, () => { HUDManager.Instance.FillChallengeResultsStats(ChallengeVal); });
        selfMenuGUI.AddButton(LKey.CustomInteraction, () => { CustomInteractions.ToggleMasterInteractions(); }, selfConfig.CustomInteractions.Value);
        selfMenuGUI.AddButton(LKey.LockPicker, () => { selfConfig.ToggleConfigEntry(selfConfig.LockPicker); }, selfConfig.LockPicker.Value);
        selfMenuGUI.AddButton(LKey.ToolTips, () => { selfConfig.ToggleConfigEntry(selfConfig.ToolTips); }, selfConfig.ToolTips.Value);
        selfMenuGUI.AddButton(LKey.AlwaysName, () => { selfConfig.ToggleConfigEntry(selfConfig.AlwaysShowUserNames); }, selfConfig.AlwaysShowUserNames.Value);
        selfMenuGUI.AddButton(LKey.HideFog, () => { selfConfig.ToggleConfigEntry(selfConfig.HideFog); HideFog.UpdateFogState(); }, selfConfig.HideFog.Value);

        // Submenu Handling
        switch (currentSubmenu)
        {
            case SelfMenuPages.esp:
                espMenu.update();
                break;
        }

        // Reset for next frame
        selfMenuGUI.ButtonIndex = 0;
        selfMenuGUI.CurrentColumn = 0;
    }

    public void SetSubMenu(SelfMenuPages input)
    {
        if (input == currentSubmenu)
        {
            currentSubmenu = SelfMenuPages.None;
        }
        else
        {
            currentSubmenu = input;
        }
    }
}