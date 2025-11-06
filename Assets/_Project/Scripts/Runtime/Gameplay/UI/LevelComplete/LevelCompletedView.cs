using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using _Project.Scripts.Runtime.Gameplay.UI;

namespace _Project.Scripts.Runtime.Gameplay.UI.LevelComplete
{
    public class LevelCompletedView : MonoBehaviour, IView<LevelCompletedViewModel>
    {
        [SerializeField] private TextMeshProUGUI _levelCompleteText;
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _homeButton;
        
        private LevelCompletedViewModel _viewModel;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        public void Initialize(LevelCompletedViewModel viewModel)
        {
            _viewModel = viewModel;
            Bind();
        }
        
        private void Bind()
        {
            // Bind level complete text
            _viewModel.LevelCompleteText
                .Subscribe(text =>
                {
                    if (_levelCompleteText != null)
                    {
                        _levelCompleteText.text = text;
                    }
                })
                .AddTo(_disposables);
            
            // Next level button
            if (_nextLevelButton != null)
            {
                _nextLevelButton.onClick.AsObservable()
                    .Subscribe(_ => _viewModel.LoadNextLevel())
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

