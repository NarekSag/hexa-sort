using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _Project.Scripts.Runtime.Gameplay.UI.Boosters
{
    /// <summary>
    /// Component to reference a booster button.
    /// Attach this to booster button prefabs.
    /// </summary>
    public class BoosterButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _lockOverlay;
        [SerializeField] private TextMeshProUGUI _unlockLevelText;
        
        public Button Button => _button;
        
        /// <summary>
        /// Sets the button to locked state with unlock level information.
        /// </summary>
        /// <param name="isLocked">Whether the button should be locked.</param>
        /// <param name="unlockLevel">The level at which this booster unlocks.</param>
        public void SetLocked(bool isLocked, int unlockLevel)
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
        
        /// <summary>
        /// Sets the button to unlocked state.
        /// </summary>
        public void SetUnlocked()
        {
            SetLocked(false, 0);
        }
    }
}

