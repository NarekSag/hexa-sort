using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;

namespace _Project.Scripts.Runtime.Gameplay.UI.Settings
{
    public class SettingsView : MonoBehaviour, IView<SettingsViewModel>
    {
        [SerializeField] private Button _settingsButton;
        [SerializeField] private GameObject _fadeOverlay;
        [SerializeField] private GameObject _buttonContainer;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _homeButton;
        
        private SettingsViewModel _viewModel;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        public void Initialize(SettingsViewModel viewModel)
        {
            _viewModel = viewModel;
            Bind();
        }
        
        private void Bind()
        {
            // Settings button toggle
            _settingsButton.onClick.AsObservable()
                .Subscribe(_ => _viewModel.ToggleSettingsMenu())
                .AddTo(_disposables);
            
            // Bind menu visibility
            _viewModel.IsSettingsMenuOpen
                .Subscribe(isOpen =>
                {
                    _fadeOverlay.SetActive(isOpen);
                    _buttonContainer.SetActive(isOpen);
                })
                .AddTo(_disposables);
            
            // Restart button
            _restartButton.onClick.AsObservable()
                .Subscribe(_ => _viewModel.RestartLevel())
                .AddTo(_disposables);
            
            // Home button
            _homeButton.onClick.AsObservable()
                .Subscribe(_ => _viewModel.GoHome())
                .AddTo(_disposables);
            
            // Close on fade overlay click (outside buttons)
            SetupFadeOverlayClick();
        }
        
        private void SetupFadeOverlayClick()
        {
            // Add EventTrigger if not present
            EventTrigger trigger = _fadeOverlay.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = _fadeOverlay.AddComponent<EventTrigger>();
            }
            
            // Create pointer click entry
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) =>
            {
                // Check if click target is the overlay itself (not child buttons)
                PointerEventData pointerData = (PointerEventData)data;
                if (pointerData.pointerCurrentRaycast.gameObject == _fadeOverlay)
                {
                    _viewModel.CloseSettingsMenu();
                }
            });
            
            trigger.triggers.Add(entry);
        }
        
        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}

