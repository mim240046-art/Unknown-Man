using UnityEngine;
using UnityEngine.InputSystem;
using UnknownMan.Core;
using UnknownMan.UI;

namespace UnknownMan.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class FirstPersonController : MonoBehaviour
    {
        private CharacterController controller;
        private Transform cameraRoot;
        private Camera playerCamera;
        private InteractionRaycaster interaction;
        private float pitch;
        private float verticalVelocity;
        private bool crouched;
        private float baseHeight;
        private Vector3 cameraBaseLocalPosition;

        [SerializeField] private float walkSpeed = 3.1f;
        [SerializeField] private float sprintSpeed = 5.2f;
        [SerializeField] private float crouchSpeed = 1.7f;
        [SerializeField] private float mouseSensitivity = 0.065f;
        [SerializeField] private float gravity = -18f;

        public void AssignCamera(Transform cameraTransform, Camera camera)
        {
            cameraRoot = cameraTransform;
            playerCamera = camera;
            cameraBaseLocalPosition = cameraRoot.localPosition;
        }

        public void AssignInteraction(InteractionRaycaster raycaster)
        {
            interaction = raycaster;
        }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            baseHeight = controller.height;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            HandleLook();
            HandleMovement();

            if (InputButtonPressed("Interact"))
                interaction?.TryInteract();
            if (InputButtonPressed("Crouch"))
                SetCrouched(!crouched);
            if (InputButtonPressed("Pause"))
                HUDController.TogglePause();
            if (InputButtonPressed("Settings"))
                HUDController.ToggleSettings();
            if (InputButtonPressed("Inventory"))
                HUDController.ToggleInventory();
            if (InputButtonPressed("Journal"))
                HUDController.ToggleJournal();

            if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
                GraphicsSettingsController.CyclePreset();

            var interactionPrompt = interaction?.GetCurrentPrompt();
            HUDController.SetPrompt(interactionPrompt);
        }

        private void HandleLook()
        {
            if (Cursor.lockState != CursorLockMode.Locked && !MobileControlsUI.Active)
                return;

            Vector2 look = MobileControlsUI.Look;
            if (Mouse.current != null && Cursor.lockState == CursorLockMode.Locked)
                look += Mouse.current.delta.ReadValue() * mouseSensitivity;
            if (Gamepad.current != null)
                look += Gamepad.current.rightStick.ReadValue() * 3.0f;

            look *= GraphicsSettingsController.LookSensitivity;
            transform.Rotate(Vector3.up, look.x, Space.Self);
            pitch = Mathf.Clamp(pitch - look.y, -84f, 84f);
            cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private void HandleMovement()
        {
            Vector2 move = MobileControlsUI.Move;
            if (Keyboard.current != null)
            {
                var keyboardMove = Vector2.zero;
                if (Keyboard.current.wKey.isPressed) keyboardMove.y += 1f;
                if (Keyboard.current.sKey.isPressed) keyboardMove.y -= 1f;
                if (Keyboard.current.aKey.isPressed) keyboardMove.x -= 1f;
                if (Keyboard.current.dKey.isPressed) keyboardMove.x += 1f;
                move = keyboardMove.sqrMagnitude > 0.001f ? keyboardMove.normalized : move;
            }
            if (Gamepad.current != null)
                move += Gamepad.current.leftStick.ReadValue();

            bool sprint = (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed) || MobileControlsUI.SprintHeld;
            float speed = crouched ? crouchSpeed : (sprint ? sprintSpeed : walkSpeed);
            Vector3 desired = transform.TransformDirection(new Vector3(move.x, 0f, move.y));
            desired *= speed;

            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;
            verticalVelocity += gravity * Time.deltaTime;
            desired.y = verticalVelocity;
            controller.Move(desired * Time.deltaTime);

            float targetHeight = crouched ? baseHeight * 0.65f : baseHeight;
            controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * 12f);
            controller.center = new Vector3(0f, controller.height * 0.5f, 0f);
            float targetCamY = crouched ? cameraBaseLocalPosition.y - 0.45f : cameraBaseLocalPosition.y;
            var camPos = cameraRoot.localPosition;
            camPos.y = Mathf.Lerp(camPos.y, targetCamY, Time.deltaTime * 12f);
            cameraRoot.localPosition = camPos;
        }

        private void SetCrouched(bool value)
        {
            crouched = value;
        }

        private static bool InputButtonPressed(string action)
        {
            if (action == "Interact")
                return (Keyboard.current?.eKey.wasPressedThisFrame ?? false) || MobileControlsUI.InteractPressed || (Gamepad.current?.buttonWest.wasPressedThisFrame ?? false);
            if (action == "Crouch")
                return (Keyboard.current?.cKey.wasPressedThisFrame ?? false) || MobileControlsUI.CrouchPressed || (Gamepad.current?.buttonEast.wasPressedThisFrame ?? false);
            if (action == "Pause")
                return (Keyboard.current?.escapeKey.wasPressedThisFrame ?? false) || MobileControlsUI.PausePressed;
            if (action == "Settings")
                return Keyboard.current?.f3Key.wasPressedThisFrame ?? false;
            if (action == "Inventory")
                return Keyboard.current?.iKey.wasPressedThisFrame ?? false || MobileControlsUI.InventoryPressed;
            if (action == "Journal")
                return Keyboard.current?.jKey.wasPressedThisFrame ?? false || MobileControlsUI.JournalPressed;
            return false;
        }
    }
}
