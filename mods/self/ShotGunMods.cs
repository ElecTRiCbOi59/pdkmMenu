using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace pdkmMenu
{
    public class ShotGunMods
    {
        public static void InfiniteAmmoUpdateMod()
        {
            if (!Plugin.SelfSettings.InfAmmo.Value) return;
            if (GameNetworkManager.Instance.localPlayerController == null) return;
            if (GameNetworkManager.Instance.localPlayerController.ItemSlots == null) return;

            GrabbableObject[] slots = GameNetworkManager.Instance.localPlayerController.ItemSlots;
            foreach (GrabbableObject item in slots)
            {
                if (item != null)
                {
                    if (item.GetComponent<ShotgunItem>() != null)
                    {
                        item.GetComponent<ShotgunItem>().shellsLoaded = 2;
                    }
                }
            }
        }
        public static void MachineShotGunUpdateMod()
        {
            PlayerControllerB player = StartOfRound.Instance.localPlayerController;
            if (player == null) return;
            if (Plugin.SelfSettings.ShootFast.Value)
            {
                ShotgunItem shotgunItem = player.currentlyHeldObjectServer as ShotgunItem;
                if (shotgunItem == null) return;
                if (Mouse.current.leftButton.isPressed && !shotgunItem.safetyOn)
                {
                    shotgunItem.ShootGunServerRpc(player.transform.position - player.gameplayCamera.transform.up * 0.45f, player.gameplayCamera.transform.forward);
                }
            }
        }
    }
}