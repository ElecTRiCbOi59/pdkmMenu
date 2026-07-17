using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace pdkmMenu
{

    public static class PlayerSpinBot
    {
        public static List<PlayerControllerB> playerControllerBs = new List<PlayerControllerB>();
        private static float rotation = 0;
        private static float timeElapsed = 0f;
        private static float interval = 0.1f; // 1 second interval
        public static void UpdateMod()
        {
            timeElapsed += Time.deltaTime; // Increase time elapsed by the time between frames

            if (timeElapsed <= interval)
            {
                return;
            }
            timeElapsed = 0f;
            if (!StartOfRound.Instance.IsServer)  return;

            foreach (PlayerControllerB player in playerControllerBs)
            {
                if(player == null)
                {
                    playerControllerBs.Remove(player);
                    continue;
                }
                //ReflectionUtil.ReflectMethod<PlayerControllerB>(player, "", new object[] { });
                short rotx = 0;
                short roty = (short)rotation;


                ReflectionUtil.ReflectMethod<PlayerControllerB>(player, "UpdatePlayerRotationClientRpc", new object[] { rotx, roty });
            }
            rotation+= 5;
            if (rotation >= 360)
            {
                rotation = 0;
            }
        }

        public static void ModifyPlayer(PlayerControllerB player)
        {
            if (playerControllerBs.Contains(player))
            {
                playerControllerBs.Remove(player);
            }else
            {
                playerControllerBs.Add(player);
            }
        }
    }
}
