using GameNetcodeStuff;
using UnityEngine;

namespace pdkmMenu
{
    public static class TeleportEnemyMod
    {
        public static void TeleportSingleEnemy(EnemyAI enemy, PlayerControllerB player = null, Vector3 position = default)
        {
            PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;
            
            if (localPlayer == null || enemy == null || HUDManager.Instance == null)
            {
                return;
            }

            enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
            SyncEnemyPosition(enemy, player, default);
        }

        public static void SyncEnemyPosition(EnemyAI enemy, PlayerControllerB player = null, Vector3 position = default)
        {
            if (enemy == null)
            {
                return;
            }

            Vector3 targetVector = player != null ? player.transform.position : position;

            if (targetVector == default)
            {
                targetVector = GameNetworkManager.Instance.localPlayerController.transform.position;
            }

            enemy.transform.position = targetVector;
            enemy.serverPosition = targetVector;
            enemy.SyncPositionToClients();
        }
        public static void TeleportAllMobsTo(PlayerControllerB targetPlayer)
        {
            foreach (EnemyAI enemyAI in EnemyAI_Patches.activeEnemies)
            {
                TeleportSingleEnemy(enemyAI, targetPlayer);
            }
        }
    }
}
