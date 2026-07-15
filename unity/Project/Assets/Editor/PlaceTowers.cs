using UnityEngine;
using UnityEditor;

public static class PlaceTowers
{
    static readonly (Vector3 pos, string name, bool isBlue)[] TowerDefs = {
        // BlueGuard_Chassis position (confirmed from level4)
        (new Vector3(11.04f, 0, -32.12f), "BlueGuard_Chassis", true),
        // Blue outer towers (estimated along lanes)
        (new Vector3(-8, 0, -22),  "BlueTower_TopOuter", true),
        (new Vector3(-5, 0, -16),  "BlueTower_TopInner", true),
        (new Vector3(-2, 0, -12),  "BlueTower_TopBase", true),
        (new Vector3(-8, 0, 22),   "BlueTower_BotOuter", true),
        (new Vector3(-5, 0, 16),   "BlueTower_BotInner", true),
        (new Vector3(-2, 0, 12),   "BlueTower_BotBase", true),
        (new Vector3(-22, 0, 0),   "BlueTower_MidOuter", true),
        (new Vector3(-16, 0, 0),   "BlueTower_MidInner", true),
        (new Vector3(-10, 0, 0),   "BlueTower_MidBase", true),
        (new Vector3(-26, 0, -6),  "BlueTower_BaseLeft", true),
        (new Vector3(-26, 0, 6),   "BlueTower_BaseRight", true),
        // RedGuard_Chassis position (confirmed from level4)
        (new Vector3(33.77f, 0, -10.81f), "RedGuard_Chassis", false),
        // Red outer towers (estimated along lanes)
        (new Vector3(8, 0, 22),    "RedTower_TopOuter", false),
        (new Vector3(5, 0, 16),    "RedTower_TopInner", false),
        (new Vector3(2, 0, 12),    "RedTower_TopBase", false),
        (new Vector3(8, 0, -22),   "RedTower_BotOuter", false),
        (new Vector3(5, 0, -16),   "RedTower_BotInner", false),
        (new Vector3(2, 0, -12),   "RedTower_BotBase", false),
        (new Vector3(22, 0, 0),    "RedTower_MidOuter", false),
        (new Vector3(16, 0, 0),    "RedTower_MidInner", false),
        (new Vector3(10, 0, 0),    "RedTower_MidBase", false),
        (new Vector3(26, 0, -6),   "RedTower_BaseLeft", false),
        (new Vector3(26, 0, 6),    "RedTower_BaseRight", false),
    };

    [MenuItem("Tools/Place Towers")]
    static void Place()
    {
        var blueModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/misc/BlueGuard.glb");
        var redModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/misc/RedGuard.glb");
        if (blueModel == null) { Debug.LogError("BlueGuard.glb not found"); return; }
        if (redModel == null) { Debug.LogError("RedGuard.glb not found"); return; }

        var root = GameObject.Find("MapRoot");
        if (root == null) { root = new GameObject("MapRoot"); }

        var bt = new GameObject("BlueTowers"); bt.transform.SetParent(root.transform);
        var rt = new GameObject("RedTowers"); rt.transform.SetParent(root.transform);

        foreach (var (pos, name, isBlue) in TowerDefs)
        {
            var go = new GameObject(name);
            go.transform.SetParent(isBlue ? bt.transform : rt.transform);
            go.transform.position = pos;
            var model = (GameObject)PrefabUtility.InstantiatePrefab(isBlue ? blueModel : redModel, go.transform);
            model.name = "TowerMesh";
            var col = go.AddComponent<BoxCollider>();
            col.size = new Vector3(2.5f, 5, 2.5f);
            col.isTrigger = true;
        }
        Debug.Log($"Placed {TowerDefs.Length} towers");
    }

    [MenuItem("Tools/Clear Towers")]
    static void Clear()
    {
        var root = GameObject.Find("MapRoot");
        if (root == null) return;
        var bt = root.transform.Find("BlueTowers");
        var rt = root.transform.Find("RedTowers");
        if (bt != null) Object.DestroyImmediate(bt.gameObject);
        if (rt != null) Object.DestroyImmediate(rt.gameObject);
    }
}
