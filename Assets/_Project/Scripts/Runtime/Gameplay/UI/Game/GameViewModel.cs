using _Project.Scripts.Runtime.Gameplay.Domain.Level;
using _Project.Scripts.Runtime.Gameplay.UI.LevelProgression;
using _Project.Scripts.Runtime.Gameplay.UI.Settings;
using _Project.Scripts.Runtime.Gameplay.UI.LevelComplete;
using _Project.Scripts.Runtime.Gameplay.UI.LevelFailed;

namespace _Project.Scripts.Runtime.Gameplay.UI.Game
{
    public class GameViewModel : IViewModel
    {
        public LevelProgressionViewModel LevelProgressionViewModel { get; }
        public SettingsViewModel SettingsViewModel { get; }
        public LevelCompletedViewModel LevelCompleteViewModel { get; }
        public LevelFailedViewModel LevelFailedViewModel { get; }
        
        public GameViewModel(LevelManager levelManager)
        {
            LevelProgressionViewModel = new LevelProgressionViewModel(levelManager);
            SettingsViewModel = new SettingsViewModel(levelManager);
            LevelCompleteViewModel = new LevelCompletedViewModel(levelManager);
            LevelFailedViewModel = new LevelFailedViewModel(levelManager);
        }
        
        public void Dispose()
        {
            LevelProgressionViewModel?.Dispose();
            SettingsViewModel?.Dispose();
            LevelCompleteViewModel?.Dispose();
            LevelFailedViewModel?.Dispose();
        }
    }
}

