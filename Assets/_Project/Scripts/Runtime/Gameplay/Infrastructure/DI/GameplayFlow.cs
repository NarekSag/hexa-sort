using System;
using UnityEngine;
using VContainer.Unity;
using _Project.Scripts.Runtime.Utilities.Logging;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Factories;
using _Project.Scripts.Runtime.Gameplay.Domain.Level;
using _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Controllers;
using _Project.Scripts.Runtime.Gameplay.Presentation.Stack;
using _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Slot;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.DI
{
    public class GameplayFlow : IStartable
    {
        private readonly HexGridFactory _hexGridFactory;
        private readonly HexSlot _slotPrefab;
        private readonly LevelManager _levelManager;
        
        private GridController _currentGridController;
        private HexStackBoard _stackBoard;
        
        public GameplayFlow(
            HexGridFactory hexGridFactory, 
            HexSlot slotPrefab,
            LevelManager levelManager,
            HexStackBoard stackBoard)
        {
            _hexGridFactory = hexGridFactory;
            _slotPrefab = slotPrefab;
            _levelManager = levelManager;
            _stackBoard = stackBoard;
        }

        public void Start()
        {
            try
            {
                _levelManager.OnLevelStarted += OnLevelStarted;
                _levelManager.OnLevelCompleted += OnLevelCompleted;
                _levelManager.OnLevelFailed += OnLevelFailed;
                
                //TODO: Get current level from level progression config
                _levelManager.StartLevel(1);
                
                CustomDebug.Log(LogCategory.Gameplay, "GameplayFlow started");
            }
            catch (Exception e)
            {
                CustomDebug.LogError(LogCategory.Gameplay, $"GameplayFlow Start Failed: {e.Message}");
            }
        }
        
        private void OnLevelStarted(Core.Models.LevelData levelData)
        {
            CustomDebug.Log(LogCategory.Gameplay, $"Level {levelData.LevelNumber} started: {levelData}");
            
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
        
        private void OnLevelCompleted(Core.Models.LevelData levelData)
        {
            CustomDebug.Log(LogCategory.Gameplay, $"Level {levelData.LevelNumber} completed!");
            
            // TODO: Show victory UI
            
            // Auto-advance to next level (TODO: change to wait for button press)
            UnityEngine.Object.Destroy(_currentGridController.GridTransform.gameObject);
            _stackBoard?.ClearAllStacks();
            _levelManager.LoadNextLevel();
        }
        
        private void OnLevelFailed(Core.Models.LevelData levelData)
        {
            CustomDebug.Log(LogCategory.Gameplay, $"Level {levelData.LevelNumber} failed!");
            
            // TODO: Show failure UI
        }
    }
}

