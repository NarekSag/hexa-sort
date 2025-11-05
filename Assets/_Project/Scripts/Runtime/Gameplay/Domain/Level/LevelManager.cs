using System;
using _Project.Scripts.Runtime.Gameplay.Config;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Utilities.Logging;

namespace _Project.Scripts.Runtime.Gameplay.Domain.Level {
    public class LevelManager {
        private readonly LevelProgressionConfig _progressionConfig;
        
        private LevelData _currentLevel;
        private int _cellsCleared = 0;
        
        public event Action<LevelData> OnLevelStarted;
        public event Action<int> OnCellsCleared;
        public event Action<LevelData> OnLevelCompleted;
        public event Action<LevelData> OnLevelFailed;
        
        public LevelData CurrentLevel => _currentLevel;
        public int CurrentLevelNumber => _currentLevel != null ? _currentLevel.LevelNumber : 1;
        public int CellsCleared => _cellsCleared;
        public int CellsRemaining => _currentLevel != null ? _currentLevel.CellsToClear - _cellsCleared : 0;
        public bool IsLevelActive => _currentLevel != null;
        
        public LevelManager(LevelProgressionConfig progressionConfig) {
            _progressionConfig = progressionConfig;
        }
        
        public void StartLevel(int levelNumber) {
            if (_progressionConfig == null || !_progressionConfig.IsValid()) {
                CustomDebug.LogError(LogCategory.Gameplay, "LevelProgressionConfig is invalid!");
                return;
            }
            
            _currentLevel = _progressionConfig.GetLevelData(levelNumber);
            _cellsCleared = 0;
            
            CustomDebug.Log(LogCategory.Gameplay, $"Starting {_currentLevel}");
            OnLevelStarted?.Invoke(_currentLevel);
        }
        
        public void NotifyCellsCleared(int cellCount) {
            if (_currentLevel == null) {
                CustomDebug.LogWarning(LogCategory.Gameplay, "Cells cleared but no level is active!");
                return;
            }
            
            _cellsCleared += cellCount;
            OnCellsCleared?.Invoke(_cellsCleared);
            
            CustomDebug.Log(LogCategory.Gameplay, 
                $"Cells cleared! {_cellsCleared}/{_currentLevel.CellsToClear}");
            
            if (_cellsCleared >= _currentLevel.CellsToClear) {
                CompleteLevel();
            }
        }
        
        private void CompleteLevel() {
            CustomDebug.Log(LogCategory.Gameplay, $"Level {_currentLevel.LevelNumber} completed!");
            OnLevelCompleted?.Invoke(_currentLevel);
        }
        
        public void FailLevel() {
            if (_currentLevel == null) {
                return;
            }
            
            CustomDebug.Log(LogCategory.Gameplay, $"Level {_currentLevel.LevelNumber} failed!");
            OnLevelFailed?.Invoke(_currentLevel);
        }
        
        public void RestartLevel() {
            if (_currentLevel != null) {
                int currentLevelNumber = _currentLevel.LevelNumber;
                StartLevel(currentLevelNumber);
            }
        }
        
        public void LoadNextLevel() {
            if (_currentLevel != null) {
                StartLevel(_currentLevel.LevelNumber + 1);
            }
        }
    }
}

