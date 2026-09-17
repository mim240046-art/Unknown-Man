#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnknownMan.Editor
{
    [InitializeOnLoad]
    public static class ProjectSetup
    {
        private const string SettingsFolder = "Assets/Settings/URP";
        private const string RendererPath = SettingsFolder + "/UnknownManRenderer.asset";
        private const string PipelinePath = SettingsFolder + "/UnknownManURP.asset";

        static ProjectSetup()
        {
            EditorApplication.delayCall += EnsureProjectAssets;
        }

        [MenuItem("Unknown Man/Setup Project Foundation")]
        public static void EnsureProjectAssets()
        {
            EnsureFolder("Assets/Settings");
            EnsureFolder(SettingsFolder);

            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
            if (pipeline == null)
            {
                var renderer = AssetDatabase.LoadAssetAtPath<ScriptableRendererData>(RendererPath);
                if (renderer == null)
                {
                    var rendererAsset = ScriptableObject.CreateInstance<UniversalRendererData>();
                    AssetDatabase.CreateAsset(rendererAsset, RendererPath);
                    AssetDatabase.SaveAssets();
                    pipeline = UniversalRenderPipelineAsset.Create(rendererAsset);
                }
                else
                {
                    pipeline = UniversalRenderPipelineAsset.Create(renderer);
                }

                AssetDatabase.CreateAsset(pipeline, PipelinePath);
                AssetDatabase.SaveAssets();
            }

            GraphicsSettings.defaultRenderPipeline = pipeline;
            EditorUtility.SetDirty(pipeline);
            AssetDatabase.SaveAssets();

            var scenes = new[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/Milestone1.unity", true)
            };
            EditorBuildSettings.scenes = scenes;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parent = System.IO.Path.GetDirectoryName(path)?.Replace('\\', '/');
            var name = System.IO.Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            if (!string.IsNullOrEmpty(parent)) AssetDatabase.CreateFolder(parent, name);
        }
    }
}
#endif
