using UnityEngine;

public class AbilitiesLoader : MonoBehaviour
{
    [System.Serializable]
    public class AbilityList
    {
        public AbilityData[] abilities;
    }
    
    public static AbilityData[] LoadAbilities()
    {
        var json = Resources.Load<TextAsset>("Abilities/abilities");
        if (json == null)
        {
            Debug.LogError("Abilities JSON not found!");
            return new AbilityData[0];
        }
        
        var wrapper = JsonUtility.FromJson<AbilityList>("{\"abilities\":" + json.text + "}");
        return wrapper.abilities;
    }
}
