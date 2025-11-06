using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Runtime.Gameplay.UI.Boosters
{
    /// <summary>
    /// Component to reference a booster button.
    /// Attach this to booster button prefabs.
    /// </summary>
    public class BoosterButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        
        public Button Button => _button;
    }
}

