using UnityEngine;
using UnknownMan.Inventory;
using UnknownMan.Player;
using UnknownMan.UI;
using UnknownMan.World;

namespace UnknownMan.Core
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        private static GameBootstrap instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureBootstrapScene()
        {
            // The project ships with a scene containing this component.
            // This method intentionally does not spawn a duplicate bootstrap.
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            SaveSystem.Initialize();
            GraphicsSettingsController.Initialize();
            ProceduralVillageBuilder.Build();
            PlayerFactory.Create();
            HUDController.Create();
            PerformanceManager.Create();

            SaveSystem.LoadIntoRuntime();
            SaveSystem.ApplySavedPlayerTransform();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) SaveSystem.SaveFromRuntime();
        }

        private void OnApplicationQuit()
        {
            SaveSystem.SaveFromRuntime();
        }
    }
}
