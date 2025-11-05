using System;
using UniRx;
using _Project.Scripts.Runtime.Gameplay.Config;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Utilities.Logging;

namespace _Project.Scripts.Runtime.Gameplay.Domain.Level {
    public class LevelManager {
        private readonly LevelProgressionConfig _progressionConfig;
        
        private readonly ReactiveProperty<LevelData> _currentLevel = new ReactiveProperty<LevelData>();
        private readonly ReactiveProperty<int> _cellsCleared = new ReactiveProperty<int>(0);
        private readonly ReactiveProperty<float> _progress = new ReactiveProperty<float>(0f);
        
        private readonly Subject<LevelData> _levelStarted = new Subject<LevelData>();
        private readonly Subject<LevelData> _levelCompleted = new Subject<LevelData>();
        private readonly Subject<LevelData> _levelFailed = new Subject<LevelData>();
        
        // Reactive Properties
        public IReadOnlyReactiveProperty<LevelData> CurrentLevel => _currentLevel;
        public IReadOnlyReactiveProperty<int> CellsCleared => _cellsCleared;
        public IReadOnlyReactiveProperty<float> Progress => _progress;
        
        // Reactive Observables for events
        public IObservable<LevelData> OnLevelStarted => _levelStarted;
        public IObservable<LevelData> OnLevelCompleted => _levelCompleted;
        public IObservable<LevelData> OnLevelFailed => _levelFailed;
        
        public LevelManager(LevelProgressionConfig progressionConfig) {
            _progressionConfig = progressionConfig;
            
            // Update progress when cells cleared or level changes
            Observable.CombineLatest(_cellsCleared, _currentLevel, (cleared, level) => 
                level != null ? (float)cleared / level.CellsToClear : 0f)
                .Subscribe(progress => _progress.Value = progress);
        }
        
        public void StartLevel(int levelNumber) {
            if (_progressionConfig == null || !_progressionConfig.IsValid()) {
                CustomDebug.LogError(LogCategory.Gameplay, "LevelProgressionConfig is invalid!");
                return;
            }
            
            var levelData = _progressionConfig.GetLevelData(levelNumber);
            _currentLevel.Value = levelData;
            _cellsCleared.Value = 0;
            
            CustomDebug.Log(LogCategory.Gameplay, $"Starting {levelData}");
            
            _levelStarted.OnNext(levelData);
        }
        
        public void NotifyCellsCleared(int cellCount) {
            if (_currentLevel.Value == null) {
                CustomDebug.LogWarning(LogCategory.Gameplay, "Cells cleared but no level is active!");
                return;
            }
            
            _cellsCleared.Value += cellCount;
            
            CustomDebug.Log(LogCategory.Gameplay, 
                $"Cells cleared! {_cellsCleared.Value}/{_currentLevel.Value.CellsToClear}");
            
            if (_cellsCleared.Value >= _currentLevel.Value.CellsToClear) {
                CompleteLevel();
            }
        }
        
        private void CompleteLevel() {
            var levelData = _currentLevel.Value;
            CustomDebug.Log(LogCategory.Gameplay, $"Level {levelData.LevelNumber} completed!");
            
            _levelCompleted.OnNext(levelData);
        }
        
        public void FailLevel() {
            if (_currentLevel.Value == null) {
                return;
            }
            
            var levelData = _currentLevel.Value;
            CustomDebug.Log(LogCategory.Gameplay, $"Level {levelData.LevelNumber} failed!");
            
            _levelFailed.OnNext(levelData);
        }
        
        public void RestartLevel() {
            if (_currentLevel.Value != null) {
                int currentLevelNumber = _currentLevel.Value.LevelNumber;
                StartLevel(currentLevelNumber);
            }
        }
        
        public void LoadNextLevel() {
            if (_currentLevel.Value != null) {
                StartLevel(_currentLevel.Value.LevelNumber + 1);
            }
        }
    }
}

