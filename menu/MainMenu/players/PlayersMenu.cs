using pdkmMenu;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameNetcodeStuff;



public class PlayersMenu : MonoBehaviour
{
    private guiBase playersMenuGUI;
    private PlayerMenu playerMenu;
    private SelfConfig selfConfig;
    private PlayerControllerB SelectedPlayer = null;
    public enum SelfMenuPages
    {
        None,
        player
    }
    public SelfMenuPages currentSubmenu = SelfMenuPages.None;

    private void Start()
    {
        playersMenuGUI = gameObject.AddComponent<guiBase>();
        playersMenuGUI.MenuColor = playersMenuGUI.CustomBlue;
        playersMenuGUI.YPercentage = 0.0f;
        playerMenu = gameObject.AddComponent<PlayerMenu>();
        selfConfig = Plugin.SelfSettings;
    }

    private static Vector2 scrollPos = Vector2.zero;
    private static int index = 0;
    public void update()
    {
        playersMenuGUI.CurrentColumn = 1;
        playersMenuGUI.ButtonIndex = 0;
        Dictionary<string, Tuple<bool, Action>> playerButtons = new Dictionary<string, Tuple<bool, Action>>();

        foreach (PlayerControllerB player in GetRealPlayerScripts())
        {
            playerButtons.Add($"{player.playerUsername}.{player.actualClientId}.{player.playerClientId}",
        Tuple.Create(SelectedPlayer == player, (Action)(() => SetSelectedPlayer(player))));
        }
        playersMenuGUI.AddButtonList(playerButtons, ref scrollPos, index);
        if (SelectedPlayer == null) return;
        playerMenu.update(SelectedPlayer);

        playersMenuGUI.ButtonIndex = 0;
        playersMenuGUI.CurrentColumn = 0;
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


    private void SetSelectedPlayer(PlayerControllerB player)
    {
        if(player == SelectedPlayer)
        {
            SelectedPlayer = null;
        }
        else
        {
            SelectedPlayer = player;
        }
    }
    public static PlayerControllerB[] GetRealPlayerScripts()
    {
        if (StartOfRound.Instance == null || StartOfRound.Instance.allPlayerScripts == null)
        {
            //startOfRound.allPlayerScripts[0].
            return new PlayerControllerB[0];
        }
        return (from x in StartOfRound.Instance.allPlayerScripts
                where x.isPlayerDead || x.isPlayerControlled
                select x).ToArray<PlayerControllerB>();
    }
}

