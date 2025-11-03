using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Cell;
using _Project.Scripts.Runtime.Gameplay.Stack.Config;
using VContainer;

namespace _Project.Scripts.Runtime.Gameplay.Stack {
    public class HexStackFactory {
        [Inject] private readonly HexStackConfig _config;
        
        public HexStack CreateRandomStack(Transform parent = null, Vector3 position = default) {
            // Create stack GameObject
            GameObject stackObject = new GameObject("HexStack");
            stackObject.transform.SetParent(parent);
            stackObject.transform.position = position;
            
            // Add HexStack component
            HexStack stack = stackObject.AddComponent<HexStack>();
            
            // Generate random number of cells (2-5)
            int cellCount = Random.Range(2, 6);
            
            // Create cells and add them to the stack
            for (int i = 0; i < cellCount; i++) {
                HexCell cell = CreateRandomCell(stackObject.transform, i);
                stack.Hexagons.Add(cell);
            }
            
            stack.Initialize();
            
            return stack;
        }

        private HexCell CreateRandomCell(Transform parent, int index) {
            if (_config.CellPrefab == null) {
                Debug.LogError("Cell prefab is not assigned!");
                return null;
            }
            
            HexCell cell = Object.Instantiate(_config.CellPrefab, parent);
            
            ColorType randomColor = _config.AvailableColors[Random.Range(0, _config.AvailableColors.Length)];
            cell.Initialize(randomColor, index);
            
            return cell;
        }
    }
}

