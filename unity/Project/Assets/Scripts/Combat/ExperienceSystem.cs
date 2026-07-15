using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Maneja el cálculo de experiencia y leveo
/// </summary>
public class ExperienceSystem : MonoBehaviour
{
    [SerializeField] private HeroStats heroStats;
    
    private int _currentExperience;
    private int _level = 1;
    
    // Experiencia necesaria por nivel
    private static readonly int[] ExperienceThresholds = new int[]
    {
        0,      // Level 1
        100,    // Level 2
        260,    // Level 3
        480,    // Level 4
        760,    // Level 5
        1100,   // Level 6
        1500,   // Level 7
        1960,   // Level 8
        2480,   // Level 9
        3060,   // Level 10
        3700,   // Level 11
        4400,   // Level 12
        5160,   // Level 13
        5980,   // Level 14
        6860,   // Level 15
        7800,   // Level 16
        8800    // Level 17
    };
    
    public event System.Action<int> OnLevelUp;  // int = new level
    
    public void AddExperience(int amount)
    {
        _currentExperience += amount;
        CheckLevelUp();
    }
    
    private void CheckLevelUp()
    {
        int nextLevel = _level + 1;
        if (nextLevel < ExperienceThresholds.Length && _currentExperience >= ExperienceThresholds[nextLevel])
        {
            LevelUp();
        }
    }
    
    private void LevelUp()
    {
        _level++;
        heroStats.LevelUp();
        OnLevelUp?.Invoke(_level);
        Debug.Log($"Level up! Now level {_level}");
        CheckLevelUp();  // En caso de que gane suficiente XP para múltiples niveles
    }
    
    public int GetLevel() => _level;
    public int GetCurrentExperience() => _currentExperience;
    public int GetExperienceForNextLevel() => _level + 1 < ExperienceThresholds.Length ? ExperienceThresholds[_level + 1] : int.MaxValue;
    public float GetExperiencePercent() => (float)_currentExperience / GetExperienceForNextLevel();
}
