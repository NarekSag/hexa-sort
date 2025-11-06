using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using _Project.Scripts.Runtime.Gameplay.UI;
using _Project.Scripts.Runtime.Gameplay.Domain.Boosters;

namespace _Project.Scripts.Runtime.Gameplay.UI.Boosters
{
    public class BoosterSelectionView : MonoBehaviour, IView<BoosterSelectionViewModel>
    {
        [SerializeField] private Transform _boosterContainer;
        [SerializeField] private GameObject _boosterButtonPrefab;
        
        private BoosterSelectionViewModel _viewModel;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        public void Initialize(BoosterSelectionViewModel viewModel)
        {
            _viewModel = viewModel;
            Bind();
        }
        
        private void Bind()
        {
            // Start visible (will be controlled by state changes)
            gameObject.SetActive(true);
            
            // Subscribe to available boosters and create UI buttons
            _viewModel.AvailableBoosters
                .Subscribe(UpdateBoosterButtons)
                .AddTo(_disposables);
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private void UpdateBoosterButtons(IEnumerable<IBooster> boosters)
        {
            if (_boosterContainer == null || _boosterButtonPrefab == null)
            {
                return;
            }
            
            // Clear existing buttons
            foreach (Transform child in _boosterContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Create buttons for each available booster
            foreach (var booster in boosters)
            {
                GameObject buttonObj = Instantiate(_boosterButtonPrefab, _boosterContainer);
                
                // Get booster button component and use its button reference
                BoosterButton boosterButton = buttonObj.GetComponent<BoosterButton>();
                Button button = boosterButton != null ? boosterButton.Button : buttonObj.GetComponent<Button>();
                
                if (button != null)
                {
                    button.onClick.AddListener(() => _viewModel.SelectBooster(booster));
                }
            }
        }
        
        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}

