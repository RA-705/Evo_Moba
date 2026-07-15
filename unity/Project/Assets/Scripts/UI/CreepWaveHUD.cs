using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI que muestra información de oleadas de minions
/// </summary>
public class CreepWaveHUD : MonoBehaviour
{
    [SerializeField] private CreepWaveSpawner waveSpawner;
    [SerializeField] private Text waveNumberText;
    [SerializeField] private Text nextWaveTimerText;
    [SerializeField] private Text activeMinionCountText;
    
    private void Update()
    {
        if (waveSpawner == null) return;
        
        int waveCount = waveSpawner.GetWaveCount();
        float nextWaveIn = waveSpawner.GetNextWaveIn();
        int activeMinions = waveSpawner.GetActiveMinions();
        
        waveNumberText.text = $"Wave: {waveCount}";
        nextWaveTimerText.text = $"Next: {nextWaveIn:F1}s";
        activeMinionCountText.text = $"Minions: {activeMinions}";
    }
}
