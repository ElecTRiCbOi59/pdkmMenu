using GameNetcodeStuff;
using UnityEngine;

namespace pdkmMenu
{
    public class PlayerMods
    {
        public static void ChangePlayerHealh(PlayerControllerB player, int Amount)
        {
            if (GameNetworkManager.Instance.localPlayerController.IsHost)
            {
                player.DamagePlayerServerRpc(0, Amount*1);
            }
            else
            {
                player.DamagePlayerFromOtherClientServerRpc(Amount, new UnityEngine.Vector3(1f, 2f, 3f), (int)player.playerClientId);
                player.MakeCriticallyInjured(false);
            }
                

        }
        public static void KillPlayer(PlayerControllerB player)
        {
            //KillPlayerServerRpcPrefix(player, (int)player.actualClientId, true, kilvec, 0, 1);
            Vector3 kilvec = new Vector3(1.0f, 2.0f, 3.0f);
            //ReflectionUtil.ReflectMethod<PlayerControllerB>(player, "KillPlayerServerRpc", new object[] {(int)player.playerClientId, true, kilvec,0, 1, default(Vector3) });
            //ReflectionUtil.ReflectMethod<PlayerControllerB>(player, "KillPlayerClientRpc", new object[] { (int)player.playerClientId, true, kilvec, 0, 1, default(Vector3) });
            //player.KillPlayer(kilvec);
            player.DamagePlayerFromOtherClientServerRpc(player.health, new Vector3(0f, 0f, 0f), -1);
        }
       

        public static string Message = "Hello";
        public static void SendChatAs(PlayerControllerB player)
        {
            LogToChat.SendChat(Message, player);
        }

        public static void UpdateMod()
        {
            OrbitItemsMod.UpdateMod();
            //PlayerSpinBot.UpdateMod();
        }
    }
}
