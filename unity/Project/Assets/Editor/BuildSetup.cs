using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace Evo.UnityClient.Editor
{
    public class BuildSetup
    {
        [MenuItem("Evo/Setup Project")]
        public static void SetupProject()
        {
            // Set active scene
            var scenes = new[]
            {
                "Assets/Scenes/level1.unity"
            };
            
            // Create build settings
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/level1.unity", true)
            };

            // Set scripting define symbols
            var defines = "EVO_CLIENT;LITE_NET_LIB";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defines);
            
            // Set API compatibility
            // PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_Standard_2_1);
            
            // Set scripting backend
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
            
            Debug.Log("[Evo] Project setup complete!");
        }

        [MenuItem("Evo/Create Scene References")]
        public static void CreateSceneReferences()
        {
            // Ensure all scenes have proper references
            var sceneGuids = AssetDatabase.FindAssets("t:Scene");
            
            foreach (var guid in sceneGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log($"Found scene: {path}");
            }
        }
    }
}