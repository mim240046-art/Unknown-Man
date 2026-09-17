using UnityEngine;
using UnknownMan.Core;
using UnknownMan.UI;

namespace UnknownMan.Player
{
    public static class PlayerFactory
    {
        public static GameObject Create()
        {
            var existing = GameObject.Find("Player");
            if (existing != null) return existing;

            var player = new GameObject("Player");
            player.transform.position = new Vector3(0f, 1.15f, -10f);

            var controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.32f;
            controller.center = new Vector3(0f, 0.9f, 0f);
            controller.stepOffset = 0.25f;
            controller.slopeLimit = 50f;

            var firstPerson = player.AddComponent<FirstPersonController>();

            var cameraObject = new GameObject("PlayerCamera");
            cameraObject.transform.SetParent(player.transform);
            cameraObject.transform.localPosition = new Vector3(0f, 1.55f, 0f);
            cameraObject.transform.localRotation = Quaternion.identity;
            var camera = cameraObject.AddComponent<Camera>();
            camera.fieldOfView = 72f;
            camera.nearClipPlane = 0.03f;
            camera.farClipPlane = 160f;
            cameraObject.AddComponent<AudioListener>();
            firstPerson.AssignCamera(camera.transform, camera);

            var flashlight = new GameObject("Flashlight");
            flashlight.transform.SetParent(cameraObject.transform);
            flashlight.transform.localPosition = new Vector3(0.12f, -0.08f, 0.28f);
            flashlight.transform.localRotation = Quaternion.Euler(4f, 0f, 0f);
            var light = flashlight.AddComponent<Light>();
            light.type = LightType.Spot;
            light.range = 32f;
            light.spotAngle = 60f;
            light.intensity = 3.2f;
            light.shadows = LightShadows.Soft;

            var interaction = cameraObject.AddComponent<InteractionRaycaster>();
            interaction.SetCamera(camera);
            firstPerson.AssignInteraction(interaction);

            return player;
        }
    }
}
