using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using _Project.Scripts.Runtime.Gameplay.UI;
using _Project.Scripts.Runtime.Gameplay.Domain.Boosters;
using _Project.Scripts.Runtime.Gameplay.Domain.Level;
using _Project.Scripts.Runtime.Gameplay.Config;

namespace _Project.Scripts.Runtime.Gameplay.UI.Boosters
{
    public class BoosterSelectionView : MonoBehaviour, IView<BoosterSelectionViewModel>
    {
        [SerializeField] private Transform _boosterContainer;
        [SerializeField] private GameObject _boosterButtonPrefab;

        [SerializeField] private BoosterIconConfig _iconConfig;
        
        private BoosterSelectionViewModel _viewModel;
        private BoosterManager _boosterManager;
        private LevelManager _levelManager;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        // IView interface implementation (kept for compatibility, but use the overload below)
        public void Initialize(BoosterSelectionViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        
        // Preferred initialization with all dependencies
        public void Initialize(BoosterSelectionViewModel viewModel, BoosterManager boosterManager, LevelManager levelManager)
        {
            _viewModel = viewModel;
            _boosterManager = boosterManager;
            _levelManager = levelManager;
            Bind();
        }
        
        private void Bind()
        {
            gameObject.SetActive(true);
            
            // Combine boosters and level changes into single reactive stream
            Observable.CombineLatest(
                _viewModel.AvailableBoosters,
                _levelManager.CurrentLevel,
                (boosters, _) => boosters)
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
            if (_boosterContainer == null || _boosterButtonPrefab == null) return;
            
            // Clear existing buttons
            foreach (Transform child in _boosterContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Create and configure buttons for each booster
            foreach (var booster in boosters)
            {
                var buttonObj = Instantiate(_boosterButtonPrefab, _boosterContainer);
                var boosterButton = buttonObj.GetComponent<BoosterButton>();
                if (boosterButton == null) continue;
                
                bool isUnlocked = _boosterManager.IsBoosterUnlocked(booster.Type);
                int unlockLevel = _boosterManager.GetUnlockLevel(booster.Type);

                boosterButton.Initialize(booster.Type, isUnlocked, unlockLevel, _iconConfig);
                boosterButton.Button.onClick.AddListener(() => _viewModel.SelectBooster(booster));
            }
        }
        
        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}

