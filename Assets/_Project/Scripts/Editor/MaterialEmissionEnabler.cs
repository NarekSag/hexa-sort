using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Editor
{
    /// <summary>
    /// Utility to enable emission on materials to ensure shader variants are included in builds
    /// </summary>
    public static class MaterialEmissionEnabler
    {
        [MenuItem("Tools/Fix Materials/Enable Emission on All Cell Materials")]
        public static void EnableEmissionOnCellMaterials()
        {
            string[] materialPaths = new[]
            {
                "Assets/_Project/Art/Materials/CellColors/Blue.mat",
                "Assets/_Project/Art/Materials/CellColors/Cyan.mat",
                "Assets/_Project/Art/Materials/CellColors/Default.mat",
                "Assets/_Project/Art/Materials/CellColors/Green.mat",
                "Assets/_Project/Art/Materials/CellColors/Orange.mat",
                "Assets/_Project/Art/Materials/CellColors/Pink.mat",
                "Assets/_Project/Art/Materials/CellColors/Purple.mat",
                "Assets/_Project/Art/Materials/CellColors/Red.mat",
                "Assets/_Project/Art/Materials/CellColors/Yellow.mat"
            };

            int successCount = 0;
            foreach (string path in materialPaths)
            {
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material != null)
                {
                    if (material.HasProperty("_EmissionColor"))
                    {
                        // Enable emission keyword
                        material.EnableKeyword("_EMISSION");
                        
                        // Set Global Illumination flags to support emission
                        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                        
                        // Set a very low emission color so it doesn't show by default
                        material.SetColor("_EmissionColor", Color.black);
                        
                        EditorUtility.SetDirty(material);
                        successCount++;
                        Debug.Log($"Enabled emission on: {material.name}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Could not load material at: {path}");
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"<color=green>Successfully enabled emission on {successCount} materials!</color>");
        }

        [MenuItem("Tools/Fix Materials/Scan and Fix All Materials with Emission Property")]
        public static void ScanAndFixAllMaterials()
        {
            string[] allMaterialGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/_Project" });
            int successCount = 0;

            foreach (string guid in allMaterialGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
                
                if (material != null && material.HasProperty("_EmissionColor"))
                {
                    material.EnableKeyword("_EMISSION");
                    material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                    material.SetColor("_EmissionColor", Color.black);
                    EditorUtility.SetDirty(material);
                    successCount++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"<color=green>Scanned and fixed {successCount} materials with emission support!</color>");
        }
    }
}

