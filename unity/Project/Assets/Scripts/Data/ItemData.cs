using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Item", menuName = "MOBA/Item")]
public class ItemData : ScriptableObject
{
    public string itemId;
    public string itemName;
    public string description;
    public int cost = 1000;
    
    [Header("Stats Provided")]
    public int healthBonus = 0;
    public int manaBonus = 0;
    public float attackDamageBonus = 0f;
    public float abilityPowerBonus = 0f;
    public float attackSpeedBonus = 0f;  // Percentage
    public float armorBonus = 0f;
    public float magicResistBonus = 0f;
    public float movementSpeedBonus = 0f;  // Percentage
    
    [Header("Active Effect")]
    public bool hasActiveEffect = false;
    public string activeEffectName = "";
    public float activeEffectCooldown = 0f;
    
    [Header("Passive Effect")]
    public bool hasPassiveEffect = false;
    public string passiveEffectName = "";
    
    [Header("Build Info")]
    public ItemData[] buildsFrom = new ItemData[0];  // Recipe components
    public ItemData[] buildsInto = new ItemData[0];  // What upgrades to
}
