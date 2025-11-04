using UnityEngine;
using _Project.Scripts.Runtime.Utilities.Logging;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Gameplay.Config;

namespace _Project.Scripts.Runtime.Gameplay.Presentation.Cell {
    public class HexCellMaterial {
        private readonly GameObject _gameObject;
        private readonly ColorMaterialConfig _colorMaterialConfig;
        private MeshRenderer _meshRenderer;
        
        public HexCellMaterial(GameObject gameObject, ColorMaterialConfig colorMaterialConfig) {
            _gameObject = gameObject;
            _colorMaterialConfig = colorMaterialConfig;
        }
        
        public void Initialize() {
            _meshRenderer = _gameObject.GetComponent<MeshRenderer>();
            if (_meshRenderer == null) {
                _meshRenderer = _gameObject.GetComponentInChildren<MeshRenderer>();
            }
        }
        
        public void UpdateMaterial(ColorType colorType) {
            if (_meshRenderer == null) {
                CustomDebug.LogWarning(LogCategory.Cell, $"No MeshRenderer found on {_gameObject.name}");
                return;
            }
            
            if (_colorMaterialConfig == null) {
                CustomDebug.LogWarning(LogCategory.Cell, $"ColorMaterialConfig not assigned on {_gameObject.name}");
                return;
            }
            
            Material material = _colorMaterialConfig.GetMaterial(colorType);
            if (material != null) {
                _meshRenderer.material = material;
            }
        }
    }
}

