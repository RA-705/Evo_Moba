using UnityEngine;
using System.Threading.Tasks;
using Evo.Core;
using Evo.UnityClient;

namespace Evo.Client
{
    public class GameEntryPoint : MonoBehaviour
    {
        public static GameEntryPoint Instance { get; private set; }

        [Header("Auto Start")]
        public bool AutoConnectOnStart = true;
        public string ServerAddress = "127.0.0.1";
        public int ServerPort = 7777;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            QualitySettings.maxQueuedFrames = 1;
        }

        private async void Start()
        {
            Debug.Log("[GameEntryPoint] Starting Evo MOBA Client...");
            InitializeManagers();
            if (AutoConnectOnStart) await ConnectToServer();
        }

        private void InitializeManagers()
        {
            var gameManager = GameManager.Instance;
            var camera = GameCamera.Instance;
            var uiManager = UIManager.Instance;
            var entityManager = EntityManager.Instance;
            var effectManager = EffectManager.Instance;
            Debug.Log("[GameEntryPoint] All managers initialized");
        }

        private async Task ConnectToServer()
        {
            Debug.Log($"[GameEntryPoint] Connecting to {ServerAddress}:{ServerPort}...");
            var networkManager = NetworkManager.Instance;
            bool connected = await networkManager.ConnectAsync(ServerAddress, ServerPort);
            if (connected)
            {
                Debug.Log("[GameEntryPoint] Connected successfully!");
                var gameManager = GameManager.Instance;
                await gameManager.StartGameAsync();
            }
            else
            {
                Debug.LogError("[GameEntryPoint] Failed to connect to server!");
                ShowConnectionError();
            }
        }

        private void ShowConnectionError()
        {
            var uiManager = UIManager.Instance;
            if (uiManager != null)
                uiManager.ShowError("Failed to connect to server. Please check if the server is running.");
        }

        private void OnApplicationQuit()
        {
            NetworkManager.Instance?.Disconnect();
        }
    }
}
