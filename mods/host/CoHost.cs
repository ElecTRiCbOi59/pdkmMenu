using GameNetcodeStuff;
using HarmonyLib;
using pdkmMenu;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace pdkmMenu
{
    [HarmonyPatch(typeof(HUDManager), "Start")]
    public class NetworkInitPatch
    {
        [HarmonyPostfix]
        static void InitModNetworking()
        {
            if (CoHostHandler.Instance != null)
            {
                CoHostHandler.Instance.StartRegistrationRoutine();
                return;
            }

            GameObject modObj = new GameObject("pdkm_CoHostHandler");
            modObj.AddComponent<CoHostHandler>();
            UnityEngine.Object.DontDestroyOnLoad(modObj);
        }
    }

    public class CoHostHandler : MonoBehaviour
    {
        public static CoHostHandler Instance { get; private set; }
        private const string MSG_COHOST_GRANT = "pdkm_CoHostGrant";
        private const string MSG_COHOST_REQ = "pdkm_CoHostRequest";

        public bool IsCoHost { get; private set; }
        public Dictionary<ulong, string> VerifiedTokens = new Dictionary<ulong, string>();
        private string _myLocalToken = "";

        private bool _isRegistered;
        private CustomMessagingManager _messenger => NetworkManager.Singleton?.CustomMessagingManager;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start() { StartCoroutine(WaitForNetworkManager()); }

        private IEnumerator WaitForNetworkManager()
        {
            yield return new WaitUntil(() => NetworkManager.Singleton != null);

            // Subscribe to both Connect and Disconnect
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            if (NetworkManager.Singleton.IsListening) StartRegistrationRoutine();
        }

        private void OnClientConnected(ulong id)
        {
            if (id == NetworkManager.Singleton.LocalClientId) StartRegistrationRoutine();
        }

        private void OnClientDisconnected(ulong id)
        {
            // CLEANUP: Prevent new players from inheriting old tokens
            if (NetworkManager.Singleton.IsServer && VerifiedTokens.ContainsKey(id))
            {
                VerifiedTokens.Remove(id);
                Log($"Cleaned up token for client {id}");
            }
        }

        public void StartRegistrationRoutine()
        {
            _isRegistered = false;
            IsCoHost = false;
            StopAllCoroutines();
            StartCoroutine(RegistrationRoutine());
        }

        private IEnumerator RegistrationRoutine()
        {
            yield return new WaitUntil(() => NetworkManager.Singleton.IsListening);
            yield return new WaitUntil(() => _messenger != null);
            RegisterReceivers();
        }

        private void RegisterReceivers()
        {
            if (_isRegistered) return; // Prevent double-registration crashes
            _isRegistered = true;

            _messenger.RegisterNamedMessageHandler(MSG_COHOST_REQ, (senderId, reader) =>
            {
                if (senderId == NetworkManager.ServerClientId) return;

                reader.ReadValueSafe(out string sentToken);
                reader.ReadValueSafe(out string action);

                if (VerifiedTokens.TryGetValue(senderId, out string correctToken) && sentToken == correctToken)
                {
                    Log($"VALID: {senderId} {action} {sentToken}.");
                    CommandProcessor.Execute(action);
                }
                else
                {
                    Log($"UNAUTHORIZED: {senderId} sent invalid token | {correctToken}.", true);
                }
            });

            _messenger.RegisterNamedMessageHandler(MSG_COHOST_GRANT, (senderId, reader) =>
            {
                reader.ReadValueSafe(out bool status);
                reader.ReadValueSafe(out string receivedToken);
                IsCoHost = status;
                _myLocalToken = receivedToken;
                HUDManager.Instance?.DisplayTip("MOD INFO", status ? "Co-Host Gained!" : "Co-Host Lost.");
            });

        }

        public void SetCoHost(ulong clientId, bool toggle)
        {
            if (!NetworkManager.Singleton.IsServer) return;

            string newToken = toggle ? Guid.NewGuid().ToString() : "";

            if (toggle) VerifiedTokens[clientId] = newToken;
            else VerifiedTokens.Remove(clientId);

            using var writer = new FastBufferWriter(128, Allocator.Temp);
            writer.WriteValueSafe(toggle);
            writer.WriteValueSafe(newToken);
            _messenger.SendNamedMessage(MSG_COHOST_GRANT, clientId, writer);
        }

        public void RequestAction(string actionName)
        {
            if (NetworkManager.Singleton.IsServer) { CommandProcessor.Execute(actionName); return; }
            if (!IsCoHost || _messenger == null) return;

            using var writer = new FastBufferWriter(256, Allocator.Temp);
            writer.WriteValueSafe(_myLocalToken);
            writer.WriteValueSafe(actionName);
            _messenger.SendNamedMessage(MSG_COHOST_REQ, NetworkManager.ServerClientId, writer);
        }

        private void Log(string msg, bool isError = false)
        {
            if (isError) 
                Debug.LogError($"[pdkm] {msg}");
            else Debug.Log($"[pdkm] {msg}");
        }

        void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }
        }
    }
}
static class CommandProcessor
{
    private delegate void CommandHandler(string[] cmdArgs, PlayerControllerB target);

