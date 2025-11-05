using UniRx;
using _Project.Scripts.Runtime.Gameplay.Domain.Level;

namespace _Project.Scripts.Runtime.Gameplay.UI.LevelProgression
{
    public class LevelProgressionViewModel : IViewModel
    {
        private readonly LevelManager _levelManager;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        private readonly ReadOnlyReactiveProperty<int> _currentCellsCleared;
        private readonly ReadOnlyReactiveProperty<int> _goalCells;
        private readonly ReadOnlyReactiveProperty<string> _progressText;
        private readonly ReadOnlyReactiveProperty<float> _progressValue;
        
        public IReadOnlyReactiveProperty<int> CurrentCellsCleared => _currentCellsCleared;
        public IReadOnlyReactiveProperty<int> GoalCells => _goalCells;
        public IReadOnlyReactiveProperty<string> ProgressText => _progressText;
        public IReadOnlyReactiveProperty<float> ProgressValue => _progressValue;
        
        public LevelProgressionViewModel(LevelManager levelManager)
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
            
            // Progress text (format: "100/250")
            _progressText = Observable.CombineLatest(
                    _currentCellsCleared,
                    _goalCells,
                    (current, goal) => $"{current}/{goal}")
                .ToReadOnlyReactiveProperty("0/0")
                .AddTo(_disposables);
            
            // Progress value (0-1 for slider)
            _progressValue = _levelManager.Progress
                .ToReadOnlyReactiveProperty(0f)
                .AddTo(_disposables);
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}

