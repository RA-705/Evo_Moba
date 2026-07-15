using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ShopUIUpdated : MonoBehaviour
{
    public static ShopUIUpdated Instance { get; private set; }
    
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private GameObject itemButtonPrefab;
    [SerializeField] private Text goldText;
    [SerializeField] private Text playerLevelText;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private Text itemNameText;
    [SerializeField] private Text itemDescriptionText;
    [SerializeField] private Text itemStatsText;
    
    private HeroData _heroData;
    private ItemData[] _availableItems;
    private bool _shopOpen;
    private int _currentGold = 5000;  // Starting gold
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        shopPanel.SetActive(false);
        LoadItems();
        UpdateGoldDisplay();
    }
    
    private void LoadItems()
    {
        var json = Resources.Load<TextAsset>("Items/items");
        if (json == null)
        {
            Debug.LogError("Items JSON not found!");
            return;
        }
        
        // Parse JSON
        _availableItems = JsonHelper.FromJson<ItemData>(json.text);
    }
    
    public void ToggleShop()
    {
        _shopOpen = !_shopOpen;
        shopPanel.SetActive(_shopOpen);
        
        if (_shopOpen)
        {
            RefreshShop();
        }
    }
    
    private void RefreshShop()
    {
        // Clear previous items
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Sort items by cost
        var sortedItems = _availableItems.OrderBy(i => i.cost).ToList();
        
        // Load available items based on hero level and gold
        foreach (var item in sortedItems)
        {
            var buttonGo = Instantiate(itemButtonPrefab, itemsContainer);
            var button = buttonGo.GetComponent<Button>();
            var buttonText = buttonGo.GetComponentInChildren<Text>();
            
            bool canAfford = _currentGold >= item.cost;
            buttonText.text = $"<color={(canAfford ? "green" : "red")}>{item.itemName} ({item.cost}g)</color>";
            
            button.interactable = canAfford;
            button.onClick.AddListener(() => ShowItemDetails(item));
        }
    }
    
    private void ShowItemDetails(ItemData item)
    {
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;
        
        // Build stats display
        var stats = new List<string>();
        if (item.healthBonus > 0) stats.Add($"+{item.healthBonus} Health");
        if (item.manaBonus > 0) stats.Add($"+{item.manaBonus} Mana");
        if (item.attackDamageBonus > 0) stats.Add($"+{item.attackDamageBonus} AD");
        if (item.abilityPowerBonus > 0) stats.Add($"+{item.abilityPowerBonus} AP");
        if (item.attackSpeedBonus > 0) stats.Add($"+{item.attackSpeedBonus}% AS");
        if (item.armorBonus > 0) stats.Add($"+{item.armorBonus} Armor");
        if (item.magicResistBonus > 0) stats.Add($"+{item.magicResistBonus} MR");
        if (item.movementSpeedBonus > 0) stats.Add($"+{item.movementSpeedBonus}% MS");
        if (item.hasPassiveEffect) stats.Add($"✦ {item.passiveEffectName}");
        if (item.hasActiveEffect) stats.Add($"⚡ {item.activeEffectName}");
        
        itemStatsText.text = string.Join("\n", stats);
        
        // Show build path
        if (item.buildsFrom.Length > 0)
        {
            Debug.Log($"Builds from: {string.Join(", ", item.buildsFrom.Select(i => i.itemName))}" );
        }
        if (item.buildsInto.Length > 0)
        {
            Debug.Log($"Builds into: {string.Join(", ", item.buildsInto.Select(i => i.itemName))}");
        }
    }
    
    public void BuyItem(ItemData item)
    {
        if (_currentGold >= item.cost)
        {
            _currentGold -= item.cost;
            UpdateGoldDisplay();
            Debug.Log($"Purchased: {item.itemName}");
            // TODO: Send to server and update UI
        }
    }
    
    public void AddGold(int amount)
    {
        _currentGold += amount;
        UpdateGoldDisplay();
    }
    
    private void UpdateGoldDisplay()
    {
        goldText.text = $"Gold: {_currentGold}";
    }
}

// Helper class for JSON parsing
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        var wrapper = JsonUtility.FromJson<Wrapper<T>>("{\"items\":" + json + "}");
        return wrapper.items;
    }
    
    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}
