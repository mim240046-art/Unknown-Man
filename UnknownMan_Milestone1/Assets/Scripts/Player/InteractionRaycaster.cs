using UnityEngine;
using UnknownMan.UI;

namespace UnknownMan.Player
{
    public sealed class InteractionRaycaster : MonoBehaviour
    {
        private Camera playerCamera;
        private Interactable current;

        public void SetCamera(Camera cam) => playerCamera = cam;

        public string GetCurrentPrompt()
        {
            current = null;
            if (playerCamera == null) return string.Empty;
            var ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out var hit, 3.0f, ~0, QueryTriggerInteraction.Ignore))
            {
                current = hit.collider.GetComponentInParent<Interactable>();
                if (current != null && current.CanInteract())
                    return current.GetPrompt();
            }
            return string.Empty;
        }

        public void TryInteract()
        {
            if (current != null && current.CanInteract())
                current.Interact();
        }
    }
}
