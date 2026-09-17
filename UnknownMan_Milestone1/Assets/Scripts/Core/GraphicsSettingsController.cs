using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnknownMan.Core
{
    public enum GraphicsPreset
    {
        Auto,
        VeryLow,
        Low,
        Medium,
        High,
        Ultra
    }

    public static class GraphicsSettingsController
    {
        public static GraphicsPreset CurrentPreset { get; private set; } = GraphicsPreset.Auto;
        public static float RenderScale { get; private set; } = 1f;
        public static float LookSensitivity { get; private set; } = 1.0f;

        public static void Initialize()
        {
            var recommended = RecommendPreset();
            ApplyPreset(recommended);
        }

        public static GraphicsPreset RecommendPreset()
        {
            int memory = SystemInfo.graphicsMemorySize;
            if (SystemInfo.systemMemorySize <= 3000 || memory > 0 && memory <= 512) return GraphicsPreset.VeryLow;
            if (SystemInfo.systemMemorySize <= 4500 || memory > 0 && memory <= 1024) return GraphicsPreset.Low;
            if (SystemInfo.systemMemorySize <= 8000 || memory > 0 && memory <= 2048) return GraphicsPreset.Medium;
            if (SystemInfo.systemMemorySize <= 16000 || memory > 0 && memory <= 4096) return GraphicsPreset.High;
            return GraphicsPreset.Ultra;
        }

        public static void ApplyPreset(GraphicsPreset preset)
        {
            CurrentPreset = preset == GraphicsPreset.Auto ? RecommendPreset() : preset;

            switch (CurrentPreset)
            {
                case GraphicsPreset.VeryLow:
                    ApplyCore(0.65f, 0, 0, 20f, 0.50f, 1, 2, 30, 30);
                    break;
                case GraphicsPreset.Low:
                    ApplyCore(0.80f, 1, 1, 35f, 0.65f, 2, 1, 45, 45);
                    break;
                case GraphicsPreset.Medium:
                    ApplyCore(0.95f, 2, 2, 60f, 0.85f, 2, 1, 65, 60);
                    break;
                case GraphicsPreset.High:
                    ApplyCore(1.00f, 3, 3, 90f, 1.0f, 3, 1, 100, 90);
                    break;
                case GraphicsPreset.Ultra:
                    ApplyCore(1.00f, 4, 4, 140f, 1.15f, 4, 0, 150, 120);
                    break;
            }
        }

        public static void SetRenderScale(float scale)
        {
            RenderScale = Mathf.Clamp(scale, 0.5f, 1.25f);
            ApplyRenderScale();
        }

        public static void SetLookSensitivity(float sensitivity)
        {
            LookSensitivity = Mathf.Clamp(sensitivity, 0.25f, 2.5f);
        }

        public static void CyclePreset()
        {
            int next = ((int)CurrentPreset + 1) % Enum.GetValues(typeof(GraphicsPreset)).Length;
            ApplyPreset((GraphicsPreset)next);
        }

        private static void ApplyCore(float renderScale, int shadows, int shadowResolutionTier,
            float shadowDistance, float lodBias, int pixelLights, int textureLimit, int viewDistance, int targetFps)
        {
            RenderScale = renderScale;
            QualitySettings.shadows = shadows == 0 ? ShadowQuality.Disable : ShadowQuality.All;
            QualitySettings.shadowDistance = shadowDistance;
            QualitySettings.lodBias = lodBias;
            QualitySettings.pixelLightCount = pixelLights;
            QualitySettings.masterTextureLimit = textureLimit;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.vSyncCount = 0;
            QualitySettings.maximumLODLevel = 0;
            Application.targetFrameRate = targetFps;

            Shader.globalMaximumLOD = Mathf.Clamp(viewDistance * 10, 100, 2000);
            ApplyRenderScale();
        }

        private static void ApplyRenderScale()
        {
            var urp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (urp == null)
                urp = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;

            if (urp != null)
                urp.renderScale = RenderScale;
        }

        public static string GetSummary()
        {
            return $"{CurrentPreset} | Render {RenderScale:0.00} | Sensitivity {LookSensitivity:0.00}";
        }
    }
}
