using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Mappers;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Config;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Presentation {
    public class HexGridFactory {
        public HexGrid Create(HexGridConfig config, Transform parent = null) {
            if (config == null) {
                Debug.LogError("HexGridConfig is not assigned!");
                return null;
            }

            if (!config.IsValid()) {
                Debug.LogError("HexGridConfig is invalid! Check width, height, and cell prefab.");
                return null;
            }

            GameObject gridObject = new GameObject("HexGrid");
            if (parent != null) {
                gridObject.transform.SetParent(parent);
            }

            HexGrid grid = new HexGrid();
            IHexGridMapper mapper = new HexGridMapper(config.Width, config.Height);
            grid.Initialize(config, mapper, gridObject.transform);

            return grid;
        }
    }
}

