using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Gameplay.Presentation.Cell;
using _Project.Scripts.Runtime.Gameplay.Presentation.Stack;
using _Project.Scripts.Runtime.Gameplay.Config;
using VContainer;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.Factories {
    public class HexStackFactory {
        [Inject] private readonly LevelProgressionConfig _config;
        
        public IStack CreateRandomStack(Transform parent = null, Vector3 position = default, LevelData levelData = null) {
            GameObject stackObject = new GameObject("HexStack");
            stackObject.transform.SetParent(parent);
            stackObject.transform.position = position;
            
            IStack stack = stackObject.AddComponent<HexStack>();
            
            // Use level data if provided, otherwise use defaults
            int minHeight = levelData?.MinStackHeight ?? 2;
            int maxHeight = levelData?.MaxStackHeight ?? 6;
            ColorType[] availableColors = levelData?.AvailableColors ?? new[] { ColorType.Red, ColorType.Blue };
            
            // Ensure minimum height is at least 2 to match Range constraint and prevent invalid stacks
            minHeight = Mathf.Max(2, minHeight);
            maxHeight = Mathf.Max(minHeight, maxHeight);
            
            int cellCount = Random.Range(minHeight, maxHeight + 1);
            
            for (int i = 0; i < cellCount; i++) {
                ICell cell = CreateRandomCell(stackObject.transform, i, availableColors);
                stack.Cells.Add(cell);
            }
            
            stack.Initialize();
            
            return stack;
        }

        private ICell CreateRandomCell(Transform parent, int index, ColorType[] availableColors) {
            if (_config.CellPrefab == null) {
                Debug.LogError("Cell prefab is not assigned!");
                return null;
            }
            
            ICell cell = Object.Instantiate(_config.CellPrefab, parent);
            
            ColorType randomColor = availableColors[Random.Range(0, availableColors.Length)];
            
            cell.Initialize(randomColor, index);
            
            return cell;
        }
    }
}

