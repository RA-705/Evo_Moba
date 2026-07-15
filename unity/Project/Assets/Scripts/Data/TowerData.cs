using UnityEngine;

[CreateAssetMenu(fileName = "Tower", menuName = "MOBA/Tower")]
public class TowerData : ScriptableObject
{
    public string towerId;
    public string towerName;
    
    [Header("Stats")]
    public int health = 1800;
    public float attackDamage = 100f;
    public float attackRange = 12f;
    public float attackSpeed = 1f;
    public float armor = 40f;
    
    [Header("Behavior")]
    public float targetingRange = 15f;
    public bool prioritizeHeroes = true;
    public float aggro = 2f;  // How easily it aggros
}
