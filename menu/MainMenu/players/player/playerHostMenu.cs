using UnityEngine;
using GameNetcodeStuff;
using Unity.Netcode;
using pdkmMenu;

public class PlayerHostSharedMenu : MonoBehaviour
{
    private guiBase gui;
    private static int combinedMobIndex = 0;
    private static int ObjectIndex = 0;

    private void Start()
    {
        gui = gameObject.AddComponent<guiBase>();
        gui.MenuColor = gui.CustomBlue;
        gui.YPercentage = 0.0f;
    }

    public void update(PlayerControllerB target)
    {
        // Security: Only Host or Verified Co-Host can see/use this
        if (!NetworkManager.Singleton.IsServer && !CoHostHandler.Instance.IsCoHost) return;

        gui.CurrentColumn = 3;
        gui.ButtonIndex = 0;

        ulong tid = target.actualClientId;

        // 1. Host-Only: Manage Permissions
        if (NetworkManager.Singleton.IsServer && target!=StartOfRound.Instance.localPlayerController)
        {
            bool isCo = CoHostHandler.Instance.VerifiedTokens.ContainsKey(tid);
            gui.AddButton(isCo ? LKey.RevokeCoHost : LKey.GiveCoHost,
                () => CoHostHandler.Instance.SetCoHost(tid, !isCo), isCo);
        }

        // 2. Shared Actions: Player Interactions
        gui.AddButton(LKey.Heal, () => Request($"heal:{tid}"));
        gui.AddButton(LKey.MakeLimp, () => Request($"limp:{tid}"));
        gui.AddButton(LKey.DisableJetpack, () => Request($"nojet:{tid}"));
        gui.AddButton(LKey.LightningStrike, () => Request($"lightning:{tid}"));

        gui.AddButton(LKey.BreakLegsSfx, () => Request($"legsfx:{tid}"));
        //gui.AddButton(LKey.LandFromJumpSfx, () => Request($"landfromjumpsfx:{tid}"));
        gui.AddButton(LKey.JumpSfx, () => Request($"jumpsfx:{tid}"));

        gui.AddButton(LKey.Spawning, () => { }, false);

        RenderSpawning(target);

        gui.ButtonIndex = 0;
        gui.CurrentColumn = 0;
    }

    private void RenderSpawning(PlayerControllerB target)
    {
        var enemies = SpawnHelper.GetallEnemies();
        var mapObjs = RoundManager.Instance.currentLevel.spawnableMapObjects;
        ulong tid = target.actualClientId;

        // Enemy Slider/Buttons (Including the "There/Laggy" option)
        if (enemies != null && enemies.Count > 0)
        {
            combinedMobIndex = gui.AddSlider(0, enemies.Count - 1, combinedMobIndex, enemies[Mathf.Clamp(combinedMobIndex, 0, enemies.Count - 1)].enemyType.enemyName);

            gui.AddButton(LKey.SpawnNearPlayer, () => Request($"spawnmob|{combinedMobIndex}:{tid}"));
            gui.AddButton(LKey.SpawnThereLaggy, () => Request($"spawnmob|{combinedMobIndex}|there:{tid}"));
            gui.AddButton(LKey.MeteorStrike, () => Request($"meteor:{tid}"));
        }
        else
        {
            gui.ButtonIndex++; // Maintain layout spacing
        }

        // Object Slider/Button
        if (mapObjs != null && mapObjs.Length > 0)
        {
            ObjectIndex = gui.AddSlider(0, mapObjs.Length - 1, ObjectIndex, mapObjs[Mathf.Clamp(ObjectIndex, 0, mapObjs.Length - 1)].prefabToSpawn.name);
            gui.AddButton(LKey.SpawnMapObject, () => Request($"spawnobj|{ObjectIndex}:{tid}"));
        }
    }

    private void Request(string cmd) => CoHostHandler.Instance.RequestAction(cmd);
}