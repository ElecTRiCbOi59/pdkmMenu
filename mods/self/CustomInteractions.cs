using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace pdkmMenu
{
    public class CustomInteractions
    {
        public static void UpdateMod() { }

        public static void ToggleMasterInteractions()
        {
            Plugin.SelfSettings.ToggleConfigEntry(Plugin.SelfSettings.CustomInteractions);
            RefreshAll();
        }

        public static void ToggleTurretInteractions()
        {
            Plugin.SelfSettings.CustomInteractTurret.Value = !Plugin.SelfSettings.CustomInteractTurret.Value;
            RefreshAll();
        }

        public static void ToggleBigDoorInteractions()
        {
            Plugin.SelfSettings.CustomInteractDoor.Value = !Plugin.SelfSettings.CustomInteractDoor.Value;
            RefreshAll();
        }

        public static void ToggleLandMineInteractions()
        {
            Plugin.SelfSettings.CustomInteractMine.Value = !Plugin.SelfSettings.CustomInteractMine.Value;
            RefreshAll();
        }

        public static void ToggleSpikeRoofTrapInteractions()
        {
            Plugin.SelfSettings.CustomInteractSpikeRoofTrap.Value = !Plugin.SelfSettings.CustomInteractSpikeRoofTrap.Value;
            RefreshAll();
        }

        public static void RefreshAll()
        {
            bool master = Plugin.SelfSettings.CustomInteractions.Value;

            // Update Turrets
            foreach (var turret in Turret_Patches.activeTurrets)
            {
                if (turret == null) continue;
                ApplyState(turret.gameObject, master && Plugin.SelfSettings.CustomInteractTurret.Value);
            }

            // Update Big Doors
            foreach (var door in TerminalAccessibleObject_Patches.activeBigDoors)
            {
                if (door == null) continue;
                ApplyState(door.gameObject, master && Plugin.SelfSettings.CustomInteractDoor.Value);
            }

            // Update Landmines
            foreach (var mine in Landmine_Patches.activeMines)
            {
                if (mine == null) continue;
                ApplyState(mine.gameObject, master && Plugin.SelfSettings.CustomInteractMine.Value);
            }

            // Update Spike Traps
            foreach (var spike in SpikeRoofTrap_Patches.activeSpikeRoofTraps)
            {
                if (spike == null) continue;
                ApplyState(spike.gameObject, master && Plugin.SelfSettings.CustomInteractSpikeRoofTrap.Value);
            }
        }

        private static void ApplyState(GameObject obj, bool shouldBeActive)
        {
            InteractTrigger trigger = obj.GetComponentInChildren<InteractTrigger>();
            if (trigger != null)
            {
                trigger.interactable = shouldBeActive;
                trigger.gameObject.tag = shouldBeActive ? "InteractTrigger" : "Untagged";
            }
        }
        private static Sprite handIcon;
        public static void SetupTrigger(GameObject obj, string tip, Action<PlayerControllerB> interactAction, bool isTurret = false, bool isMine = false, bool isDoor = false, bool isSpikeRoofTrap = false)
        {
            if (obj == null) return;

            var trigger = obj.GetComponent<InteractTrigger>() ?? obj.AddComponent<InteractTrigger>();

            if(handIcon == null) 
                handIcon = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(s => s.name == "HandIcon");
            trigger.hoverIcon = handIcon;
            trigger.hoverTip = tip;
            trigger.interactCooldown = false;

            trigger.onInteract = new InteractEvent();
            trigger.onInteract.AddListener(new UnityAction<PlayerControllerB>(interactAction));

            bool master = Plugin.SelfSettings.CustomInteractions.Value;
            bool subEnabled = true;

            if (isTurret) subEnabled = Plugin.SelfSettings.CustomInteractTurret.Value;
            else if (isMine) subEnabled = Plugin.SelfSettings.CustomInteractMine.Value;
            else if (isDoor) subEnabled = Plugin.SelfSettings.CustomInteractDoor.Value;
            else if (isSpikeRoofTrap) subEnabled = Plugin.SelfSettings.CustomInteractSpikeRoofTrap.Value;
            bool finalState = master && subEnabled;
            ApplyState(obj, finalState);
        }
    }
}