using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnknownMan.Core;
using UnknownMan.Inventory;

namespace UnknownMan.UI
{
    public sealed class HUDController : MonoBehaviour
    {
        private static HUDController instance;
        private Text promptText;
        private Text messageText;
        private Text statusText;
        private GameObject panel;
        private GameObject inventoryPanel;
        private GameObject journalPanel;
        private GameObject pausePanel;
        private GameObject settingsPanel;
        private float messageTimer;

        public static void Create()
        {
            if (instance != null) return;
            var go = new GameObject("HUD");
            instance = go.AddComponent<HUDController>();
            instance.Build();
        }

        public static void SetPrompt(string text)
        {
            if (instance != null) instance.promptText.text = string.IsNullOrEmpty(text) ? string.Empty : text;
        }

        public static void ShowMessage(string text)
        {
            if (instance == null) return;
            instance.messageText.text = text;
            instance.messageTimer = 3.0f;
        }

        public static void TogglePause() => instance?.TogglePanel(instance.pausePanel);
        public static void ToggleSettings() => instance?.TogglePanel(instance.settingsPanel);
        public static void ToggleInventory() { if (instance != null) { instance.RefreshInventory(); instance.TogglePanel(instance.inventoryPanel); } }
        public static void ToggleJournal() { if (instance != null) { instance.RefreshJournal(); instance.TogglePanel(instance.journalPanel); } }

        private void Build()
        {
            var canvasGo = new GameObject("Canvas");
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            canvasGo.AddComponent<GraphicRaycaster>();

            EnsureEventSystem();
            MobileControlsUI.Create(canvas);

            promptText = CreateText(canvas.transform, "Prompt", 30, new Vector2(0.5f, 0.17f), new Vector2(0.5f, 0.17f), new Vector2(900f, 70f));
            promptText.alignment = TextAnchor.MiddleCenter;
            messageText = CreateText(canvas.transform, "Message", 26, new Vector2(0.5f, 0.80f), new Vector2(0.5f, 0.80f), new Vector2(1000f, 80f));
            messageText.alignment = TextAnchor.MiddleCenter;
            statusText = CreateText(canvas.transform, "Status", 20, new Vector2(0.02f, 0.95f), new Vector2(0.02f, 0.95f), new Vector2(700f, 55f));
            statusText.alignment = TextAnchor.MiddleLeft;

            panel = CreatePanel(canvas.transform, "PanelRoot");
            inventoryPanel = CreateSimplePanel(canvas.transform, "InventoryPanel", "INVENTORY\n\nI = toggle");
            journalPanel = CreateSimplePanel(canvas.transform, "JournalPanel", "JOURNAL / EVIDENCE\n\nJ = toggle\n\nEvidence is logged automatically.");
            pausePanel = CreateSimplePanel(canvas.transform, "PausePanel", "PAUSED\n\nESC = continue\nF3 = settings");
            settingsPanel = CreateSettingsPanel(canvas.transform);
        }

        private void Update()
        {
            if (messageTimer > 0f)
            {
                messageTimer -= Time.unscaledDeltaTime;
                if (messageTimer <= 0f) messageText.text = string.Empty;
            }
            statusText.text = $"UNKNOWN MAN  •  {GraphicsSettingsController.GetSummary()}  •  G = preset";
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null) return;
            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        private static Text CreateText(Transform parent, string name, int fontSize, Vector2 min, Vector2 max, Vector2 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = min;
            rt.anchorMax = max;
            rt.sizeDelta = size;
            var text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = fontSize;
            text.color = Color.white;
            text.raycastTarget = false;
            return text;
        }

        private static GameObject CreatePanel(Transform parent, string name)
        {
            var panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            var rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(900f, 560f);
            var image = panel.AddComponent<Image>();
            image.sprite = MobileControlsUISprite();
            image.color = new Color(0.02f, 0.025f, 0.03f, 0.95f);
            panel.SetActive(false);
            return panel;
        }

        private static GameObject CreateSimplePanel(Transform parent, string name, string content)
        {
            var panel = CreatePanel(parent, name);
            var text = CreateText(panel.transform, "Text", 32, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(760f, 400f));
            text.alignment = TextAnchor.MiddleCenter;
            text.text = content;
            var button = CreateButton(panel.transform, "CLOSE", new Vector2(0.5f, 0.10f), 220f, 70f);
            button.onClick.AddListener(() => instance?.CloseAllPanels());
            return panel;
        }

