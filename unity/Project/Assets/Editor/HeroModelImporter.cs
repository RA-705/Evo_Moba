using UnityEngine;
using UnityEditor;
using System.IO;

namespace Evo.Editor
{
    public static class HeroModelImporter
    {
        [MenuItem("Evo/Generate Hero Prefabs")]
        public static void GeneratePrefabs()
        {
            string modelsRoot = "Assets/Models/Heroes";
            string outputDir = "Assets/Resources/Heroes";
            Directory.CreateDirectory(outputDir);

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.StartAssetEditing();

            var heroDirs = Directory.GetDirectories(modelsRoot);
            foreach (var dir in heroDirs)
            {
                string dirName = Path.GetFileName(dir);
                string heroIdStr = dirName.Substring(0, 3);
                if (!int.TryParse(heroIdStr, out int heroId)) continue;

                string meshDir = Path.Combine(dir, "Meshes");
                string texDir = Path.Combine(dir, "Textures");
                if (!Directory.Exists(meshDir)) continue;

                var meshFiles = Directory.GetFiles(meshDir, "*.obj");
                if (meshFiles.Length == 0) continue;

                string mainMesh = null;
                foreach (var m in meshFiles)
                {
                    string name = Path.GetFileNameWithoutExtension(m);
                    if (name.Contains("Mid") && (name.Contains("_1_") || name.EndsWith("_Mid") || name.Contains("1_")))
                    { mainMesh = m; break; }
                }
                if (mainMesh == null) mainMesh = meshFiles[0];

                var allMeshes = AssetDatabase.FindAssets("t:Mesh", new[] { $"Assets/Models/Heroes/{dirName}" });
                string meshPath = null;
                foreach (var guid in allMeshes)
                {
                    var p = AssetDatabase.GUIDToAssetPath(guid);
                    if (!p.EndsWith(".obj", System.StringComparison.OrdinalIgnoreCase)) continue;
                    string name = Path.GetFileNameWithoutExtension(p);
                    if ((name.Contains("Mid") || name.Contains("_1_") || name.Contains("1_")))
                    { meshPath = p; break; }
                }
                if (meshPath == null)
                {
                    foreach (var guid in allMeshes)
                    {
                        var p = AssetDatabase.GUIDToAssetPath(guid);
                        if (!p.EndsWith(".obj", System.StringComparison.OrdinalIgnoreCase)) continue;
                        meshPath = p; break;
                    }
                }
                if (meshPath == null) { Debug.LogWarning($"No .obj mesh for {dirName}"); continue; }
                Debug.Log($"Loading mesh: {meshPath}");
                var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
                if (mesh == null) { Debug.LogWarning($"Failed to load mesh at {meshPath}"); continue; }

                var go = new GameObject($"Hero_{heroId:D3}");
                var mf = go.AddComponent<MeshFilter>();
                mf.sharedMesh = mesh;
                var mr = go.AddComponent<MeshRenderer>();

                var allTexs = AssetDatabase.FindAssets("t:Texture2D", new[] { $"Assets/Models/Heroes/{dirName}/Textures" });
                Texture2D tex = null;
                foreach (var tguid in allTexs)
                {
                    var tp = AssetDatabase.GUIDToAssetPath(tguid);
                    tex = AssetDatabase.LoadAssetAtPath<Texture2D>(tp);
                    if (tex != null) { Debug.Log($"Loaded texture: {tp}"); break; }
                }
                if (tex != null)
                {
                    var mat = new Material(Shader.Find("Standard"));
                    mat.mainTexture = tex;
                    mr.sharedMaterial = mat;
                }

                string prefabPath = Path.Combine(outputDir, $"{dirName}.prefab").Replace("\\", "/");
                PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                Object.DestroyImmediate(go);
                Debug.Log($"Generated prefab: {prefabPath}");
            }

            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Hero prefab generation complete!");
        }

        private static string FindTexture(string texDir, string dirName, string heroIdStr)
        {
            string heroName = dirName.Substring(4);
            string[] candidates = {
                Path.Combine(texDir, $"{heroName}.png"),
                Path.Combine(texDir, $"{heroIdStr}1_{heroName}_D.png"),
                Path.Combine(texDir, $"{heroIdStr}2_{heroName}_D.png"),
                Path.Combine(texDir, $"{heroName}_D.png"),
                Path.Combine(texDir, $"Hero{heroIdStr}_{heroName}.png"),
                Path.Combine(texDir, $"Hero{heroIdStr}_s.png"),
            };

            foreach (var candidate in candidates)
            {
                if (File.Exists(candidate))
                {
                    string rel = "Assets" + candidate.Substring(Application.dataPath.Length);
                    return rel.Replace("\\", "/").Replace("//", "/");
                }
            }

            var allTexFiles = Directory.GetFiles(texDir, "*.png");
            if (allTexFiles.Length > 0)
            {
                string rel = "Assets" + allTexFiles[0].Substring(Application.dataPath.Length);
                return rel.Replace("\\", "/").Replace("//", "/");
            }
            return null;
        }
    }
}
