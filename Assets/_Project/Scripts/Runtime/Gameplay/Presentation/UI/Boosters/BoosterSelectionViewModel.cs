using System.Collections.Generic;
using System.Linq;
using UniRx;
using _Project.Scripts.Runtime.Gameplay.Domain.Boosters;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.State;
using _Project.Scripts.Runtime.Gameplay.Presentation.Stack;

namespace _Project.Scripts.Runtime.Gameplay.UI.Boosters
{
    public class BoosterSelectionViewModel : IViewModel
    {
        private readonly BoosterManager _boosterManager;
        private readonly GameplayStateManager _stateManager;
        private readonly HexStackBoard _stackBoard;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly ReadOnlyReactiveProperty<IEnumerable<IBooster>> _availableBoosters;

        public IReadOnlyReactiveProperty<IEnumerable<IBooster>> AvailableBoosters => _availableBoosters;

        public BoosterSelectionViewModel(
            BoosterManager boosterManager,
            GameplayStateManager stateManager,
            HexStackBoard stackBoard)
        {
            _boosterManager = boosterManager;
            _stateManager = stateManager;
            _stackBoard = stackBoard;

            // Create reactive property for available boosters
            // Update when state changes OR when boosters are reset
            _availableBoosters = Observable.Merge(
                    _stateManager.CurrentState.Select(_ => Unit.Default),
                    _boosterManager.OnBoostersReset
                )
                .Select(_ => _boosterManager.GetAvailableBoosters().ToList().AsEnumerable())
                .ToReadOnlyReactiveProperty(_boosterManager.GetAvailableBoosters())
                .AddTo(_disposables);
        }

        public void SelectBooster(IBooster booster)
        {
            if (booster == null || !booster.CanUse())
            {
                return;
            }

            // Check if the booster is unlocked
            if (!_boosterManager.IsBoosterUnlocked(booster.Type))
            {
                return;
            }

            // Handle immediate-use boosters (like Shuffle) vs click-based boosters (like Hammer)
            if (booster is ShuffleBooster)
            {
                // Execute shuffle immediately
                _stackBoard.ShuffleStacks();

                // Mark booster as used
                _boosterManager.MarkBoosterAsUsed(booster.Id);

                // Booster is already used, no need to set active or change state
            }
            else
            {
                // Click-based boosters (Hammer) require user interaction
                _boosterManager.SetActiveBooster(booster);
                booster.Use();
                _stateManager.SetState(GameplayState.BoosterActive);
            }
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}

