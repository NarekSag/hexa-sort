using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

namespace _Project.Scripts.Runtime.Gameplay.UI.LevelProgression
{
    public class LevelProgressionView : MonoBehaviour, IView<LevelProgressionViewModel>
    {
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private TextMeshProUGUI _progressText;
        
        private LevelProgressionViewModel _viewModel;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        public void Initialize(LevelProgressionViewModel viewModel)
        {
            _viewModel = viewModel;
            Bind();
        }
        
        private void Bind()
        {
            // Bind progress text
            _viewModel.ProgressText
                .Subscribe(text => _progressText.text = text)
                .AddTo(_disposables);
            
            // Bind progress slider value
            _viewModel.ProgressValue
                .Subscribe(value => _progressSlider.value = value)
                .AddTo(_disposables);
        }
        
        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}

