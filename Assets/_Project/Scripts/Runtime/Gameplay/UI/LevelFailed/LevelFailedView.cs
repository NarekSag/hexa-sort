using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

namespace _Project.Scripts.Runtime.Gameplay.UI.LevelFailed
{
    public class LevelFailedView : MonoBehaviour, IView<LevelFailedViewModel>
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _headerText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private TextMeshProUGUI _progressValueText;
        
        [Space, Header("Buttons")]
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _homeButton;
        
        private LevelFailedViewModel _viewModel;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        public void Initialize(LevelFailedViewModel viewModel)
        {
            _viewModel = viewModel;
            Bind();
        }
        
        private void Bind()
        {
            gameObject.SetActive(false);
            
            // Set header text
            if (_headerText != null)
            {
                _headerText.text = _viewModel.HeaderText;
            }
            
            // Set description text
            if (_descriptionText != null)
            {
                _descriptionText.text = _viewModel.DescriptionText;
            }
            
            // Set progress text
            if (_progressText != null)
            {
                _progressText.text = _viewModel.ProgressText;
            }
            
            // Bind progress value text (dynamic)
            if (_progressValueText != null)
            {
                _viewModel.ProgressValueText
                    .Subscribe(value => _progressValueText.text = value)
                    .AddTo(_disposables);
            }
            
            // Restart button
            if (_restartButton != null)
            {
                _restartButton.onClick.AsObservable()
                    .Subscribe(_ => _viewModel.RestartLevel())
                    .AddTo(_disposables);
            }
            
            // Home button
            if (_homeButton != null)
            {
                _homeButton.onClick.AsObservable()
                    .Subscribe(_ => _viewModel.GoHome())
                    .AddTo(_disposables);
            }
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}

