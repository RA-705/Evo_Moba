using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD que muestra estado de torres
/// </summary>
public class TowerHUD : MonoBehaviour
{
    [SerializeField] private Text towerStatusText;
    [SerializeField] private Image[] blueTowerIndicators;   // Fila de torres azules
    [SerializeField] private Image[] redTowerIndicators;    // Fila de torres rojas
    
    private TowerManager _towerManager;
    
    private void Start()
    {
        _towerManager = TowerManager.Instance;
        if (_towerManager != null)
        {
            _towerManager.OnTowerDestroyed += OnTowerDestroyed;
        }
    }
    
    private void Update()
    {
        UpdateTowerIndicators();
    }
    
    private void UpdateTowerIndicators()
    {
        if (_towerManager == null) return;
        
        // Mostrar torres azules
        var blueTowers = _towerManager.GetTowersByTeam(0);
        for (int i = 0; i < blueTowerIndicators.Length && i < blueTowers.Count; i++)
        {
            Tower tower = blueTowers[i];
            if (tower.IsDestroyed())
                blueTowerIndicators[i].color = Color.gray;
            else
                blueTowerIndicators[i].color = Color.blue;
        }
        
        // Mostrar torres rojas
        var redTowers = _towerManager.GetTowersByTeam(1);
        for (int i = 0; i < redTowerIndicators.Length && i < redTowers.Count; i++)
        {
            Tower tower = redTowers[i];
            if (tower.IsDestroyed())
                redTowerIndicators[i].color = Color.gray;
            else
                redTowerIndicators[i].color = Color.red;
        }
    }
    
    private void OnTowerDestroyed(Tower tower, int destroyerTeam)
    {
        Debug.Log($"Tower destroyed by team {destroyerTeam}!");
        // TODO: Animación o sonido
    }
}
