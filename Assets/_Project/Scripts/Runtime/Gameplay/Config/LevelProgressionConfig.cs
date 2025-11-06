using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Slot;
using _Project.Scripts.Runtime.Gameplay.Presentation.Cell;

namespace _Project.Scripts.Runtime.Gameplay.Config {
    [CreateAssetMenu(fileName = "LevelProgressionConfig", menuName = "Hexa Sort/Level Progression Config")]
    public class LevelProgressionConfig : ScriptableObject {
        [Header("Prefabs")]
        [Tooltip("Prefab used for all hex slots in the grid")]
        [SerializeField] private HexSlot _slotPrefab;
        
        [Tooltip("Prefab used for all hex cells in stacks")]
        [SerializeField] private HexCell _cellPrefab;
        
        [Header("Level Ranges")]
        [Tooltip("Define progression tiers - each tier applies to a range of levels")]
        [SerializeField] private LevelTier[] _levelTiers;
        
        [Header("Win Conditions")]
        [SerializeField] private int _baseCellsToClear = 50;
        [Tooltip("How many additional cells to clear per level")]
        [SerializeField] private float _cellsToClearIncrement = 25f;  // +25 per level
        [SerializeField] private int _maxCellsToClear = 500;
        
        public LevelTier[] LevelTiers => _levelTiers;
        public HexSlot SlotPrefab => _slotPrefab;
        public HexCell CellPrefab => _cellPrefab;
        
        public LevelData GetLevelData(int levelNumber) {
            LevelTier tier = GetTierForLevel(levelNumber);
            
            if (tier == null) {
                Debug.LogError($"No tier found for level {levelNumber}");
                return null;
            }
            
            return new LevelData {
                LevelNumber = levelNumber,
                GridWidth = tier.GridWidth,
                GridHeight = tier.GridHeight,
                MinColors = tier.MinColors,
                MaxColors = tier.MaxColors,
                MinStackHeight = tier.MinStackHeight,
                MaxStackHeight = tier.MaxStackHeight,
                AvailableColors = tier.AvailableColors,
                CellsToClear = CalculateCellsToClear(levelNumber)
            };
        }
        
        private LevelTier GetTierForLevel(int levelNumber) {
            if (_levelTiers == null || _levelTiers.Length == 0) {
                return null;
            }
            
            for (int i = _levelTiers.Length - 1; i >= 0; i--) {
                if (levelNumber >= _levelTiers[i].StartLevel) {
                    return _levelTiers[i];
                }
            }
            
            return _levelTiers[0];
        }
        
        private int CalculateCellsToClear(int levelNumber) {
            int cells = _baseCellsToClear + Mathf.FloorToInt(levelNumber * _cellsToClearIncrement);
            return Mathf.Min(cells, _maxCellsToClear);
        }
        
        public bool IsValid() {
            return _slotPrefab != null && _cellPrefab != null && _levelTiers != null && _levelTiers.Length > 0;
        }
    }
    
    [System.Serializable]
    public class LevelTier {
        [Header("Level Range")]
        [Tooltip("Starting level for this tier (inclusive)")]
        public int StartLevel = 1;
        
        [Header("Grid Settings")]
        public int GridWidth = 6;
        public int GridHeight = 6;
        
        [Header("Color Settings")]
        [Range(2, 8)]
        public int MinColors = 2;
        [Range(2, 8)]
        public int MaxColors = 3;
        [Tooltip("Colors available in this tier")]
        public ColorType[] AvailableColors;
        
        [Header("Stack Settings")]
        [Range(2, 10)]
        public int MinStackHeight = 3;
        [Range(2, 10)]
        public int MaxStackHeight = 5;
    }
}

