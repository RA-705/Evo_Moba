using UnityEngine;

[CreateAssetMenu(fileName = "Minion", menuName = "MOBA/Minion")]
public class MinionData : ScriptableObject
{
    public string minionId;
    public string minionName;
    
    [Header("Stats")]
    public int health = 50;
    public float attackDamage = 5f;
    public float attackSpeed = 1f;
    public float attackRange = 5.5f;
    public float movementSpeed = 3f;
    public int armor = 5;
    public int magicResist = 0;
    
    [Header("Rewards")]
    public int goldReward = 20;
    public int experienceReward = 60;
    
    [Header("Behavior")]
    public float visionRange = 8f;
    public float aggressionRange = 10f;
    public bool isSiegeMinion = false;  // Minion más fuerte, menos frecuente
}
