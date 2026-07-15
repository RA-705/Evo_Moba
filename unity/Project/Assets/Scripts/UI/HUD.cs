using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Text gameTimeText;
    [SerializeField] private Text blueGoldText;
    [SerializeField] private Text redGoldText;
    [SerializeField] private Text heroHealthText;
    [SerializeField] private Image heroHealthBar;
    [SerializeField] private Transform abilitiesContainer;
    [SerializeField] private GameObject abilityButtonPrefab;
    
    private GameState _lastGameState;
    
    private void Start()
    {
        GameClient.Instance.OnSnapshotReceived += OnSnapshotReceived;
    }
    
    private void OnSnapshotReceived(WorldSnapshot snapshot)
    {
        UpdateGameState(snapshot.GameState);
    }
    
    private void UpdateGameState(GameState state)
    {
        _lastGameState = state;
        
        // Update time
        int minutes = state.GameTime / 60;
        int seconds = state.GameTime % 60;
        gameTimeText.text = $"{minutes:00}:{seconds:00}";
        
        // Update gold
        blueGoldText.text = $"Gold: {state.BlueTeamGold}";
        redGoldText.text = $"Gold: {state.RedTeamGold}";
    }
    
    public void UpdateHeroHealth(int current, int max)
    {
        heroHealthText.text = $"{current}/{max}";
        heroHealthBar.fillAmount = (float)current / max;
    }
    
    private void OnDestroy()
    {
        if (GameClient.Instance != null)
            GameClient.Instance.OnSnapshotReceived -= OnSnapshotReceived;
    }
}
