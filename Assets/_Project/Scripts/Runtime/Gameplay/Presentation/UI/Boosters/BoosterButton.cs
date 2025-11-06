using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _Project.Scripts.Runtime.Gameplay.Domain.Boosters;
using _Project.Scripts.Runtime.Gameplay.Config;
using System;

namespace _Project.Scripts.Runtime.Gameplay.UI.Boosters
{
    public class BoosterButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _iconImage;
        [SerializeField] private GameObject _lockOverlay;
        [SerializeField] private TextMeshProUGUI _unlockLevelText;

        public Button Button => _button;

        public void Initialize(BoosterType boosterType, bool isUnlocked, int unlockLevel, BoosterIconConfig iconConfig)
        {
            SetIcon(boosterType, iconConfig);

            if (isUnlocked)
            {
                SetUnlocked();
            }
            else
            {
                SetLocked(true, unlockLevel);
            }
        }

        private void SetLocked(bool isLocked, int unlockLevel)
        {
            if (_button != null)
            {
                _button.interactable = !isLocked;
            }

            if (_lockOverlay != null)
            {
                _lockOverlay.SetActive(isLocked);
            }

            if (_unlockLevelText != null && isLocked)
            {
                _unlockLevelText.text = $"Level {unlockLevel}";
            }
        }

        private void SetUnlocked()
        {
            SetLocked(false, 0);
        }

        private void SetIcon(BoosterType boosterType, BoosterIconConfig iconConfig)
        {
            if (_iconImage != null)
            {
                _iconImage.sprite = iconConfig.GetIcon(boosterType);
                _iconImage.enabled = _iconImage.sprite != null;
            }
        }
    }
}

