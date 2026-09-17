using UnityEngine;

namespace UnknownMan.Player
{
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] private string prompt = "Interact";
        public virtual string GetPrompt() => prompt;
        public virtual bool CanInteract() => true;
        public abstract void Interact();
    }
}
