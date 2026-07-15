using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel de información de torretas
/// Muestra estado de torres en el carril
/// </summary>
public class TowerStatusPanel : MonoBehaviour
{
    [SerializeField] private Tower tower;
    [SerializeField] private Image healthBar;
    [SerializeField] private Text healthText;
    [SerializeField] private Text towerNameText;
    
    private void Start()
    {
        if (tower != null)
        {
            tower.OnHealthChanged += UpdateHealthDisplay;
            tower.OnDestroyed += OnTowerDestroyed;
            
            towerNameText.text = tower.name;
            UpdateHealthDisplay(tower.GetHealth());
        }
    }
    
    private void Update()
    {
        if (tower != null && !tower.IsDestroyed())
        {
            // Posicionar panel encima de la torre
            transform.position = Camera.main.WorldToScreenPoint(tower.transform.position + Vector3.up * 5f);
        }
    }
    
    private void UpdateHealthDisplay(int health)
    {
        int maxHealth = tower.GetMaxHealth();
        healthBar.fillAmount = (float)health / maxHealth;
        healthText.text = $"{health}/{maxHealth}";
    }
    
    private void OnTowerDestroyed()
    {
        Destroy(gameObject);
    }
}
