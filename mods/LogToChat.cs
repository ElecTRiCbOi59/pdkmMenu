using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.InputSystem.InputRemoting;

namespace pdkmMenu
{
    public class LogToChat
    {
        //18 char wide
        //message len max is 50
        //only checked in serverrpc.
        //<color=#FF0000> test </color>
        //private void AddTextMessageServerRpc(string chatMessage)
        //HUDManager.Instance.AddTextMessageServerRpc("\n");
        //HUDManager.Instance.AddTextMessageServerRpc("\n\n");
        //HUDManager.Instance.AddTextMessageServerRpc("\n");
        //HUDManager.Instance.AddTextMessageServerRpc("\n\n");
        //flag = <color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color>.........<color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color>.........<color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color>.........<color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color>.........<color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color>.........<color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color>.........<color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color>.........<color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color>.........<color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color>.........<color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color><color=#0000FF>.</color><color=#FFFFFF>.</color><color=#FF0000>.</color>
        public static void LogLocal(string Message)
        {
            Console.WriteLine(Message);
            ReflectionUtil.ReflectMethod<HUDManager>(HUDManager.Instance, "AddChatMessage", new object[] { Message, "", -1, false });
        }
        public static void SendChat(string Message, PlayerControllerB player = null)
        {
            int PlayerId = -1;
            if(player != null)
            {
                PlayerId = (int)player.playerClientId;
            }
            HUDManager.Instance.AddTextToChatOnServer(Message, PlayerId);
        }
        public static void SendServerMessage(string Message, PlayerControllerB player = null)
        {
            //int PlayerId = -1;
            //if(player != null)
            //{
            //    PlayerId = (int)player.actualClientId;
            //}
            ReflectionUtil.ReflectMethod<HUDManager>(HUDManager.Instance, "AddTextMessageServerRpc", new object[] { Message });
            //HUDManager.Instance.AddTextMessageServerRpc(Message);
        }

    }
}
