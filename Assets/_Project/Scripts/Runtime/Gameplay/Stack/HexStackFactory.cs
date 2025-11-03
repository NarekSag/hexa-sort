using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Cell;
using _Project.Scripts.Runtime.Gameplay.Stack.Config;
using VContainer;

namespace _Project.Scripts.Runtime.Gameplay.Stack {
    public class HexStackFactory {
        [Inject] private readonly HexStackConfig _config;
        
        public HexStack CreateRandomStack(Transform parent = null, Vector3 position = default) {
            GameObject stackObject = new GameObject("HexStack");
            stackObject.transform.SetParent(parent);
            stackObject.transform.position = position;
            
            HexStack stack = stackObject.AddComponent<HexStack>();
            
            int cellCount = Random.Range(2, 6);
            
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

