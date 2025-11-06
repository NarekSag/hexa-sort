using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Project.Scripts.Editor
{
    /// <summary>
    /// Ensures shader variant collection is preloaded in graphics settings
    /// </summary>
    [InitializeOnLoad]
    public static class ShaderVariantPreloader
    {
        static ShaderVariantPreloader()
        {
            // Auto-setup on editor load
            EditorApplication.delayCall += SetupShaderVariantCollection;
        }

        [MenuItem("Tools/Fix Materials/Setup Shader Variant Preloading")]
        public static void SetupShaderVariantCollection()
        {
            string shaderVariantPath = "Assets/_Project/Settings/RequiredShaderVariants.shadervariants";
            ShaderVariantCollection collection = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(shaderVariantPath);

            if (collection == null)
            {
                Debug.LogWarning($"Shader Variant Collection not found at: {shaderVariantPath}");
                return;
            }

            SerializedObject graphicsSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
            SerializedProperty preloadedShaders = graphicsSettings.FindProperty("m_PreloadedShaders");

            // Check if already added
            for (int i = 0; i < preloadedShaders.arraySize; i++)
            {
                if (preloadedShaders.GetArrayElementAtIndex(i).objectReferenceValue == collection)
                {
                    Debug.Log("Shader Variant Collection already in preloaded shaders.");
                    return;
                }
            }

            // Add to preloaded shaders
            preloadedShaders.InsertArrayElementAtIndex(preloadedShaders.arraySize);
            preloadedShaders.GetArrayElementAtIndex(preloadedShaders.arraySize - 1).objectReferenceValue = collection;
            
            graphicsSettings.ApplyModifiedProperties();
            
            Debug.Log("<color=green>✓ Shader Variant Collection added to Graphics Settings!</color>");
        }
    }
}

