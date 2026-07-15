using UnityEngine;
using Evo.Client.ServerPackets;

namespace Evo.Client
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        public GameObject MainMenuPanel;
        public GameObject LobbyPanel;
        public GameObject LoadingPanel;
        public GameObject HUDPanel;
        public GameObject ErrorPanel;

        private void Awake()
        {
            Instance = this;
            ShowPanel(MainMenuPanel);
        }

        public void ShowPanel(GameObject panel)
        {
            MainMenuPanel?.SetActive(panel == MainMenuPanel);
            LobbyPanel?.SetActive(panel == LobbyPanel);
            LoadingPanel?.SetActive(panel == LoadingPanel);
            HUDPanel?.SetActive(panel == HUDPanel);
            ErrorPanel?.SetActive(panel == ErrorPanel);
        }

        public void ShowError(string message)
        {
            Debug.LogError($"[UIManager] {message}");
            ShowPanel(ErrorPanel);
        }

        public void UpdateHUD(string heroName, int level, float healthPercent, float manaPercent, int gold, int kills, int deaths, int assists)
        {
        }

        public void UpdateTimer(int seconds)
        {
        }

        public void UpdatePing(int ping)
        {
        }

        public void AddChatMessage(string playerName, string message, ChatChannel channel)
        {
        }

        public void ShowHUD()
        {
            ShowPanel(HUDPanel);
        }

        public void ShowLobby()
        {
            ShowPanel(LobbyPanel);
        }

        public void OnDisconnectButton()
        {
            ShowPanel(MainMenuPanel);
        }
    }
}
