using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnknownMan.UI
{
    public sealed class TouchJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public Vector2 Value { get; private set; }
        [SerializeField] private RectTransform knob;
        private float radius = 80f;

        public void Configure(RectTransform knobTransform, float joystickRadius)
        {
            knob = knobTransform;
            radius = joystickRadius;
        }

        public void OnPointerDown(PointerEventData eventData) => UpdateValue(eventData);
        public void OnDrag(PointerEventData eventData) => UpdateValue(eventData);
        public void OnPointerUp(PointerEventData eventData)
        {
            Value = Vector2.zero;
            if (knob != null) knob.anchoredPosition = Vector2.zero;
        }

        private void UpdateValue(PointerEventData eventData)
        {
            var rect = (RectTransform)transform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out var local);
            Value = Vector2.ClampMagnitude(local / radius, 1f);
            if (knob != null) knob.anchoredPosition = Value * radius;
        }
    }

    public sealed class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool Held { get; private set; }
        public bool PressedThisFrame { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            Held = true;
            PressedThisFrame = true;
        }

        public void OnPointerUp(PointerEventData eventData) => Held = false;

        private void LateUpdate() => PressedThisFrame = false;
    }

    public static class MobileControlsUI
    {
        private static TouchJoystick moveJoystick;
        private static TouchJoystick lookJoystick;
        private static HoldButton interact;
        private static HoldButton sprint;
        private static HoldButton crouch;
        private static HoldButton inventory;
        private static HoldButton journal;
        private static HoldButton pause;

        public static bool Active { get; private set; }
        public static Vector2 Move => moveJoystick != null ? moveJoystick.Value : Vector2.zero;
        public static Vector2 Look => lookJoystick != null ? lookJoystick.Value * 3.0f : Vector2.zero;
        public static bool SprintHeld => sprint != null && sprint.Held;
        public static bool InteractPressed => TakePressed(interact);
        public static bool CrouchPressed => TakePressed(crouch);
        public static bool InventoryPressed => TakePressed(inventory);
        public static bool JournalPressed => TakePressed(journal);
        public static bool PausePressed => TakePressed(pause);

        public static void Create(Canvas canvas)
        {
            if (moveJoystick != null) return;
            Active = Application.isMobilePlatform;
            var ui = new GameObject("MobileControls");
            ui.transform.SetParent(canvas.transform, false);
            ui.SetActive(Active);

            moveJoystick = CreateJoystick(ui.transform, "MoveJoystick", new Vector2(0.16f, 0.18f));
            lookJoystick = CreateJoystick(ui.transform, "LookJoystick", new Vector2(0.84f, 0.18f));
            interact = CreateButton(ui.transform, "INTERACT", new Vector2(0.67f, 0.34f), new Vector2(0.10f, 0.08f));
            sprint = CreateButton(ui.transform, "SPRINT", new Vector2(0.58f, 0.17f), new Vector2(0.10f, 0.07f));
            crouch = CreateButton(ui.transform, "CROUCH", new Vector2(0.79f, 0.30f), new Vector2(0.10f, 0.07f));
            inventory = CreateButton(ui.transform, "BAG", new Vector2(0.92f, 0.90f), new Vector2(0.08f, 0.06f));
            journal = CreateButton(ui.transform, "JOURNAL", new Vector2(0.82f, 0.90f), new Vector2(0.10f, 0.06f));
            pause = CreateButton(ui.transform, "PAUSE", new Vector2(0.94f, 0.96f), new Vector2(0.08f, 0.06f));
        }

        private static bool TakePressed(HoldButton button)
        {
            return button != null && button.PressedThisFrame;
        }

        private static TouchJoystick CreateJoystick(Transform parent, string name, Vector2 anchor)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            var rt = root.AddComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.sizeDelta = new Vector2(180f, 180f);
            var image = root.AddComponent<Image>();
            image.sprite = MakeWhiteSprite();
            image.color = new Color(1f, 1f, 1f, 0.14f);
            var joystick = root.AddComponent<TouchJoystick>();

            var knob = new GameObject("Knob");
            knob.transform.SetParent(root.transform, false);
            var knobRt = knob.AddComponent<RectTransform>();
            knobRt.sizeDelta = new Vector2(80f, 80f);
            var knobImage = knob.AddComponent<Image>();
            knobImage.sprite = MakeWhiteSprite();
            knobImage.color = new Color(1f, 1f, 1f, 0.30f);
            joystick.Configure(knobRt, 50f);
            return joystick;
        }

        private static HoldButton CreateButton(Transform parent, string text, Vector2 anchor, Vector2 size)
        {
            var root = new GameObject(text);
            root.transform.SetParent(parent, false);
            var rt = root.AddComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.sizeDelta = new Vector2(170f * size.x / 0.1f, 100f * size.y / 0.07f);
            var image = root.AddComponent<Image>();
            image.sprite = MakeWhiteSprite();
            image.color = new Color(1f, 1f, 1f, 0.16f);
            var labelObject = new GameObject("Label");
            labelObject.transform.SetParent(root.transform, false);
            var label = labelObject.AddComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.fontSize = 20;
            var labelRt = label.GetComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.offsetMin = Vector2.zero;
            labelRt.offsetMax = Vector2.zero;
            return root.AddComponent<HoldButton>();
        }

        private static Sprite whiteSprite;
        private static Sprite MakeWhiteSprite()
        {
            if (whiteSprite != null) return whiteSprite;
            var texture = new Texture2D(2, 2);
            texture.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
            texture.Apply();
            whiteSprite = Sprite.Create(texture, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
            return whiteSprite;
        }
    }
}
