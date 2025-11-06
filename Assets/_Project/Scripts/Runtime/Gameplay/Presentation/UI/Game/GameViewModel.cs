using _Project.Scripts.Runtime.Gameplay.Domain.Level;
using _Project.Scripts.Runtime.Gameplay.UI.LevelProgression;
using _Project.Scripts.Runtime.Gameplay.UI.Settings;
using _Project.Scripts.Runtime.Gameplay.UI.LevelComplete;
using _Project.Scripts.Runtime.Gameplay.UI.LevelFailed;
using _Project.Scripts.Runtime.Gameplay.UI.Boosters;
using _Project.Scripts.Runtime.Gameplay.Domain.Boosters;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.State;
using _Project.Scripts.Runtime.Gameplay.Presentation.Stack;

namespace _Project.Scripts.Runtime.Gameplay.UI.Game
{
    public class GameViewModel : IViewModel
    {
        public LevelProgressionViewModel LevelProgressionViewModel { get; }
        public SettingsViewModel SettingsViewModel { get; }
        public LevelCompletedViewModel LevelCompleteViewModel { get; }
        public LevelFailedViewModel LevelFailedViewModel { get; }
        public BoosterSelectionViewModel BoosterSelectionViewModel { get; }
        public BoosterViewModel BoosterViewModel { get; }
        public BoosterManager BoosterManager { get; }
        
        public GameViewModel(
            LevelManager levelManager,
            BoosterManager boosterManager,
            GameplayStateManager stateManager,
            HexStackBoard stackBoard)
        {
            BoosterManager = boosterManager;
            LevelProgressionViewModel = new LevelProgressionViewModel(levelManager);
            SettingsViewModel = new SettingsViewModel(levelManager);
            LevelCompleteViewModel = new LevelCompletedViewModel(levelManager);
            LevelFailedViewModel = new LevelFailedViewModel(levelManager);
            BoosterSelectionViewModel = new BoosterSelectionViewModel(boosterManager, stateManager, stackBoard);
            BoosterViewModel = new BoosterViewModel(boosterManager, stateManager);
        }
        
        public void Dispose()
        {
            LevelProgressionViewModel?.Dispose();
            SettingsViewModel?.Dispose();
            LevelCompleteViewModel?.Dispose();
            LevelFailedViewModel?.Dispose();
            BoosterSelectionViewModel?.Dispose();
            BoosterViewModel?.Dispose();
        }
    }
}

