using UniRx;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.State
{
    public class GameplayStateManager
    {
        private readonly ReactiveProperty<GameplayState> _currentState = new ReactiveProperty<GameplayState>(GameplayState.Playing);
        
        public IReadOnlyReactiveProperty<GameplayState> CurrentState => _currentState;
        
        public void SetState(GameplayState state)
        {
            _currentState.Value = state;
        }
    }
}

