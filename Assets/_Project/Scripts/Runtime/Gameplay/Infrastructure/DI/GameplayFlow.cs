using System;
using UniRx;
using UnityEngine;
using VContainer.Unity;
using _Project.Scripts.Runtime.Utilities.Logging;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Factories;
using _Project.Scripts.Runtime.Gameplay.Domain.Level;
using _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Controllers;
using _Project.Scripts.Runtime.Gameplay.Presentation.Stack;
using _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Slot;
using _Project.Scripts.Runtime.Utilities.Persistence;
using _Project.Scripts.Runtime.Utilities.Persistence.Models;
using _Project.Scripts.Runtime.Gameplay.UI.Game;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.DI
{
    public class GameplayFlow : IStartable, IDisposable
    {
        private readonly HexGridFactory _hexGridFactory;
        private readonly HexSlot _slotPrefab;
        private readonly LevelManager _levelManager;
        private readonly SaveService _saveService;
        private readonly LoadService _loadService;
        private readonly GameViewModel _gameViewModel;
        private readonly GameView _gameView;
        
        private GridController _currentGridController;
        private readonly HexStackBoard _stackBoard;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        public GameplayFlow(
            HexGridFactory hexGridFactory, 
            HexSlot slotPrefab,
            LevelManager levelManager,
            HexStackBoard stackBoard,
            SaveService saveService,
            LoadService loadService,
            GameViewModel gameViewModel,
            GameView gameView)
        {
            _hexGridFactory = hexGridFactory;
            _slotPrefab = slotPrefab;
            _levelManager = levelManager;
            _stackBoard = stackBoard;
            _saveService = saveService;
            _loadService = loadService;
            _gameViewModel = gameViewModel;
            _gameView = gameView;
        }

        public async void Start()
        {
            try
            {
                _gameView.Initialize(_gameViewModel);
                
                // Subscribe to level events using reactive observables
                _levelManager.OnLevelStarted
                    .Subscribe(OnLevelStarted)
                    .AddTo(_disposables);
                
                _levelManager.OnLevelCompleted
                    .Subscribe(OnLevelCompleted)
                    .AddTo(_disposables);
                
                _levelManager.OnLevelFailed
                    .Subscribe(OnLevelFailed)
                    .AddTo(_disposables);
                
                // Load saved level
                var saveData = await _loadService.LoadGameData();
                _levelManager.StartLevel(saveData.CurrentLevel);
            }
            catch (Exception e)
            {
                CustomDebug.LogError(LogCategory.Gameplay, $"GameplayFlow Start Failed: {e.Message}");
            }
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
        }
        
        private void OnLevelStarted(Core.Models.LevelData levelData)
        {   
            ClearOldLevel();
            CreateNewLevel(levelData);
        }

        private void ClearOldLevel()
        {
            // Unsubscribe from old grid's cleanup service
            if (_currentGridController != null)
            {
                if (_currentGridController.CleanupService != null)
                {
                    _currentGridController.CleanupService.OnCellsCleared -= OnCellsCleared;
                }
                
                // Destroy old grid
                if (_currentGridController.GridTransform != null)
                {
                    GameObject.Destroy(_currentGridController.GridTransform.gameObject);
                }
            }
        }

        private void CreateNewLevel(Core.Models.LevelData levelData)
        {
            // Create new grid with level-specific dimensions
            _currentGridController = _hexGridFactory.Create(_slotPrefab, levelData);
            
            // Subscribe to new grid's cleanup service
            if (_currentGridController?.CleanupService != null)
            {
                _currentGridController.CleanupService.OnCellsCleared += OnCellsCleared;
            }
            
            // Initialize or update stack board with level data
            if (_stackBoard != null)
            {
                _stackBoard.Initialize(levelData);
            }
        }
        
        private void OnCellsCleared(int cellCount)
        {
            // Notify level manager how many cells were cleared
            _levelManager.NotifyCellsCleared(cellCount);
        }
        
        private async void OnLevelCompleted(Core.Models.LevelData levelData)
        {
            // TODO: Show victory UI
            
            // Save progress for next level
            await SaveProgress(levelData);
            
            // Auto-advance to next level (TODO: change to wait for button press)
            UnityEngine.Object.Destroy(_currentGridController.GridTransform.gameObject);
            _stackBoard?.ClearAllStacks();
            _levelManager.LoadNextLevel();
        }

        private async UniTask SaveProgress(Core.Models.LevelData levelData)
        {
            try
            {
                var saveData = new GameSaveData(levelData.LevelNumber + 1);
                await _saveService.SaveGameData(saveData);
            }
            catch (Exception e)
            {
                CustomDebug.LogError(LogCategory.Gameplay, $"Failed to save progress: {e.Message}");
            }
        }
        
        private void OnLevelFailed(Core.Models.LevelData levelData)
        {
            CustomDebug.Log(LogCategory.Gameplay, $"Level {levelData.LevelNumber} failed!");
            
            // TODO: Show failure UI
        }
    }
}

