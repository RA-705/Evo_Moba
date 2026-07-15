using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Evo.Client;

public static class HeroPrefabCreator
{
    static readonly Dictionary<string, (int id, string name)> HeroMapping = new()
    {
        {"CH_Keth", (1, "Keth")}, {"CH_Cleo", (2, "Cleo")}, {"CH_Brunhild", (3, "Brunhild")},
        {"CH_Mikal", (4, "Mikal")}, {"CH_Winona", (5, "Winona")}, {"CH_Moros", (6, "Moros")},
        {"CH_Nivan", (7, "Nivan")}, {"CH_tigria", (8, "Tigria")}, {"CH_Valena", (9, "Valena")},
        {"CH_Aramis", (10, "Aramis")}, {"CH_Eida", (11, "Eida")}, {"CH_Styx", (12, "Styx")},
        {"CH_Clawdia", (13, "Clawdia")}, {"CH_Memphis", (14, "Memphis")}, {"CH_loth", (15, "Loth")},
        {"CH_Zoya", (16, "Zoya")}, {"CH_Reisa", (17, "Reisa")}, {"CH_Hagen", (18, "Hagen")},
        {"CH_Christine", (19, "Christine")}, {"CH_Raven", (20, "Raven")}, {"CH_Judith", (21, "Judith")},
        {"CH_Orsour", (22, "Orsour")}, {"CH_Idony", (23, "Idony")}, {"CH_Kyouya", (24, "Kyouya")},
        {"CH_Scorpion", (25, "Scorpion")}, {"CH_Norah", (26, "Norah")}, {"CH_Salome", (27, "Salome")},
        {"CH_Wulfric", (28, "Wulfric")}, {"CH_Oboro", (29, "Oboro")}, {"CH_Adele", (30, "Adele")},
        {"CH_Joey", (31, "Joey")}, {"CH_Xenos", (32, "Xenos")}, {"CH_Chloe", (35, "Chloe")},
        {"CH_Lars", (36, "Lars")}
    };

    [MenuItem("Tools/Create Hero Prefabs")]
    public static void CreateAllHeroPrefabs()
    {
        string charsDir = "Assets/Characters";
        string outputDir = "Assets/Resources/Heroes";
        Directory.CreateDirectory(outputDir);

        var heroDirs = Directory.GetDirectories(charsDir)
            .Where(d => HeroMapping.ContainsKey(Path.GetFileName(d)))
            .OrderBy(d => HeroMapping[Path.GetFileName(d)].id)
            .ToArray();

        int created = 0, skipped = 0;
        foreach (var dir in heroDirs)
        {
            string folderName = Path.GetFileName(dir);
            var (heroId, heroName) = HeroMapping[folderName];
            string prefabPath = $"{outputDir}/{heroName}.prefab";

            if (File.Exists(prefabPath))
            {
                Debug.Log($"Skipping {heroName} (prefab exists)");
                skipped++;
                continue;
            }

            var root = new GameObject(heroName);
            root.transform.position = Vector3.zero;

            var entity = root.AddComponent<HeroEntity>();
            entity.HeroId = heroId;

            var controller = root.GetComponent<CharacterController>();
            if (controller == null) controller = root.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.4f;
            controller.center = new Vector3(0, 0.9f, 0);

            var animator = root.GetComponent<Animator>();
            if (animator == null) animator = root.AddComponent<Animator>();
            entity.Animator = animator;
            entity.Controller = controller;

            string animCtrlPath = $"Assets/Animators/{heroName}_Animator.controller";
            var animCtrl = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(animCtrlPath);
            if (animCtrl != null)
            {
                animator.runtimeAnimatorController = animCtrl;
            }

            GameObject modelInstance = null;
            var modelsDir = Path.Combine(dir, "models").Replace("\\", "/");
            if (Directory.Exists(modelsDir))
            {
                var glbFiles = Directory.GetFiles(modelsDir, "*.glb")
                    .Select(f => f.Replace("\\", "/"))
                    .OrderByDescending(f => new FileInfo(f).Length)
                    .ToArray();

                if (glbFiles.Length > 0)
                {
                    var mainModel = AssetDatabase.LoadAssetAtPath<GameObject>(glbFiles[0]);
                    if (mainModel != null)
                    {
                        modelInstance = (GameObject)PrefabUtility.InstantiatePrefab(mainModel, root.transform);
                        modelInstance.name = "Model";
                        modelInstance.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                        entity.ModelRoot = modelInstance.transform;
                    }
                }
            }

            if (modelInstance == null)
            {
                var modelGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                modelGO.transform.SetParent(root.transform);
                modelGO.transform.localPosition = Vector3.zero;
                modelGO.name = "Model_Fallback";
                entity.ModelRoot = modelGO.transform;
            }

            var entityRef = root.GetComponent<HeroEntity>();
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            if (prefab != null)
            {
                Debug.Log($"Created prefab: {heroName} (ID: {heroId})");
                created++;
            }
            else
            {
                Debug.LogError($"Failed to create prefab for {heroName}");
            }

            Object.DestroyImmediate(root);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Hero prefabs created: {created}, skipped: {skipped}");
    }

    [MenuItem("Tools/Create Single Hero Prefab _%#h")]
    public static void CreateSingleHeroPrefab()
    {
        var selected = Selection.activeObject;
        if (selected == null) return;

        string path = AssetDatabase.GetAssetPath(selected);
        if (!AssetDatabase.IsValidFolder(path)) return;

        string folderName = Path.GetFileName(path);
        if (!HeroMapping.ContainsKey(folderName))
        {
            Debug.LogError($"Not a hero folder: {folderName}");
            return;
        }

        var (heroId, heroName) = HeroMapping[folderName];
        string outputDir = "Assets/Resources/Heroes";
        Directory.CreateDirectory(outputDir);

        var root = new GameObject(heroName);
        var entity = root.AddComponent<HeroEntity>();
        entity.HeroId = heroId;

        var controller = root.GetComponent<CharacterController>();
        controller.height = 1.8f;
        controller.radius = 0.4f;
        controller.center = new Vector3(0, 0.9f, 0);

        var animator = root.GetComponent<Animator>();
        var animCtrl = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
            $"Assets/Animators/{heroName}_Animator.controller");
        if (animCtrl != null) animator.runtimeAnimatorController = animCtrl;
        entity.Animator = animator;
        entity.Controller = controller;

        string modelsDir = Path.Combine(path, "models").Replace("\\", "/");
        if (Directory.Exists(modelsDir))
        {
            var glb = Directory.GetFiles(modelsDir, "*.glb")
                .OrderByDescending(f => new FileInfo(f).Length)
                .FirstOrDefault();
            if (glb != null)
            {
                var model = AssetDatabase.LoadAssetAtPath<GameObject>(glb.Replace("\\", "/"));
                if (model != null)
                {
                    var instance = (GameObject)PrefabUtility.InstantiatePrefab(model, root.transform);
                    instance.name = "Model";
                    instance.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                    entity.ModelRoot = instance.transform;
                }
            }
        }

        string prefabPath = $"{outputDir}/{heroName}.prefab";
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        Object.DestroyImmediate(root);
        AssetDatabase.Refresh();
        Debug.Log($"Created prefab: {heroName}");
    }
}
