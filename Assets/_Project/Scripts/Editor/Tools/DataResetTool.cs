using System.IO;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Editor.Tools
{
    public static class DataResetTool
    {
        private const string SaveFileName = "gamesave.json";
        
        [MenuItem("Tools/Custom/Reset Data")]
        public static void ResetGameData()
        {
            string saveFilePath = Path.Combine(Application.persistentDataPath, SaveFileName);
            
            if (!File.Exists(saveFilePath))
            {
                EditorUtility.DisplayDialog(
                    "Reset Data",
                    "No save data found to reset.",
                    "OK");
                return;
            }
            
            bool confirmed = EditorUtility.DisplayDialog(
                "Reset Data",
                $"Are you sure you want to delete the save data?\n\nFile: {saveFilePath}",
                "Yes, Reset",
                "Cancel");
            
            if (confirmed)
            {
                try
                {
                    File.Delete(saveFilePath);
                    Debug.Log($"[DataResetTool] Save data reset successfully. File deleted: {saveFilePath}");
                    
                    EditorUtility.DisplayDialog(
                        "Reset Data",
                        "Save data has been reset successfully!\n\nThe game will start from level 1 on next play.",
                        "OK");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[DataResetTool] Failed to reset save data: {e.Message}");
                    
                    EditorUtility.DisplayDialog(
                        "Reset Data Failed",
                        $"Failed to reset save data:\n\n{e.Message}",
                        "OK");
                }
            }
        }
        
        [MenuItem("Tools/Custom/Show Save Location")]
        public static void ShowSaveLocation()
        {
            string saveFilePath = Path.Combine(Application.persistentDataPath, SaveFileName);
            string directory = Path.GetDirectoryName(saveFilePath);
            
            bool exists = File.Exists(saveFilePath);
            string message = exists 
                ? $"Save file exists at:\n\n{saveFilePath}" 
                : $"No save file found.\n\nExpected location:\n{saveFilePath}";
            
            EditorUtility.DisplayDialog(
                "Save File Location",
                message,
                "OK");
            
            // Try to reveal in finder/explorer
            if (Directory.Exists(directory))
            {
                EditorUtility.RevealInFinder(saveFilePath);
            }
        }
    }
}

