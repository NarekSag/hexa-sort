using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Slot;

namespace _Project.Scripts.Runtime.Gameplay.Config {
    [CreateAssetMenu(fileName = "HexGridConfig", menuName = "Gameplay/Grid/Hex Grid Config")]
    public class HexGridConfig : ScriptableObject {
        [SerializeField] private int _width = 6;
        [SerializeField] private int _height = 6;
        [SerializeField] private HexSlot _slotPrefab;

        public int Width => _width;
        public int Height => _height;
        public HexSlot SlotPrefab => _slotPrefab;

        public bool IsValid() {
            return _slotPrefab != null && _width > 0 && _height > 0;
        }
    }
}

