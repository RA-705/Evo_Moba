using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI de combate mostrando stats en tiempo real
/// </summary>
public class CombatHUD : MonoBehaviour
{
    [SerializeField] private HeroStats heroStats;
    [SerializeField] private ExperienceSystem expSystem;
    [SerializeField] private GoldSystem goldSystem;
    [SerializeField] private AbilityExecutor abilityExecutor;
    
    [Header("Health/Mana")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private Text healthText;
    [SerializeField] private Text manaText;
    
    [Header("Experience")]
    [SerializeField] private Image expBar;
    [SerializeField] private Text levelText;
    
    [Header("Resources")]
    [SerializeField] private Text goldText;
    
    [Header("Abilities")]
    [SerializeField] private Image[] abilityCooldowns = new Image[4];
    [SerializeField] private Text[] abilityTexts = new Text[4];
    
    private void Start()
    {
        heroStats.OnHealthChanged += UpdateHealth;
        expSystem.OnLevelUp += UpdateLevel;
        goldSystem.OnGoldChanged += UpdateGold;
    }
    
    private void Update()
    {
        UpdateMana();
        UpdateExperience();
        UpdateAbilities();
    }
    
    private void UpdateHealth(int health)
    {
        int maxHealth = heroStats.GetMaxHealth();
        healthBar.fillAmount = (float)health / maxHealth;
        healthText.text = $"{health}/{maxHealth}";
    }
    
    private void UpdateMana()
    {
        int mana = heroStats.GetMana();
        int maxMana = heroStats.GetMaxMana();
        manaBar.fillAmount = (float)mana / maxMana;
        manaText.text = $"{mana}/{maxMana}";
    }
    
    private void UpdateExperience()
    {
        expBar.fillAmount = expSystem.GetExperiencePercent();
        levelText.text = $"Lvl {expSystem.GetLevel()}";
    }
    
    private void UpdateGold(int gold)
    {
        goldText.text = $"Gold: {gold}";
    }
    
    private void UpdateAbilities()
    {
        for (int i = 0; i < 4; i++)
        {
            var ability = abilityExecutor.GetAbility(i);
            if (ability != null)
            {
                float cooldown = abilityExecutor.GetCooldown(i);
                if (cooldown > 0)
                {
                    abilityCooldowns[i].fillAmount = 1 - (cooldown / ability.cooldown);
                    abilityTexts[i].text = $"{cooldown:F1}";
                }
                else
                {
                    abilityCooldowns[i].fillAmount = 0;
                    abilityTexts[i].text = "R";
                }
            }
        }
    }
    
    private void OnDestroy()
    {
        if (heroStats != null)
            heroStats.OnHealthChanged -= UpdateHealth;
    }
}
