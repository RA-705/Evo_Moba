using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Hero", menuName = "MOBA/Hero")]
public class HeroData : ScriptableObject
{
    public string heroId;
    public string heroName;
    public string description;
    
    [Header("Stats")]
    public int baseHealth;
    public int baseMana;
    public float baseAttackDamage;
    public float baseAttackSpeed;
    public float baseMovementSpeed;
    public float baseArmor;
    public float baseMagicResist;
    
    [Header("Abilities")]
    public AbilityData[] abilities = new AbilityData[4];
    
    [Header("Progression")]
    public int healthPerLevel = 80;
    public int manaPerLevel = 40;
    public float damagePerLevel = 3.5f;
}
