using GameNetcodeStuff;
using UnityEngine;

namespace pdkmMenu
{
    public static class EnemyTarget
    {
        public static void AllTarget(PlayerControllerB victim)
        {
            if (victim == null) return;
            foreach (var enemy in EnemyAI_Patches.activeEnemies)
            {
                if (enemy == null || enemy.isEnemyDead) continue;
                ApplyTargeting(enemy, victim);
            }
        }

        public static void Target(PlayerControllerB victim, EnemyAI enemy)
        {
            if (enemy == null || victim == null || enemy.isEnemyDead) return;
            ApplyTargeting(enemy, victim);
        }

        private static void ApplyTargeting(EnemyAI enemy, PlayerControllerB victim)
        {
            enemy.targetPlayer = victim;
            enemy.ChangeEnemyOwnerServerRpc(StartOfRound.Instance.localPlayerController.actualClientId);
            enemy.SetMovingTowardsTargetPlayer(victim);
            SetTargetByEnemyType(enemy, victim);
        }

        private static void SetTargetByEnemyType(EnemyAI enemy, PlayerControllerB victim)
        {
            switch (enemy)
            {
                case CrawlerAI e: SetTargetCrawler(e, victim); break;
                case MouthDogAI e: SetTargetMouthDog(e, victim); break;
                case BaboonBirdAI e: SetTargetBaboonBird(e, victim); break;
                case ForestGiantAI e: SetTargetForestGiant(e, victim); break;
                case CentipedeAI e: SetTargetCentipede(e, victim); break;
                case FlowermanAI e: SetTargetFlowerman(e, victim); break;
                case SandSpiderAI e: SetTargetSandSpider(e, victim); break;
                case RedLocustBees e: SetTargetBees(e, victim); break;
                case HoarderBugAI e: SetTargetHoarderBug(e, victim); break;
                case NutcrackerEnemyAI e: SetTargetNutcracker(e, victim); break;
                case BushWolfEnemy e: SetTargetBushWolf(e, victim); break;
                case RadMechAI e: SetTargetRadMech(e, victim); break;
                default:
                    enemy.SwitchToBehaviourServerRpc(1);
                    break;
            }
        }

        private static void SetTargetCrawler(CrawlerAI crawler, PlayerControllerB victim) =>
            crawler.BeginChasingPlayerServerRpc((int)victim.playerClientId);

        private static void SetTargetMouthDog(MouthDogAI dog, PlayerControllerB victim) =>
            dog.ReactToOtherDogHowl(victim.transform.position);

        private static void SetTargetBaboonBird(BaboonBirdAI baboon, PlayerControllerB victim)
        {
            var threat = new Threat
            {
                threatScript = victim,
                lastSeenPosition = victim.transform.position,
                threatLevel = int.MaxValue,
                type = ThreatType.Player,
                focusLevel = int.MaxValue,
                timeLastSeen = Time.time,
                interestLevel = int.MaxValue,
                hasAttacked = true
            };
            baboon.SetAggressiveModeServerRpc(1);
            ReflectionUtil.ReflectMethod<BaboonBirdAI>(baboon, "ReactToThreat", new object[] { threat });
        }

        private static void SetTargetForestGiant(ForestGiantAI giant, PlayerControllerB victim)
        {
            giant.SwitchToBehaviourServerRpc(1);
            giant.StopSearch(giant.roamPlanet, false);
            giant.chasingPlayer = victim;
            giant.investigating = true;
            giant.SetDestinationToPosition(victim.transform.position, false);
            ReflectionUtil.ReflectSetField<ForestGiantAI>(giant, "lostPlayerInChase", false);
        }

        private static void SetTargetCentipede(CentipedeAI centipede, PlayerControllerB victim)
        {
            centipede.SwitchToBehaviourServerRpc(2);
            if ((bool)ReflectionUtil.ReflectField<CentipedeAI>(centipede, "clingingToCeiling"))
                centipede.TriggerCentipedeFallServerRpc(victim.playerClientId);
        }

        private static void SetTargetFlowerman(FlowermanAI bracken, PlayerControllerB victim)
        {
            bracken.SwitchToBehaviourServerRpc(2);
            bracken.EnterAngerModeServerRpc(20f);
        }

        private static void SetTargetSandSpider(SandSpiderAI spider, PlayerControllerB victim)
        {
            spider.SwitchToBehaviourServerRpc(1);
            var ray = new Ray(victim.transform.position, Vector3.Scale(Random.onUnitSphere, new Vector3(1f, Random.Range(0.6f, 1f), 1f)));
            if (Physics.Raycast(ray, out RaycastHit hit, 7f, StartOfRound.Instance.collidersAndRoomMask))
            {
                if (Physics.Raycast(hit.point, Vector3.down, out RaycastHit groundHit, 10f, StartOfRound.Instance.collidersAndRoomMask))
                    spider.SpawnWebTrapServerRpc(groundHit.point, groundHit.point);
            }
            spider.webTraps.ForEach(web => spider.PlayerTripWebServerRpc(web.trapID, (int)victim.playerClientId));
        }

        private static void SetTargetBees(RedLocustBees bees, PlayerControllerB victim)
        {
            bees.SwitchToBehaviourServerRpc(2);
            if (bees.hive != null) bees.hive.isHeld = true;
        }

        private static void SetTargetHoarderBug(HoarderBugAI bug, PlayerControllerB victim)
        {
            bug.SwitchToBehaviourServerRpc(2);
            bug.angryAtPlayer = victim;
            bug.angryTimer = float.MaxValue;
            ReflectionUtil.ReflectSetField<HoarderBugAI>(bug, "lostPlayerInChase", false);
            ReflectionUtil.ReflectMethod<HoarderBugAI>(bug, "SyncNestPositionServerRpc", new object[] { victim.transform.position });
        }

        private static void SetTargetNutcracker(NutcrackerEnemyAI nut, PlayerControllerB victim)
        {
            nut.SwitchToBehaviourServerRpc(2);
            ReflectionUtil.ReflectSetField<NutcrackerEnemyAI>(nut, "lastSeenPlayerPos", victim.transform.position);
            ReflectionUtil.ReflectSetField<NutcrackerEnemyAI>(nut, "timeSinceSeeingTarget", 0.0f);
        }

        private static void SetTargetBushWolf(BushWolfEnemy wolf, PlayerControllerB victim)
        {
            ReflectionUtil.ReflectSetField<BushWolfEnemy>(wolf, "isHiding", false);
            ReflectionUtil.ReflectSetField<BushWolfEnemy>(wolf, "staringAtPlayer", true);
            wolf.SwitchToBehaviourServerRpc(1);
        }

        private static void SetTargetRadMech(RadMechAI mech, PlayerControllerB victim)
        {
            mech.SetChargingForwardClientRpc(true);
            mech.SwitchToBehaviourServerRpc(1);
        }
    }
}