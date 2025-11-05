using UniRx;
using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Domain.Level;

namespace _Project.Scripts.Runtime.Gameplay.UI.Settings
{
    public class SettingsViewModel : IViewModel
    {
        private readonly LevelManager _levelManager;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        private readonly ReactiveProperty<bool> _isSettingsMenuOpen = new ReactiveProperty<bool>(false);
        
        public IReadOnlyReactiveProperty<bool> IsSettingsMenuOpen => _isSettingsMenuOpen;
        
        public SettingsViewModel(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }
        
        public void ToggleSettingsMenu()
        {
            _isSettingsMenuOpen.Value = !_isSettingsMenuOpen.Value;
        }
        
        public void CloseSettingsMenu()
        {
            _isSettingsMenuOpen.Value = false;
        }
        
        public void RestartLevel()
        {
            _levelManager.RestartLevel();
            CloseSettingsMenu();
        }
        
        public void GoHome()
        {
            Debug.Log("home button pressed");
            CloseSettingsMenu();
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
            _isSettingsMenuOpen?.Dispose();
        }
    }
}

