using UnityEngine;
using UniRx;
using _Project.Scripts.Runtime.Gameplay.UI.LevelProgression;
using _Project.Scripts.Runtime.Gameplay.UI.Settings;
using _Project.Scripts.Runtime.Gameplay.UI.LevelComplete;
using _Project.Scripts.Runtime.Gameplay.UI.LevelFailed;
using _Project.Scripts.Runtime.Gameplay.UI.Boosters;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.State;
using _Project.Scripts.Runtime.Gameplay.Domain.Boosters;
using _Project.Scripts.Runtime.Gameplay.Domain.Level;

namespace _Project.Scripts.Runtime.Gameplay.UI.Game
{
    public class GameView : MonoBehaviour, IView<GameViewModel>
    {
        [SerializeField] private LevelProgressionView _levelProgressionView;
        [SerializeField] private SettingsView _settingsView;
        [SerializeField] private LevelCompletedView _levelCompleteView;
        [SerializeField] private LevelFailedView _levelFailedView;
        [SerializeField] private BoosterSelectionView _boosterSelectionView;
        [SerializeField] private BoosterView _boosterView;
        
        private GameViewModel _viewModel;
        private BoosterManager _boosterManager;
        private LevelManager _levelManager;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        // IView interface implementation
        public void Initialize(GameViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeViews();
        }
        
        // Overload with dependencies
        public void Initialize(GameViewModel viewModel, LevelManager levelManager)
        {
            _viewModel = viewModel;
            _levelManager = levelManager;
            _boosterManager = viewModel.BoosterManager;
            InitializeViews();
        }
        
        private void InitializeViews()
        {
            InitializeView(_levelProgressionView, _viewModel.LevelProgressionViewModel);
            InitializeView(_settingsView, _viewModel.SettingsViewModel);
            InitializeView(_levelCompleteView, _viewModel.LevelCompleteViewModel);
            InitializeView(_levelFailedView, _viewModel.LevelFailedViewModel);
            InitializeView(_boosterView, _viewModel.BoosterViewModel);
            
            // Initialize booster selection view with dependencies
            if (_boosterManager != null && _levelManager != null)
            {
                _boosterSelectionView?.Initialize(_viewModel.BoosterSelectionViewModel, _boosterManager, _levelManager);
            }
            else
            {
                _boosterSelectionView?.Initialize(_viewModel.BoosterSelectionViewModel);
            }
        }
        
        public void SetupStateManager(GameplayStateManager stateManager)
        {
            stateManager.CurrentState.Subscribe(OnGameplayStateChanged).AddTo(_disposables);
        }
        
        private void InitializeView<T>(IView<T> view, T viewModel) where T : IViewModel
        {
            view?.Initialize(viewModel);
        }
        
        private void OnGameplayStateChanged(GameplayState state)
        {
            // Handle booster views
            bool isBoosterActive = state == GameplayState.BoosterActive;
            if (isBoosterActive)
            {
                _boosterSelectionView?.Hide();
                _boosterView?.Show();
            }
            else
            {
                _boosterSelectionView?.Show();
                _boosterView?.Hide();
            }
            
            // Handle level complete/failed views
            switch (state)
            {
                case GameplayState.Playing:
                    _levelCompleteView?.Hide();
                    _levelFailedView?.Hide();
                    break;
                    
                case GameplayState.LevelCompleted:
                    _levelCompleteView?.Show();
                    _levelFailedView?.Hide();
                    break;
                    
                case GameplayState.LevelFailed:
                    _levelFailedView?.Show();
                    _levelCompleteView?.Hide();
                    break;
            }
        }
        
        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}