    private static readonly Dictionary<string, CommandHandler> Handlers = new(StringComparer.OrdinalIgnoreCase)
    {
        ["credits"] = HandleCredits,
        ["setquota"] = HandleSetQuota,
        ["revive"] = HandleRevive,
        ["godmode"] = HandleGodMode,
        ["testroom"] = HandleTestRoom,
        ["eject"] = HandleEject,
        ["resetlevel"] = HandleResetLevel,
        ["killall"] = HandleKillAll,
        ["despawnall"] = HandleDespawnAll,
        ["despawnallprops"] = HandleDespawnAllProps,
        ["spawnallmobs"] = HandleSpawnAllMobs,
        ["spawnallitems"] = HandleSpawnAllItems,
        ["heal"] = HandleHeal,
        ["limp"] = HandleLimp,
        ["nojet"] = HandleNoJet,
        ["lightning"] = HandleLightning,
        ["legsfx"] = HandleLegsFx,
        ["jumpsfx"] = HandleJumpSfx,
        ["meteor"] = HandleMeteor,
        ["spawnmob"] = HandleSpawnMob,
        ["spawnobj"] = HandleSpawnObject,
        ["spawnitem"] = HandleSpawnItem
    };

    public static void Execute(string action)
    {
        if (string.IsNullOrEmpty(action)) return;

        string[] parts = action.Split(':');
        string[] cmdArgs = parts[0].Split('|');
        if (cmdArgs.Length == 0 || string.IsNullOrWhiteSpace(cmdArgs[0])) return;

        string cmd = cmdArgs[0];

        PlayerControllerB target = null;
        if (parts.Length > 1 && ulong.TryParse(parts[1], out ulong id))
            target = StartOfRound.Instance.allPlayerScripts.FirstOrDefault(p => p.actualClientId == id);

        target ??= StartOfRound.Instance.localPlayerController;

        if (!Handlers.TryGetValue(cmd, out CommandHandler handler))
        {
            Debug.LogWarning($"[pdkm] Unknown co-host command '{cmd}'.");
            return;
        }

        try
        {
            handler(cmdArgs, target);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[pdkm] Co-host command '{cmd}' failed and was skipped: {ex}");
        }
    }

    private static void HandleCredits(string[] cmdArgs, PlayerControllerB target)
    {
        if (TryGetIntArg(cmdArgs, 1, out int creds))
            WorldMods.SetCredits(creds);
    }

    private static void HandleSetQuota(string[] cmdArgs, PlayerControllerB target)
    {
        if (TryGetIntArg(cmdArgs, 1, out int quota) && TryGetIntArg(cmdArgs, 2, out int deadline))
            WorldMods.SetQuota(quota, deadline);
    }

    private static void HandleRevive(string[] cmdArgs, PlayerControllerB target)
    {
        StartOfRound.Instance.Debug_ReviveAllPlayersClientRpc();
        StartOfRound.Instance.ReviveDeadPlayers();
    }

