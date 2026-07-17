using pdkmMenu;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameNetcodeStuff;
using System.Numerics;


public class PlayerMenu : MonoBehaviour
{
    private guiBase playerMenuGUI;
    private PlayerHostSharedMenu playerHostMenu;

    //private SelfConfig selfConfig;

    //public enum SelfMenuPages
    //{
    //    None,
    //    esp
    //}
    //public SelfMenuPages currentSubmenu = SelfMenuPages.None;

    private void Start()
    {
        playerMenuGUI = gameObject.AddComponent<guiBase>();
        playerMenuGUI.MenuColor = playerMenuGUI.CustomBlue;
        playerMenuGUI.YPercentage = 0.0f;
        playerHostMenu = gameObject.AddComponent<PlayerHostSharedMenu>();

        //selfConfig = Plugin.SelfSettings;
    }

    int audio_indexPlayer = 0;
    public void update(PlayerControllerB player)
    {
        playerMenuGUI.CurrentColumn = 2;
        playerMenuGUI.ButtonIndex = 0;

        if (StartOfRound.Instance.localPlayerController.IsServer || CoHostHandler.Instance.IsCoHost)
        {
            playerHostMenu.update(player);
        }
        //StartOfRound.Instance.localPlayerController.actualClientId == The id used in rpc calls
        //StartOfRound.Instance.localPlayerController.playerClientId == the players index.
        playerMenuGUI.AddButton(LKey.Spectate, () => Spectate.SwitchCamera(player));
        playerMenuGUI.AddButton(LKey.TpTo, () => TeleportMod.TeleportToPos(player.transform.position));
        audio_indexPlayer = playerMenuGUI.AddSlider(0, SoundManager.Instance.syncedAudioClips.Length, audio_indexPlayer, audio_indexPlayer.ToString());
        playerMenuGUI.AddButton(LKey.PlayAudio, () =>{var pos = player.transform.position;SoundManager.Instance.PlayAudio1AtPositionForAllClients(pos, audio_indexPlayer);});

        int damageAmount = 10;
        playerMenuGUI.AddButton(LKey.Damage, () => PlayerMods.ChangePlayerHealh(player, damageAmount));
        playerMenuGUI.AddButton(LKey.Heal, () => PlayerMods.ChangePlayerHealh(player, -damageAmount));
        playerMenuGUI.AddButton(LKey.Kill, () => PlayerMods.KillPlayer(player));
        PlayerMods.Message = playerMenuGUI.TextBox(PlayerMods.Message);
        playerMenuGUI.AddButton(LKey.ChatAs, () => PlayerMods.SendChatAs(player));
        playerMenuGUI.AddButton(LKey.OrbitItems, () => OrbitItemsMod.SetPlayer(player), (OrbitItemsMod.player == player));
        playerMenuGUI.AddButton(LKey.EndMatchAs, () => WorldMods.EndMatch((int)player.playerClientId));
        playerMenuGUI.AddButton(LKey.DropAll, () => player.DropAllHeldItemsAndSync(player.transform.position, player.localItemHolder.position, player.localItemHolder.eulerAngles, player.playerEye.position, player.playerEye.eulerAngles));

        playerMenuGUI.AddButton(LKey.OpenBigDoor, () => WorldMods.ToggleDoors(player));
        playerMenuGUI.AddButton(LKey.EnableTurret, () => WorldMods.ChangeNearestTurret(player, true));
        playerMenuGUI.AddButton(LKey.DisableTurret, () => WorldMods.ChangeNearestTurret(player, false));
        playerMenuGUI.AddButton(LKey.ExplodeMine, () => WorldMods.ExplodeAllMines(player));
        playerMenuGUI.AddButton(LKey.UnlockDoor, () => WorldMods.UnlockAllDoors(player));
        playerMenuGUI.AddButton(LKey.KillEnemy, () => WorldMods.KillAllMobs(player));
        playerMenuGUI.AddButton(LKey.TeleportEnemies, () => TeleportEnemyMod.TeleportAllMobsTo(player));
        playerMenuGUI.AddButton(LKey.EnemiesTarget, () => EnemyTarget.AllTarget(player));
        playerMenuGUI.ButtonIndex = 0;
        playerMenuGUI.CurrentColumn = 0;
    }


    //public void SetSubMenu(SelfMenuPages input)
    //{
    //    if (input == currentSubmenu)
    //    {
    //        currentSubmenu = SelfMenuPages.None;
    //    }
    //    else
    //    {
    //        currentSubmenu = input;
    //    }
    //}
}

