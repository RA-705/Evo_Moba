using UnityEngine;

public class HeroesLoader : MonoBehaviour
{
    [System.Serializable]
    public class HeroList
    {
        public HeroData[] heroes;
    }
    
    public static HeroData[] LoadHeroes()
    {
        var json = Resources.Load<TextAsset>("Heroes/heroes");
        if (json == null)
        {
            Debug.LogError("Heroes JSON not found!");
            return new HeroData[0];
        }
        
        var wrapper = JsonUtility.FromJson<HeroList>("{\"heroes\":" + json.text + "}");
        return wrapper.heroes;
    }
}
