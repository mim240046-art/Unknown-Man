using UnityEngine;
using UnityEngine.InputSystem;

namespace UnknownMan.Core
{
    public sealed class PerformanceManager : MonoBehaviour
    {
        private static PerformanceManager instance;
        private float sampleTimer;
        private float fps;

        public static void Create()
        {
            if (instance != null) return;
            var go = new GameObject("PerformanceManager");
            instance = go.AddComponent<PerformanceManager>();
            DontDestroyOnLoad(go);
#if !DEVELOPMENT_BUILD && !UNITY_EDITOR
            instance.enabled = false;
#endif
        }

        private void Update()
        {
            sampleTimer += Time.unscaledDeltaTime;
            if (sampleTimer >= 0.25f)
            {
                fps = 1f / Mathf.Max(0.0001f, Time.unscaledDeltaTime);
                sampleTimer = 0f;
            }

            if (KeyboardPressed(KeyCode.F8))
            {
                enabled = !enabled;
            }
        }

        private void OnGUI()
        {
            if (!enabled) return;
            GUI.color = new Color(1f, 1f, 1f, 0.9f);
            var memoryMb = System.GC.GetTotalMemory(false) / (1024f * 1024f);
            GUI.Label(new Rect(12, 12, 380, 110),
                $"DEV PERFORMANCE\nFPS: {fps:0}\nFrame: {(1000f / Mathf.Max(1f, fps)):0.0} ms\nManaged Memory: {memoryMb:0} MB\nLoaded Objects: {FindObjectsByType<GameObject>(FindObjectsSortMode.None).Length}");
        }

        private static bool KeyboardPressed(KeyCode key)
        {
            return key == KeyCode.F8 && Keyboard.current != null && Keyboard.current.f8Key.wasPressedThisFrame;
        }
    }
}
