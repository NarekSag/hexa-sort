using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Project.Scripts.Runtime.Utilities.Logging;
using _Project.Scripts.Runtime.Utilities.Persistence.Models;

namespace _Project.Scripts.Runtime.Utilities.Persistence
{
    public class SaveService
    {
        private const string SaveFileName = "gamesave.json";
        private readonly string _saveFilePath;

        public SaveService()
        {
            _saveFilePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        }

        public async UniTask SaveGameData(GameSaveData data)
        {
            try
            {
                CustomDebug.Log(LogCategory.Persistence, $"Saving game data: Level {data.CurrentLevel}");

                // Perform file I/O on background thread
                await UniTask.RunOnThreadPool(() =>
                {
                    // Ensure directory exists
                    string directory = Path.GetDirectoryName(_saveFilePath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Serialize to JSON
                    string json = JsonUtility.ToJson(data, true);

                    // Write to file
                    File.WriteAllText(_saveFilePath, json);
                });

                // Switch back to main thread
                await UniTask.SwitchToMainThread();

                CustomDebug.Log(LogCategory.Persistence, $"Game data saved successfully to: {_saveFilePath}");
            }
            catch (Exception e)
            {
                CustomDebug.LogError(LogCategory.Persistence, $"Failed to save game data: {e.Message}", e);
                throw;
            }
        }

        public async UniTask ResetSaveData()
        {
            try
            {
                CustomDebug.Log(LogCategory.Persistence, "Resetting save data...");

                // Perform file I/O on background thread
                await UniTask.RunOnThreadPool(() =>
                {
                    if (File.Exists(_saveFilePath))
                    {
                        File.Delete(_saveFilePath);
                    }
                });

                // Switch back to main thread
                await UniTask.SwitchToMainThread();

                CustomDebug.Log(LogCategory.Persistence, $"Save data reset successfully. File deleted: {_saveFilePath}");
            }
            catch (Exception e)
            {
                CustomDebug.LogError(LogCategory.Persistence, $"Failed to reset save data: {e.Message}", e);
                throw;
            }
        }
    }
}

