using UniRx;
using _Project.Scripts.Runtime.Gameplay.Domain.Boosters;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.State;
using _Project.Scripts.Runtime.Gameplay.UI;

namespace _Project.Scripts.Runtime.Gameplay.UI.Boosters
{
    public class BoosterViewModel : IViewModel
    {
        private readonly BoosterManager _boosterManager;
        private readonly GameplayStateManager _stateManager;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        private readonly ReadOnlyReactiveProperty<IBooster> _activeBooster;
        private readonly ReadOnlyReactiveProperty<string> _description;
        
        public IReadOnlyReactiveProperty<IBooster> ActiveBooster => _activeBooster;
        public IReadOnlyReactiveProperty<string> Description => _description;
        
        public BoosterViewModel(BoosterManager boosterManager, GameplayStateManager stateManager)
        {
            _boosterManager = boosterManager;
            _stateManager = stateManager;
            
            // Create reactive property for active booster
            // Update when state becomes BoosterActive
            _activeBooster = _stateManager.CurrentState
                .Select(state => state == GameplayState.BoosterActive ? _boosterManager.ActiveBooster : null)
                .ToReadOnlyReactiveProperty(null)
                .AddTo(_disposables);
            
            // Description from active booster
            _description = _activeBooster
                .Select(booster => booster != null ? booster.Description : string.Empty)
                .ToReadOnlyReactiveProperty(string.Empty)
                .AddTo(_disposables);
        }
        
        public void Close()
        {
            _boosterManager.ClearActiveBooster();
            _stateManager.SetState(GameplayState.Playing);
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}

