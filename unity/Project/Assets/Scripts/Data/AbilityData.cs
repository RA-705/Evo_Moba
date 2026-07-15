using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "MOBA/Ability")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    public string description;
    
    [Header("Cooldown & Cost")]
    public float cooldown = 10f;
    public int manaCost = 50;
    
    [Header("Damage")]
    public float baseDamage = 100f;
    public float scalingAD = 0.7f;  // Attack Damage scaling
    public float scalingAP = 0.6f;  // Ability Power scaling
    
    [Header("Range & Radius")]
    public float castRange = 10f;
    public float effectRadius = 3f;
    
    [Header("Effects")]
    public float slowPercent = 0f;  // 0-100
    public float stunDuration = 0f;
    public float knockbackForce = 0f;
    
    [Header("Utility")]
    public bool isUltimate = false;
    public bool targetGround = false;  // True if targets ground, false if targets unit
}
