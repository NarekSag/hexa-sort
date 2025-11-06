using UniRx;
using _Project.Scripts.Runtime.Gameplay.Domain.Level;
using _Project.Scripts.Runtime.Gameplay.UI;

namespace _Project.Scripts.Runtime.Gameplay.UI.LevelComplete
{
    public class LevelCompletedViewModel : IViewModel
    {
        private readonly LevelManager _levelManager;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        private readonly ReadOnlyReactiveProperty<string> _levelCompleteText;
        
        public IReadOnlyReactiveProperty<string> LevelCompleteText => _levelCompleteText;
        
        public LevelCompletedViewModel(LevelManager levelManager)
        {
            _levelManager = levelManager;
            
            // Create level complete text: "Congratulations!\nLevel X is complete"
            _levelCompleteText = _levelManager.CurrentLevel
                .Select(level => level != null 
                    ? $"Congratulations!\nLevel {level.LevelNumber} is complete"
                    : "Congratulations!\nLevel is complete")
                .ToReadOnlyReactiveProperty("Congratulations!\nLevel is complete")
                .AddTo(_disposables);
        }
        
        public void LoadNextLevel()
        {
            _levelManager.LoadNextLevel();
        }
        
        public void GoHome()
        {
            UnityEngine.Debug.Log("Go home button pressed");
            // TODO: Implement navigation to home screen
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}

