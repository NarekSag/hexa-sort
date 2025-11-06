using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using _Project.Scripts.Runtime.Gameplay.UI;

namespace _Project.Scripts.Runtime.Gameplay.UI.Boosters
{
    public class BoosterView : MonoBehaviour, IView<BoosterViewModel>
    {
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Button _closeButton;
        
        private BoosterViewModel _viewModel;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        public void Initialize(BoosterViewModel viewModel)
        {
            _viewModel = viewModel;
            Bind();
        }
        
        private void Bind()
        {
            gameObject.SetActive(false);
            
            // Bind description text
            if (_descriptionText != null)
            {
                _viewModel.Description
                    .Subscribe(description => _descriptionText.text = description)
                    .AddTo(_disposables);
            }
            
            // Close button
            if (_closeButton != null)
            {
                _closeButton.onClick.AsObservable()
                    .Subscribe(_ => _viewModel.Close())
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

