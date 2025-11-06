using UniRx;
using _Project.Scripts.Runtime.Gameplay.Domain.Level;
using _Project.Scripts.Runtime.Gameplay.UI;

namespace _Project.Scripts.Runtime.Gameplay.UI.LevelFailed
{
    public class LevelFailedViewModel : IViewModel
    {
        private readonly LevelManager _levelManager;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        private readonly ReadOnlyReactiveProperty<int> _currentCellsCleared;
        private readonly ReadOnlyReactiveProperty<int> _goalCells;
        private readonly ReadOnlyReactiveProperty<string> _progressValueText;
        
        public string HeaderText => "LEVEL FAILED";
        public string DescriptionText => "OUT OF SPACE";
        public string ProgressText => "PROGRESS:";
        public IReadOnlyReactiveProperty<string> ProgressValueText => _progressValueText;
        
        public LevelFailedViewModel(LevelManager levelManager)
        {
            _levelManager = levelManager;
            
            // Current cells cleared
            _currentCellsCleared = _levelManager.CellsCleared
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);
            
            // Goal cells (from current level)
            _goalCells = _levelManager.CurrentLevel
                .Select(level => level != null ? level.CellsToClear : 0)
                .ToReadOnlyReactiveProperty(0)
                .AddTo(_disposables);
            
            // Progress value text (format: "100/250")
            _progressValueText = Observable.CombineLatest(
                    _currentCellsCleared,
                    _goalCells,
                    (current, goal) => $"{current}/{goal}")
                .ToReadOnlyReactiveProperty("0/0")
                .AddTo(_disposables);
        }
        
        public void RestartLevel()
        {
            _levelManager.RestartLevel();
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

