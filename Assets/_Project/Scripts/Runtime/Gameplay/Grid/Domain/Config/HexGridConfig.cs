using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Grid.Presentation;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Domain.Config {
    [CreateAssetMenu(fileName = "HexGridConfig", menuName = "Gameplay/Grid/Hex Grid Config")]
    public class HexGridConfig : ScriptableObject {
        [SerializeField] private int _width = 6;
        [SerializeField] private int _height = 6;
        [SerializeField] private HexCell _cellPrefab;

        public int Width => _width;
        public int Height => _height;
        public HexCell CellPrefab => _cellPrefab;

        public bool IsValid() {
            return _cellPrefab != null && _width > 0 && _height > 0;
        }
    }
}

