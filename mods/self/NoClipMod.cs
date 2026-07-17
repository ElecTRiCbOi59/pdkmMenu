using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using GameNetcodeStuff;

namespace pdkmMenu
{
    public class NoClipMod
    {
        private static CharacterController MyCharacterController = null;
        private static Rigidbody MyRigidbody = null;
        private static float MyCharacterControllerRadius = 0;
        public static bool _isNoClip = false;

        // Added to track the current player instance
        private static PlayerControllerB lastPlayer = null;

        public static bool IsNoClip
        {
            get => _isNoClip;
            set
            {
                if (_isNoClip == value) return;

                _isNoClip = value;

                if (_isNoClip)
                {
                    FreeCamMod._isFreeCam = false;
                    FreeCamMod.DisableFreeCam();
                    // Setup happens once on toggle
                    EnableNoClip();
                }
                else
                {
                    // Cleanup happens once on toggle
                    DisableNoClip();
                }
            }
        }

        public static void UpdateMod()
        {
            var player = StartOfRound.Instance?.localPlayerController;
            if (player == null) return;

            // Only search for components if the player instance has changed
            if (player != lastPlayer)
            {
                SetChar(player);
            }

            if (IsNoClip)
            {
                // We only need to run the movement logic every frame
                NoClipMove();
            }
        }

        private static void SetChar(PlayerControllerB player)
        {
            lastPlayer = player;
            MyCharacterController = player.GetComponent<CharacterController>();
            MyRigidbody = player.GetComponent<Rigidbody>();

            if (MyCharacterController != null && MyCharacterControllerRadius == 0)
            {
                MyCharacterControllerRadius = MyCharacterController.radius;
            }
        }

        private static void NoClipMove()
        {
            if (MyCharacterController == null || MyRigidbody == null) return;
            HandleMovement();
        }

        private static void HandleMovement()
        {
            if (lastPlayer == null) return;

            float speed = Keyboard.current.leftShiftKey.isPressed ? 40f : 20f;
            Vector3 moveDirection = Vector3.zero;

            if (Keyboard.current.wKey.isPressed) moveDirection += Vector3.forward;
            if (Keyboard.current.sKey.isPressed) moveDirection += Vector3.back;
            if (Keyboard.current.aKey.isPressed) moveDirection += Vector3.left;
            if (Keyboard.current.dKey.isPressed) moveDirection += Vector3.right;
            if (Keyboard.current.spaceKey.isPressed) moveDirection += Vector3.up;
            if (Keyboard.current.leftCtrlKey.isPressed) moveDirection += Vector3.down;

            moveDirection.Normalize();
            moveDirection *= speed * Time.deltaTime;

            lastPlayer.transform.Translate(moveDirection);
        }

        private static void EnableNoClip()
        {
            if (MyCharacterController == null || MyRigidbody == null || lastPlayer == null) return;

            StartOfRound.Instance.allowLocalPlayerDeath = false;
            MyCharacterController.enabled = false;
            lastPlayer.isInHangarShipRoom = false;
            MyRigidbody.useGravity = false;
            MyRigidbody.isKinematic = true;
        }

        public static void DisableNoClip()
        {
            if (MyCharacterController == null || MyRigidbody == null) return;

            MyCharacterController.enabled = true;
            MyCharacterController.radius = MyCharacterControllerRadius;

            // Re-enabling physics handled by the coroutine to maintain your original logic
            StartOfRound.Instance.StartCoroutine(DisableGodMode());
        }

        private static IEnumerator DisableGodMode()
        {
            yield return new WaitForSeconds(2.0f);
            StartOfRound.Instance.allowLocalPlayerDeath = Plugin.SelfSettings.GodMode.Value;
        }
    }
}