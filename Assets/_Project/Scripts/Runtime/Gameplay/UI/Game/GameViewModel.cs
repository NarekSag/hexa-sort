using _Project.Scripts.Runtime.Gameplay.Domain.Level;
using _Project.Scripts.Runtime.Gameplay.UI.LevelProgression;
using _Project.Scripts.Runtime.Gameplay.UI.Settings;

namespace _Project.Scripts.Runtime.Gameplay.UI.Game
{
    public class GameViewModel : IViewModel
    {
        public LevelProgressionViewModel LevelProgressionViewModel { get; }
        public SettingsViewModel SettingsViewModel { get; }
        
        public GameViewModel(LevelManager levelManager)
        {
            LevelProgressionViewModel = new LevelProgressionViewModel(levelManager);
            SettingsViewModel = new SettingsViewModel(levelManager);
        }
        
        public void Dispose()
        {
            LevelProgressionViewModel?.Dispose();
            SettingsViewModel?.Dispose();
        }
    }
}

