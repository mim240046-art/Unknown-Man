using UnityEngine;
using UnknownMan.Inventory;

namespace UnknownMan.Player
{
    public sealed class EvidenceInteractable : Interactable
    {
        [SerializeField] private string evidenceId = "Diary — Day 1";
        [SerializeField] private string evidenceText = "The diary mentions footsteps outside the house every night.";
        [SerializeField] private string itemId = "Old Diary";
        private bool collected;

        public override string GetPrompt() => collected ? "Evidence recorded" : "Inspect evidence [E]";
        public override bool CanInteract() => !collected;

        public override void Interact()
        {
            if (collected) return;
            collected = true;
            InventorySystem.AddItem(itemId);
            InventorySystem.AddEvidence($"{evidenceId}: {evidenceText}");
            gameObject.SetActive(false);
        }
    }
}
