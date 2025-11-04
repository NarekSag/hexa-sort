using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Gameplay.Presentation.Cell;
using _Project.Scripts.Runtime.Gameplay.Presentation.Stack;
using _Project.Scripts.Runtime.Gameplay.Config;
using VContainer;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.Factories {
    public class HexStackFactory {
        [Inject] private readonly HexStackConfig _config;
        
        public IStack CreateRandomStack(Transform parent = null, Vector3 position = default) {
            GameObject stackObject = new GameObject("HexStack");
            stackObject.transform.SetParent(parent);
            stackObject.transform.position = position;
            
            IStack stack = stackObject.AddComponent<HexStack>();
            
            int cellCount = Random.Range(2, 6);
            
            for (int i = 0; i < cellCount; i++) {
                ICell cell = CreateRandomCell(stackObject.transform, i);
                stack.Cells.Add(cell);
            }
            
            stack.Initialize();
            
            return stack;
        }

        private ICell CreateRandomCell(Transform parent, int index) {
            if (_config.CellPrefab == null) {
                Debug.LogError("Cell prefab is not assigned!");
                return null;
            }
            
            ICell cell = Object.Instantiate(_config.CellPrefab, parent);
            
            ColorType randomColor = _config.AvailableColors[Random.Range(0, _config.AvailableColors.Length)];
            
            cell.Initialize(randomColor, index);
            
            return cell;
        }
    }
}

