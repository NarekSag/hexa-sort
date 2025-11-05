using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Project.Scripts.Runtime.Utilities.Logging;
using _Project.Scripts.Runtime.Utilities.Persistence.Models;

namespace _Project.Scripts.Runtime.Utilities.Persistence
{
    public class LoadService
    {
        private const string SaveFileName = "gamesave.json";
        private readonly string _saveFilePath;

        public LoadService()
        {
            _saveFilePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        }

        public async UniTask<GameSaveData> LoadGameData()
        {
            try
            {
                CustomDebug.Log(LogCategory.Persistence, $"Loading game data from: {_saveFilePath}");

                // Check if save file exists
                if (!File.Exists(_saveFilePath))
                {
                    CustomDebug.Log(LogCategory.Persistence, "No save file found. Returning default data.");
                    return new GameSaveData();
                }

                GameSaveData data = null;

                // Perform file I/O on background thread
                await UniTask.RunOnThreadPool(() =>
                {
                    // Read file
                    string json = File.ReadAllText(_saveFilePath);

                    // Deserialize from JSON
                    data = JsonUtility.FromJson<GameSaveData>(json);
                });

                // Switch back to main thread
                await UniTask.SwitchToMainThread();

                // Validate loaded data
                if (data == null || data.CurrentLevel < 1)
                {
                    CustomDebug.LogWarning(LogCategory.Persistence, "Invalid save data. Returning default data.");
                    return new GameSaveData();
                }

                CustomDebug.Log(LogCategory.Persistence, $"Game data loaded successfully: Level {data.CurrentLevel}");
                return data;
            }
            catch (Exception e)
            {
                CustomDebug.LogError(LogCategory.Persistence, $"Failed to load game data: {e.Message}. Returning default data.", e);
                return new GameSaveData();
            }
        }
    }
}

