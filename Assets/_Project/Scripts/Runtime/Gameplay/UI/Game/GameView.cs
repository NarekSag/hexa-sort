using UnityEngine;
using UniRx;
using _Project.Scripts.Runtime.Gameplay.UI.LevelProgression;
using _Project.Scripts.Runtime.Gameplay.UI.Settings;
using _Project.Scripts.Runtime.Gameplay.UI.LevelComplete;
using _Project.Scripts.Runtime.Gameplay.UI.LevelFailed;
using _Project.Scripts.Runtime.Gameplay.Domain.Level;

namespace _Project.Scripts.Runtime.Gameplay.UI.Game
{
    public class GameView : MonoBehaviour, IView<GameViewModel>
    {
        [SerializeField] private LevelProgressionView _levelProgressionView;
        [SerializeField] private SettingsView _settingsView;
        [SerializeField] private LevelCompletedView _levelCompleteView;
        [SerializeField] private LevelFailedView _levelFailedView;
        
        private GameViewModel _viewModel;
        private LevelManager _levelManager;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
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
            
            if (_levelCompleteView != null)
            {
                _levelCompleteView.Initialize(_viewModel.LevelCompleteViewModel);
            }
            
            if (_levelFailedView != null)
            {
                _levelFailedView.Initialize(_viewModel.LevelFailedViewModel);
            }
        }
        
        public void SetupLevelEvents(LevelManager levelManager)
        {
            _levelManager = levelManager;
            
            // Subscribe to level events
            _levelManager.OnLevelStarted
                .Subscribe(_ => OnLevelStarted())
                .AddTo(_disposables);
            
            _levelManager.OnLevelCompleted
                .Subscribe(_ => OnLevelCompleted())
                .AddTo(_disposables);
            
            _levelManager.OnLevelFailed
                .Subscribe(_ => OnLevelFailed())
                .AddTo(_disposables);
        }
        
        private void OnLevelStarted()
        {
            // Hide both panels when level starts
            if (_levelCompleteView != null)
            {
                _levelCompleteView.Hide();
            }
            
            if (_levelFailedView != null)
            {
                _levelFailedView.Hide();
            }
        }
        
        private void OnLevelCompleted()
        {
            // Show level complete panel
            if (_levelCompleteView != null)
            {
                _levelCompleteView.Show();
            }
            
            // Hide level failed panel
            if (_levelFailedView != null)
            {
                _levelFailedView.Hide();
            }
        }
        
        private void OnLevelFailed()
        {
            // Show level failed panel
            if (_levelFailedView != null)
            {
                _levelFailedView.Show();
            }
            
            // Hide level complete panel
            if (_levelCompleteView != null)
            {
                _levelCompleteView.Hide();
            }
        }
        
        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}

