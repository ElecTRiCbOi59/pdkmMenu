using GameNetcodeStuff;
using pdkmMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace pdkmMenu
{
    public class SelfMods
    {


        public static void UpdateMod()
        {
            if (GameNetworkManager.Instance.localPlayerController == null) return;
            GodModeMod.UpdateMod();
            ShotGunMods.InfiniteAmmoUpdateMod();
            ShotGunMods.MachineShotGunUpdateMod();
            InfBatteryMod.UpdateMod();
            InfTetra.UpdateMod();
            InfSprintMod.UpdateMod();
            NightVisionMod.UpdateMod();
            GrabDistanceMod.UpdateMod();
            ToolTipsMod.UpdateMod();
            NoClipMod.UpdateMod();
            FreeCamMod.UpdateMod();
            //HearAllMod.UpdateMod();
            SpeedHackMod.UpdateMod();
            FastClimbMod.UpdateMod();
            VisorMod.UpdateMod();
            if (StartOfRound.Instance != null && StartOfRound.Instance.mapScreen != null)
            {
                StartOfRound.Instance.mapScreen.overrideRadarCameraOnAlways = true;
            }
        }
    }
}
