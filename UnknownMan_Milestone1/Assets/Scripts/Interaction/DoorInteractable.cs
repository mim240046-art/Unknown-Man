using UnityEngine;
using UnknownMan.UI;

namespace UnknownMan.Player
{
    public sealed class DoorInteractable : Interactable
    {
        [SerializeField] private float openAngle = 78f;
        private bool open;
        private Quaternion closedRotation;

        private void Awake()
        {
            closedRotation = transform.rotation;
        }

        public override string GetPrompt() => open ? "Close door [E]" : "Open door [E]";

        public override void Interact()
        {
            open = !open;
            StopAllCoroutines();
            StartCoroutine(AnimateDoor());
        }

        private System.Collections.IEnumerator AnimateDoor()
        {
            Quaternion target = open ? closedRotation * Quaternion.Euler(0f, openAngle, 0f) : closedRotation;
            float t = 0f;
            Quaternion start = transform.rotation;
            while (t < 1f)
            {
                t += Time.deltaTime * 4f;
                transform.rotation = Quaternion.Slerp(start, target, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
            transform.rotation = target;
            HUDController.ShowMessage(open ? "The room is open." : "The door is closed.");
        }
    }
}
