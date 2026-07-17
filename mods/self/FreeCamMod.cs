using System;
using UnityEngine;
using UnityEngine.InputSystem;
using GameNetcodeStuff;

namespace pdkmMenu
{
    public class FreeCamMod
    {
        private static CharacterController cachedController = null;
        private static PlayerControllerB lastPlayer = null;

        private static GameObject FreeCamHolder = null;
        public static Camera FreeCamera = null;
        private static Vector3 rotateValue = Vector3.zero;
        public static bool _isFreeCam = false;

        public static bool IsFreeCam
        {
            get => _isFreeCam;
            set
            {
                if (_isFreeCam == value) return;
                _isFreeCam = value;

                if (_isFreeCam)
                {
                    NoClipMod.IsNoClip = false;
                    EnableFreeCam();
                }
                else
                {
                    DisableFreeCam();
                }
            }
        }

        public static void UpdateMod()
        {
            var player = GameNetworkManager.Instance?.localPlayerController;
            if (player == null) return;

            if (player != lastPlayer)
            {
                lastPlayer = player;
                cachedController = player.GetComponent<CharacterController>();
            }

            if (FreeCamHolder == null) CreateFreeCam();

            if (IsFreeCam)
            {
                UpdateFreeCam();
            }
        }

        private static void CreateFreeCam()
        {
            FreeCamHolder = new GameObject("FreeCamHolder");
            UnityEngine.Object.DontDestroyOnLoad(FreeCamHolder); // Keep across levels

            FreeCamera = FreeCamHolder.AddComponent<Camera>();
            FreeCamera.enabled = false;

            FreeCamera.cullingMask = 20649983;
            Plugin.Logger.LogInfo("FreeCam Camera Created");
        }

        private static void UpdateFreeCam()
        {
            if (FreeCamera == null) return;

            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard == null || mouse == null) return;

            float rotationSpeed = 0.15f; // Adjusted for better feel
            Vector2 mouseDelta = mouse.delta.ReadValue();

            rotateValue.x -= mouseDelta.y * rotationSpeed;
            rotateValue.y += mouseDelta.x * rotationSpeed;
            rotateValue.x = Mathf.Clamp(rotateValue.x, -89f, 89f);

            FreeCamera.transform.eulerAngles = rotateValue;

            float speed = keyboard.leftShiftKey.isPressed ? 40f : 20f;
            Vector3 moveDir = Vector3.zero;

            if (keyboard.wKey.isPressed) moveDir += FreeCamera.transform.forward;
            if (keyboard.sKey.isPressed) moveDir -= FreeCamera.transform.forward;
            if (keyboard.aKey.isPressed) moveDir -= FreeCamera.transform.right;
            if (keyboard.dKey.isPressed) moveDir += FreeCamera.transform.right;
            if (keyboard.spaceKey.isPressed) moveDir += Vector3.up;
            if (keyboard.leftCtrlKey.isPressed) moveDir += Vector3.down;

            if (moveDir != Vector3.zero)
            {
                FreeCamera.transform.position += moveDir.normalized * (speed * Time.deltaTime);
            }
        }

        private static void EnableFreeCam()
        {
            if (lastPlayer == null || cachedController == null) return;

            cachedController.enabled = false;

            if (FreeCamera != null)
            {
                FreeCamera.enabled = true;
                FreeCamera.transform.position = lastPlayer.gameplayCamera.transform.position;
                FreeCamera.transform.rotation = lastPlayer.gameplayCamera.transform.rotation;

                rotateValue = FreeCamera.transform.eulerAngles;
            }
        }

        public static void DisableFreeCam()
        {
            if (cachedController != null)
                cachedController.enabled = true;

            if (FreeCamera != null)
                FreeCamera.enabled = false;
        }
    }
}