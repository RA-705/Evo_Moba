using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using LiteNetLib;
using Evo.Client;

namespace Evo.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game Config")]
        public GameConfig GameConfig;

        [Header("Scene Names")]
        public string LobbySceneName = "Lobby";
        public string MatchSceneName = "Match";

        private bool _isInMatch = false;

        public int LocalPlayerId { get; private set; }
        public int LocalHeroId { get; private set; }
        public int LocalTeamId { get; private set; }
        public MatchState CurrentMatchState { get; private set; } = MatchState.MainMenu;

        public event Action<MatchState> OnMatchStateChanged;
        public event Action OnMatchStarted;
        public event Action OnMatchEnded;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameConfig();
        }

        private void LoadGameConfig()
        {
            var configAsset = Resources.Load<TextAsset>("Data/evoconfig");
            if (configAsset != null)
            {
                GameConfig = JsonUtility.FromJson<GameConfig>(configAsset.text);
                Debug.Log($"[GameManager] Loaded config: {GameConfig.ServerName}");
            }
            else
            {
                GameConfig = new GameConfig();
                Debug.LogWarning("[GameManager] Using default config");
            }
        }

        public void StartOfflineAIBattle(int teamSize = 3)
        {
            Debug.Log($"[GameManager] Starting offline AI battle with {teamSize}v{teamSize}");
            SetMatchState(MatchState.Loading);
            _isInMatch = true;
            MatchSceneManager.Instance?.StartAIBattle(teamSize);
            OnMatchStarted?.Invoke();
            SetMatchState(MatchState.InGame);
        }

        public async Task<bool> StartGameAsync()
        {
            Debug.Log("[GameManager] Starting game...");
            var networkManager = NetworkManager.Instance;
            if (networkManager == null) return false;
            var connected = await networkManager.ConnectAsync();
            if (!connected)
            {
                Debug.LogError("[GameManager] Failed to connect to server");
                return false;
            }
            await LoadSceneAsync(LobbySceneName);
            SetMatchState(MatchState.Lobby);
            return true;
        }

        public async Task LoadSceneAsync(string sceneName)
        {
            var operation = SceneManager.LoadSceneAsync(sceneName);
            while (!operation.isDone) await Task.Yield();
            Debug.Log($"[GameManager] Loaded scene: {sceneName}");
        }

        public void SetMatchState(MatchState state)
        {
            CurrentMatchState = state;
            OnMatchStateChanged?.Invoke(state);
        }

        public void SelectHero(int heroId)
        {
            NetworkManager.Instance?.Send(new Evo.Client.ClientPackets.HeroSelectRequest { HeroId = heroId });
        }

        public void SendMoveInput(EvoVector3 direction)
        {
            NetworkManager.Instance?.Send(new Evo.Client.ClientPackets.PlayerInput
            {
                MoveDirX = direction.x,
                MoveDirY = direction.y,
                MoveDirZ = direction.z
            }, DeliveryMethod.Unreliable);
        }

        public void SendChatMessage(string message)
        {
            NetworkManager.Instance?.Send(new Evo.Client.ClientPackets.ChatMessage { Message = message });
        }

        private void Update()
        {
            if (_isInMatch)
                MatchSceneManager.Instance?.Tick();
        }

        private void OnDestroy()
        {
            NetworkManager.Instance?.Disconnect();
        }
    }

    public enum MatchState
    {
        MainMenu,
        Connecting,
        Lobby,
        Loading,
        InGame,
        PostGame,
        Reconnecting
    }

    [Serializable]
    public class GameConfig
    {
        public int GridWidth = 30;
        public int GridDepth = 30;
        public float CellSize = 1.5f;
        public int HeroesPerTeam = 3;
        public int TicksPerSecond = 10;
        public int ListenPort = 7777;
        public bool EnableNetworking = true;
        public bool EnableFogOfWar = true;
        public bool EnableTeamPlanner = true;
        public string ServerName = "EVO Match";
    }
}