    private static void HandleGodMode(string[] cmdArgs, PlayerControllerB target) => WorldMods.GiveFriendsGodMode();
    private static void HandleTestRoom(string[] cmdArgs, PlayerControllerB target) => WorldMods.EnableTestRoom();
    private static void HandleEject(string[] cmdArgs, PlayerControllerB target) => StartOfRound.Instance.ManuallyEjectPlayersServerRpc();
    private static void HandleResetLevel(string[] cmdArgs, PlayerControllerB target) => WorldMods.ResetLevel();
    private static void HandleKillAll(string[] cmdArgs, PlayerControllerB target) => WorldMods.KillAllMobs();
    private static void HandleDespawnAll(string[] cmdArgs, PlayerControllerB target) => SpawnHelper.DespawnObjects();
    private static void HandleDespawnAllProps(string[] cmdArgs, PlayerControllerB target) => RoundManager.Instance.DespawnPropsAtEndOfRound();
    private static void HandleSpawnAllMobs(string[] cmdArgs, PlayerControllerB target) => SpawnHelper.SpawnAllEnemiesInGrid(target);
    private static void HandleSpawnAllItems(string[] cmdArgs, PlayerControllerB target) => SpawnHelper.SpawnAllItemsInGrid(target);
    private static void HandleHeal(string[] cmdArgs, PlayerControllerB target) => target?.MakeCriticallyInjured(false);
    private static void HandleLimp(string[] cmdArgs, PlayerControllerB target) => target?.MakeCriticallyInjured(true);
    private static void HandleNoJet(string[] cmdArgs, PlayerControllerB target) => target?.DisableJetpackModeClientRpc();

    private static void HandleLightning(string[] cmdArgs, PlayerControllerB target)
    {
        if (target == null) return;

        RoundManager.Instance.LightningStrikeClientRpc(target.transform.position);
        RoundManager.Instance.LightningStrikeServerRpc(target.transform.position);
    }

    private static void HandleLegsFx(string[] cmdArgs, PlayerControllerB target) => target?.BreakLegsSFXClientRpc();
    private static void HandleJumpSfx(string[] cmdArgs, PlayerControllerB target) => target?.PlayerJumpedClientRpc();
    private static void HandleMeteor(string[] cmdArgs, PlayerControllerB target) => MeteorTarget.TargetPlayerWithPrediction(target);

    private static void HandleSpawnMob(string[] cmdArgs, PlayerControllerB target)
    {
        if (!TryGetIntArg(cmdArgs, 1, out int mobIndex)) return;

        bool isNear = cmdArgs.Length <= 2 || !string.Equals(cmdArgs[2], "there", StringComparison.OrdinalIgnoreCase);
        SpawnHelper.SpawnEnemy(mobIndex, target, isNear);
    }

    private static void HandleSpawnObject(string[] cmdArgs, PlayerControllerB target)
    {
        if (target == null || !TryGetIntArg(cmdArgs, 1, out int objectIndex)) return;

        SpawnHelper.SpawnObject(objectIndex, target.transform.position);
    }

    private static void HandleSpawnItem(string[] cmdArgs, PlayerControllerB target)
    {
        if (target == null || cmdArgs.Length <= 2) return;
        if (TryGetIntArg(cmdArgs, 2, out int itemId))
            Clicked(cmdArgs[1], itemId, target);
    }

    private static bool TryGetIntArg(string[] args, int index, out int value)
    {
        value = default;
        return args.Length > index && int.TryParse(args[index], out value);
    }

    private static void Clicked(string itemName, int itemId, PlayerControllerB target)

    {
        var filtered = StartOfRound.Instance.allItemsList.itemsList;

        Item item = filtered.FirstOrDefault(i => i != null && i.itemName == itemName && i.itemId == itemId);
        if (item.spawnPrefab == null) return;
        GameObject gameObject = UnityEngine.Object.Instantiate(item.spawnPrefab, target.transform.position, Quaternion.identity, StartOfRound.Instance.propsContainer);

        gameObject.GetComponent<GrabbableObject>().fallTime = 0f;

        int scrap_value = (int)(UnityEngine.Random.Range(item.minValue + 25, item.maxValue + 35) * RoundManager.Instance.scrapValueMultiplier);

        gameObject.GetComponent<GrabbableObject>().SetScrapValue(scrap_value);

        gameObject.GetComponent<NetworkObject>().Spawn(false);

    }
}
