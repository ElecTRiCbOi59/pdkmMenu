using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

namespace pdkmMenu
{
    public class TargetExit
    {
        public Vector3 position;
        public Color color;
        public string name;
        public string type;
    }

    public static class PathGuideLogic
    {
        public static List<TargetExit> FindAllExits(bool isInsideFactory)
        {
            var targets = new List<TargetExit>();
            var allEntrances = EntranceTeleport_Patches.activeEntranceTeleports;

            int fireExitCount = 0;
            foreach (var door in allEntrances)
            {
                if (door == null || !door.gameObject.activeInHierarchy) continue;

                // Match doors based on where the player currently is
                if (door.isEntranceToBuilding == !isInsideFactory)
                {
                    bool isMain = door.entranceId == 0;

                    // Logic: Main is Cyan, Fire Exits shift colors
                    float hue = isMain ? 0.5f : (0.08f + (fireExitCount++ * 0.12f)) % 1.0f;

                    targets.Add(new TargetExit
                    {
                        position = door.entrancePoint.position,
                        color = isMain ? Color.cyan : Color.HSVToRGB(hue, 1f, 1f),
                        name = isMain ? "Main Entrance" : $"Fire Exit {door.entranceId}",
                        type = isMain ? "Main Entrance" : "Fire Exit"
                    });
                }
            }

            // Ship Logic (Only relevant when outside)
            if (!isInsideFactory && StartOfRound.Instance?.shipHasLanded == true)
            {
                targets.Add(new TargetExit
                {
                    position = StartOfRound.Instance.shipLandingPosition.position,
                    color = Color.green,
                    name = "Ship",
                    type = "Ship"
                });
            }

            return targets;
        }
    }

    public class WorldSpacePathGuide : MonoBehaviour
    {
        public static WorldSpacePathGuide Instance { get; private set; }

        public float verticalOffset = 0.15f;
        public float updateInterval = 1.0f; // Faster update since we use a cached list now
        public Material pathMaterial;

        private Dictionary<string, LineRenderer> activePaths = new Dictionary<string, LineRenderer>();
        private float nextUpdate;

        void Awake()
        {
            if (Instance) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            var player = StartOfRound.Instance?.localPlayerController;

            // Global toggle or player dead/null
            if (Plugin.SelfSettings?.EnablePathGuide.Value != true || player == null)
            {
                DisableAll();
                return;
            }

            if (Time.time >= nextUpdate)
            {
                CalculatePaths(player);
                nextUpdate = Time.time + updateInterval;
            }
            else
            {
                UpdateStartPoints(player.transform.position);
            }
        }

        private void UpdateStartPoints(Vector3 pos)
        {
            foreach (var lr in activePaths.Values)
            {
                if (lr != null && lr.enabled && lr.positionCount > 0)
                    lr.SetPosition(0, pos + Vector3.up * verticalOffset);
            }
        }

        void CalculatePaths(GameNetcodeStuff.PlayerControllerB player)
        {
            var targets = PathGuideLogic.FindAllExits(player.isInsideFactory).Where(t =>
                (t.type == "Main Entrance" && Plugin.SelfSettings.DisplayMainDoorPath.Value) ||
                (t.type == "Fire Exit" && Plugin.SelfSettings.DisplaySideDoorsPath.Value) ||
                (t.type == "Ship" && Plugin.SelfSettings.DisplayShipPath.Value)).ToList();

            // Identify which paths to remove (using Name as the unique key)
            var currentTargetNames = targets.Select(t => t.name).ToList();
            var keysToRemove = activePaths.Keys.Where(k => !currentTargetNames.Contains(k)).ToList();

            foreach (var key in keysToRemove)
            {
                if (activePaths[key] != null)
                {
                    if (activePaths[key].material) Destroy(activePaths[key].material);
                    Destroy(activePaths[key].gameObject);
                }
                activePaths.Remove(key);
            }

            // Update or Create paths
            foreach (var target in targets)
            {
                if (!activePaths.TryGetValue(target.name, out var lr))
                {
                    var obj = new GameObject($"Path_{target.name}");
                    obj.transform.SetParent(this.transform);
                    lr = obj.AddComponent<LineRenderer>();
                    SetupLR(lr, target.color);
                    activePaths[target.name] = lr;
                }

                NavMeshPath path = new NavMeshPath();
                Vector3 startPos = player.transform.position;

                // Snap start to NavMesh
                if (NavMesh.SamplePosition(startPos, out var hit, 10f, NavMesh.AllAreas)) startPos = hit.position;

                if (NavMesh.CalculatePath(startPos, target.position, NavMesh.AllAreas, path))
                {
                    lr.enabled = true;
                    lr.positionCount = path.corners.Length;
                    for (int i = 0; i < path.corners.Length; i++)
                        lr.SetPosition(i, path.corners[i] + Vector3.up * verticalOffset);
                }
                else
                {
                    // Direct line fallback if NavMesh fails
                    lr.enabled = true;
                    lr.positionCount = 2;
                    lr.SetPosition(0, startPos + Vector3.up * verticalOffset);
                    lr.SetPosition(1, target.position + Vector3.up * verticalOffset);
                }
            }
        }

        void SetupLR(LineRenderer lr, Color c)
        {
            // Create instance to prevent leaking or cross-coloring
            Material baseMat = pathMaterial ? pathMaterial : new Material(Shader.Find("Sprites/Default"));
            lr.material = new Material(baseMat) { color = c };

            lr.startWidth = 0.08f;
            lr.endWidth = 0.04f;
            lr.useWorldSpace = true;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        void DisableAll()
        {
            foreach (var lr in activePaths.Values) if (lr != null) lr.enabled = false;
        }

        private void OnDestroy()
        {
            foreach (var lr in activePaths.Values)
            {
                if (lr != null && lr.material) Destroy(lr.material);
            }
        }
    }

    public static class PathGuideBootstrapper
    {
        public static void Initialize()
        {
            if (WorldSpacePathGuide.Instance != null) return;
            new GameObject("PathGuideSystem").AddComponent<WorldSpacePathGuide>();
        }
    }
}