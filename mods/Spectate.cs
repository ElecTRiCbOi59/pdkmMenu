using GameNetcodeStuff;
using pdkmMenu;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

namespace pdkmMenu
{
    public static class Spectate
    {
        public static Camera clonedCamera;
        public static PlayerControllerB SelectedPlayer;
        public static Camera targetCamera;
        private static RenderTexture miniViewTexture;
        private static GameObject miniViewUI;
        public static GameObject canvasObj;
        private static Canvas canvas;

        private static List<PlayerControllerB> playerControllers;


        public static GameObject HideMeshesForSpectatorObject;

        public static void Initialize()
        {
            // Create RenderTexture for the mini viewport
            float size = 1f;
            miniViewTexture = new RenderTexture((int)(Screen.width* size), (int)(Screen.height* size), 16);

            // Create a UI Canvas for displaying the mini view
            canvasObj = new GameObject("MiniViewCanvas");
            UnityEngine.Object.DontDestroyOnLoad(canvasObj);
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvasObj.SetActive(false);

            // Create UI RawImage to display the mini view
            miniViewUI = new GameObject("MiniViewUI");
            UnityEngine.Object.DontDestroyOnLoad(miniViewUI);
            RectTransform rectTransform = miniViewUI.AddComponent<RectTransform>();
            miniViewUI.transform.SetParent(canvas.transform, false);
            RawImage rawImage = miniViewUI.AddComponent<RawImage>();
            rawImage.texture = miniViewTexture;

            // Position & size of the mini view
            rectTransform.anchorMin = new Vector2(0.8f, 0.8f);  // Bottom-right corner
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            // Create the cloned camera object
            GameObject camObj = new GameObject("MiniViewCamera");
            UnityEngine.Object.DontDestroyOnLoad(camObj);
            clonedCamera = camObj.AddComponent<Camera>();
            //clonedCamera.gameObject.AddComponent<HideMeshesForSpectator>();

            clonedCamera.enabled = true;
            clonedCamera.targetTexture = miniViewTexture;

            // Initialize cameras list
            playerControllers = Plugin.GetRealPlayerScripts();
            if (playerControllers.Count > 0)
            {
                PlayerControllerB firstPlayer = playerControllers[0];
                SwitchCamera(firstPlayer); // Start with the first camera
            }
        }


        public static void Update()
        {

            ////ScavengerModel
            //playerControllers = Plugin.GetRealPlayerScripts();
            ////allCameras = Camera.allCameras;
            //if (playerControllers == null || playerControllers.Length == 0) return;

            //// Move to the next camera if the user presses a key (optional)
            //if (Plugin.SelfSettings.SpectateCycleHotKey.Value.IsDown()) // Press 'Tab' to cycle cameras
            //{
            //    playerIndex = (playerIndex + 1) % playerControllers.Length;
            //    SwitchCamera(playerIndex);
            //}

            // Continuously update the cloned camera's position & rotation
            // If we have a target, keep the cloned camera synced
            if (targetCamera != null && clonedCamera != null)
            {
                clonedCamera.transform.position = targetCamera.transform.position;
                clonedCamera.transform.rotation = targetCamera.transform.rotation;
            }
            // AUTO-RECOVERY: If spectate is ON but we lost our target (e.g., player disconnected)
            else if (canvasObj != null && canvasObj.activeSelf)
            {
                var players = Plugin.GetRealPlayerScripts();
                if (players.Count > 0)
                {
                    var p = players[0];
                    SwitchCamera(p);
                    //clonedCamera.Render();
                }
            }
        }
        public static void ToggleSpectate()
        {
            canvasObj.SetActive(!canvasObj.activeSelf);
        }
        public static void SetViewCamera(Camera Target)
        {
            targetCamera = Target;
            clonedCamera.CopyFrom(targetCamera);
            clonedCamera.targetTexture = miniViewTexture;
            clonedCamera.enabled = true;
            clonedCamera.nearClipPlane = 0.5f;  // Adjust this value to your needs (e.g., 5 units)
            //clonedCamera.farClipPlane = 1000f;  // Example: Set far clip plane distance

        }
        public static void SwitchCamera(PlayerControllerB player)
        {
            if (player == null || player.gameplayCamera == null)
            {
                SelectedPlayer = null;
                targetCamera = null;
                // Don't null the clonedCamera here or you'll destroy the object!
                return;
            }

            if (SelectedPlayer == player)
            {
                ToggleSpectate();
            }
            else
            {
                canvasObj.SetActive(true);
            }

            SelectedPlayer = player;
            SetViewCamera(player.gameplayCamera);
        }
    }
}
public class HideMeshesForSpectator : MonoBehaviour
{

    void Update()
    {

    }
    //void Start()
    //{
    //    Plugin.Logger.LogInfo("HideMeshes started");
    //}

    //void OnPreCull()
    //{
    //    Plugin.Logger.LogInfo("bro");
    //    if (Camera.current == Spectate.clonedCamera)
    //    {
    //        Plugin.Logger.LogInfo("yo");
    //        //Spectate.SelectedPlayer.meshContainer
    //        Spectate.SelectedPlayer.thisPlayerModel.enabled = false;
    //        Spectate.SelectedPlayer.thisPlayerModelArms.enabled = false;
    //        Spectate.SelectedPlayer.thisPlayerModelLOD1.enabled = false;
    //        Spectate.SelectedPlayer.thisPlayerModelLOD2.enabled = false;

    //    }
    //}
    //void OnRenderObject()
    //{
    //    Plugin.Logger.LogInfo("OnRenderObject triggered");

    //    if (Camera.current == Spectate.clonedCamera)
    //    {
    //    }
    //}
    //void OnWillRenderObject()
    //{
    //    Plugin.Logger.LogInfo("OnWillRenderObject triggered for clonedCamera");

    //    if (Camera.current == Spectate.clonedCamera)
    //    {
    //    }
    //}
    //void OnPreRender()
    //{
    //    Plugin.Logger.LogInfo("OnPreRender triggered for clonedCamera");

    //    if (Camera.current == Spectate.clonedCamera)
    //    {
    //    }
    //}

    //void OnPostRender()
    //{
    //    Plugin.Logger.LogInfo("bro2");

    //    if (Camera.current == Spectate.clonedCamera)
    //    {
    //        Plugin.Logger.LogInfo("no");
    //        Spectate.SelectedPlayer.thisPlayerModel.enabled = true;
    //        Spectate.SelectedPlayer.thisPlayerModelArms.enabled = true;
    //        Spectate.SelectedPlayer.thisPlayerModelLOD1.enabled = true;
    //        Spectate.SelectedPlayer.thisPlayerModelLOD2.enabled = true;
    //    }
    //}
}