        private static GameObject CreateSettingsPanel(Transform parent)
        {
            var panel = CreatePanel(parent, "SettingsPanel");
            var title = CreateText(panel.transform, "Title", 36, new Vector2(0.5f, 0.86f), new Vector2(0.5f, 0.86f), new Vector2(760f, 70f));
            title.text = "GRAPHICS SETTINGS";
            title.alignment = TextAnchor.MiddleCenter;

            var presets = new[] { GraphicsPreset.VeryLow, GraphicsPreset.Low, GraphicsPreset.Medium, GraphicsPreset.High, GraphicsPreset.Ultra };
            for (int i = 0; i < presets.Length; i++)
            {
                int index = i;
                var b = CreateButton(panel.transform, presets[i].ToString(), new Vector2(0.25f + 0.125f * i, 0.68f), 150f, 65f);
                b.onClick.AddListener(() => GraphicsSettingsController.ApplyPreset(presets[index]));
            }

            var render = CreateText(panel.transform, "RenderScale", 25, new Vector2(0.5f, 0.47f), new Vector2(0.5f, 0.47f), new Vector2(700f, 55f));
            render.text = "Render scale: preset-controlled (G cycles presets)";
            render.alignment = TextAnchor.MiddleCenter;

            var info = CreateText(panel.transform, "Info", 20, new Vector2(0.5f, 0.32f), new Vector2(0.5f, 0.32f), new Vector2(700f, 130f));
            info.text = "Core scaling: texture quality, shadows, shadow distance, LOD bias,\nrender scale, fog density, FPS target and view complexity.\nF3 toggles this panel on PC.";
            info.alignment = TextAnchor.MiddleCenter;

            var close = CreateButton(panel.transform, "CLOSE", new Vector2(0.5f, 0.10f), 220f, 70f);
            close.onClick.AddListener(() => instance?.CloseAllPanels());
            return panel;
        }

        private static Button CreateButton(Transform parent, string label, Vector2 anchor, float width, float height)
        {
            var go = new GameObject(label + "Button");
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.sizeDelta = new Vector2(width, height);
            var image = go.AddComponent<Image>();
            image.sprite = MobileControlsUISprite();
            image.color = new Color(1f, 1f, 1f, 0.14f);
            var button = go.AddComponent<Button>();
            var text = CreateText(go.transform, "Label", 21, Vector2.zero, Vector2.one, Vector2.zero);
            text.text = label.ToUpperInvariant();
            text.alignment = TextAnchor.MiddleCenter;
            return button;
        }

        private void CloseAllPanels()
        {
            foreach (var panelObject in new[] { pausePanel, settingsPanel, inventoryPanel, journalPanel })
                if (panelObject != null) panelObject.SetActive(false);
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void TogglePanel(GameObject target)
        {
            if (target == null) return;
            bool active = !target.activeSelf;
            if (active)
            {
                foreach (var other in new[] { pausePanel, settingsPanel, inventoryPanel, journalPanel })
                    if (other != null && other != target) other.SetActive(false);
            }
            target.SetActive(active);
            Time.timeScale = active ? 0f : 1f;
            Cursor.visible = active;
            Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
        }

        private void RefreshInventory()
        {
            var text = inventoryPanel != null ? inventoryPanel.transform.Find("Text")?.GetComponent<Text>() : null;
            if (text == null) return;
            text.text = "INVENTORY\n\n" + (InventorySystem.Items.Count == 0 ? "Empty" : string.Join("\n", InventorySystem.Items));
        }

        private void RefreshJournal()
        {
            var text = journalPanel != null ? journalPanel.transform.Find("Text")?.GetComponent<Text>() : null;
            if (text == null) return;
            var evidence = InventorySystem.Evidence;
            text.text = "JOURNAL / EVIDENCE\n\n" + (evidence.Count == 0 ? "No evidence recorded." : string.Join("\n\n", evidence));
        }

        private static Sprite MobileControlsUISprite()
        {
            var texture = new Texture2D(2, 2);
            texture.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
        }
    }
}
