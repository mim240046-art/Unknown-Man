using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnknownMan.Inventory;

namespace UnknownMan.Core
{
    [Serializable]
    public sealed class SaveData
    {
        public int version = 1;
        public int day = 1;
        public float[] playerPosition = { 0f, 1.1f, 0f };
        public float playerYaw;
        public List<string> inventory = new();
        public List<string> evidence = new();
        public List<string> unlockedLocations = new() { "VillageEntrance" };
        public string graphicsPreset = "Auto";
        public float renderScale = 1f;
    }

    public static class SaveSystem
    {
        private const string FileName = "save_v1.json";
        private static SaveData current = new();

        public static SaveData Current => current;
        private static string SavePath => Path.Combine(Application.persistentDataPath, FileName);

        public static void Initialize()
        {
            current = new SaveData();
        }

        public static void LoadIntoRuntime()
        {
            if (File.Exists(SavePath))
            {
                try
                {
                    current = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath)) ?? new SaveData();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Save load failed, starting fresh: {ex.Message}");
                    current = new SaveData();
                }
            }

            if (current.inventory == null) current.inventory = new List<string>();
            if (current.evidence == null) current.evidence = new List<string>();
            if (current.unlockedLocations == null) current.unlockedLocations = new List<string>();

            if (!string.IsNullOrEmpty(current.graphicsPreset) && Enum.TryParse(current.graphicsPreset, out GraphicsPreset preset))
                GraphicsSettingsController.ApplyPreset(preset);

            GraphicsSettingsController.SetRenderScale(current.renderScale);
            InventorySystem.Restore(current.inventory, current.evidence);
        }

        public static void ApplySavedPlayerTransform()
        {
            var player = GameObject.Find("Player");
            if (player == null || current.playerPosition == null || current.playerPosition.Length < 3) return;
            player.transform.position = new Vector3(current.playerPosition[0], current.playerPosition[1], current.playerPosition[2]);
            player.transform.rotation = Quaternion.Euler(0f, current.playerYaw, 0f);
        }

        public static void SaveFromRuntime()
        {
            var player = GameObject.Find("Player");
            if (player != null)
            {
                var position = player.transform.position;
                current.playerPosition = new[] { position.x, position.y, position.z };
                current.playerYaw = player.transform.eulerAngles.y;
            }

            current.inventory = InventorySystem.Items;
            current.evidence = InventorySystem.Evidence;
            current.graphicsPreset = GraphicsSettingsController.CurrentPreset.ToString();
            current.renderScale = GraphicsSettingsController.RenderScale;

            Directory.CreateDirectory(Application.persistentDataPath);
            File.WriteAllText(SavePath, JsonUtility.ToJson(current, true));
        }

        public static void ResetSave()
        {
            current = new SaveData();
            InventorySystem.Clear();
            if (File.Exists(SavePath)) File.Delete(SavePath);
        }
    }
}
