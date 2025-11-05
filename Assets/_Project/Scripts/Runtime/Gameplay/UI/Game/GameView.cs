using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.UI.LevelProgression;
using _Project.Scripts.Runtime.Gameplay.UI.Settings;

namespace _Project.Scripts.Runtime.Gameplay.UI.Game
{
    public class GameView : MonoBehaviour, IView<GameViewModel>
    {
        [SerializeField] private LevelProgressionView _levelProgressionView;
        [SerializeField] private SettingsView _settingsView;
        
        private GameViewModel _viewModel;
        
        public void Initialize(GameViewModel viewModel)
        {
            _viewModel = viewModel;
            
            // Initialize child views with their respective ViewModels
            if (_levelProgressionView != null)
            {
                _levelProgressionView.Initialize(_viewModel.LevelProgressionViewModel);
            }
            
            if (_settingsView != null)
            {
                _settingsView.Initialize(_viewModel.SettingsViewModel);
            }
        }
    }
}

