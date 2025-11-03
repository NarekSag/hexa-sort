using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Cell;

namespace _Project.Scripts.Runtime.Gameplay.Stack.Config {
    [CreateAssetMenu(fileName = "HexStackConfig", menuName = "Hexa Sort/Hex Stack Config")]
    public class HexStackConfig : ScriptableObject {
        [SerializeField] private HexCell _cellPrefab;
        [SerializeField] private ColorType[] _availableColors = new[] { ColorType.Red, ColorType.Blue };
        
        public HexCell CellPrefab => _cellPrefab;
        public ColorType[] AvailableColors => _availableColors;
        
        public bool IsValid() {
            return _cellPrefab != null && _availableColors != null && _availableColors.Length > 0;
        }
    }
}

