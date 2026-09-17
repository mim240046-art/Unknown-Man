#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace UnknownMan.Editor
{
    [InitializeOnLoad]
    public static class CIProjectConfiguration
    {
        private const string AndroidPackageId = "com.unknownman.horrorsurvival";

        static CIProjectConfiguration()
        {
            EditorApplication.delayCall += ConfigureWhenNeeded;
        }

        [MenuItem("Unknown Man/Configure Android Build")]
        public static void ConfigureAndroid()
        {
            Configure();
            AssetDatabase.SaveAssets();
        }

        private static void ConfigureWhenNeeded()
        {
            var args = Environment.GetCommandLineArgs();
            var isBatch = Array.IndexOf(args, "-batchmode") >= 0;
            var isAndroidBuild = Array.IndexOf(args, "Android") >= 0 ||
                                 Array.IndexOf(args, "-buildTarget") >= 0 && Array.IndexOf(args, "Android") >= 0;

            if (!isBatch || isAndroidBuild)
            {
                Configure();
            }
        }

        private static void Configure()
        {
            ProjectSetup.EnsureProjectAssets();

            PlayerSettings.productName = "Unknown Man";
            PlayerSettings.companyName = "Unknown Man Studio";
            PlayerSettings.applicationIdentifier = AndroidPackageId;
            PlayerSettings.bundleVersion = "0.1.0";

            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.Android.useCustomKeystore = false;

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/Milestone1.unity", true)
            };
        }
    }
}
#endif
