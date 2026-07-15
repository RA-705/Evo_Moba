using UnityEngine;

/// <summary>
/// Sistema de recompensas para kills de minions/creeps
/// </summary>
public class CreepRewardSystem : MonoBehaviour
{
    private MinionManager _minionManager;
    private Dictionary<int, GoldSystem> _playerGoldSystems = new();
    private Dictionary<int, ExperienceSystem> _playerExpSystems = new();
    
    private void Start()
    {
        _minionManager = MinionManager.Instance;
        if (_minionManager != null)
        {
            _minionManager.OnMinionKilled += OnMinionKilled;
        }
    }
    
    private void OnMinionKilled(MinionAI minion, int killerTeam)
    {
        // TODO: Agregar oro y XP al jugador que mató
        Debug.Log($"Minion killed by team {killerTeam}");
    }
    
    public void RegisterPlayer(int playerId, GoldSystem goldSystem, ExperienceSystem expSystem)
    {
        _playerGoldSystems[playerId] = goldSystem;
        _playerExpSystems[playerId] = expSystem;
    }
}
