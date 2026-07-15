using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }
    
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private GameObject itemButtonPrefab;
    [SerializeField] private Text goldText;
    [SerializeField] private Text playerLevelText;
    
    private HeroData _heroData;
    private List<ItemData> _availableItems = new();
    private bool _shopOpen;
    
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
    }
    
    private void LoadItems()
    {
        var itemsResource = Resources.LoadAll<ItemData>("Items");
        _availableItems.AddRange(itemsResource);
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
        
        // Load available items based on hero level and gold
        foreach (var item in _availableItems)
        {
            var button = Instantiate(itemButtonPrefab, itemsContainer);
            var buttonText = button.GetComponentInChildren<Text>();
            buttonText.text = $"{item.name} ({item.cost}g)";
            
            button.GetComponent<Button>().onClick.AddListener(() => BuyItem(item));
        }
    }
    
    private void BuyItem(ItemData item)
    {
        // Send purchase to server
        Debug.Log($"Buying item: {item.name}");
        // TODO: Send to server and update UI
    }
}
