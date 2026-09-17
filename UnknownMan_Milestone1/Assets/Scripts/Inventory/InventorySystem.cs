using System.Collections.Generic;
using UnityEngine;
using UnknownMan.UI;

namespace UnknownMan.Inventory
{
    public static class InventorySystem
    {
        private static readonly List<string> items = new();
        private static readonly List<string> evidence = new();

        public static List<string> Items => new(items);
        public static List<string> Evidence => new(evidence);

        public static void AddItem(string item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
                HUDController.ShowMessage($"Added: {item}");
            }
        }

        public static void AddEvidence(string clue)
        {
            if (!evidence.Contains(clue))
            {
                evidence.Add(clue);
                HUDController.ShowMessage($"Evidence logged: {clue}");
            }
        }

        public static bool Has(string item) => items.Contains(item);

        public static void Restore(List<string> restoredItems, List<string> restoredEvidence)
        {
            items.Clear();
            evidence.Clear();
            if (restoredItems != null) items.AddRange(restoredItems);
            if (restoredEvidence != null) evidence.AddRange(restoredEvidence);
        }

        public static void Clear()
        {
            items.Clear();
            evidence.Clear();
        }
    }
}
