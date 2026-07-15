using UnityEngine;

public class ItemsLoader : MonoBehaviour
{
    [System.Serializable]
    public class ItemList
    {
        public ItemData[] items;
    }
    
    public static ItemData[] LoadItems()
    {
        var json = Resources.Load<TextAsset>("Items/items");
        if (json == null)
        {
            Debug.LogError("Items JSON not found!");
            return new ItemData[0];
        }
        
        var wrapper = JsonUtility.FromJson<ItemList>("{\"items\":" + json.text + "}");
        return wrapper.items;
    }
}
