using UnityEngine;
using System;
using System.Collections.Generic;

namespace _Project.Scripts.Runtime.Gameplay.Cell {
    /// <summary>
    /// ScriptableObject that maps ColorType to Materials.
    /// This allows for easy configuration of cell colors in the inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "ColorMaterialConfig", menuName = "Hexa Sort/Color Material Config")]
    public class ColorMaterialConfig : ScriptableObject {
        [SerializeField] private List<ColorMaterialMapping> _colorMappings = new List<ColorMaterialMapping>();
        
        private Dictionary<ColorType, Material> _materialCache;
        
        /// <summary>
        /// Gets the material for a given color type.
        /// </summary>
        /// <param name="colorType">The color type to get material for.</param>
        /// <returns>The material, or null if not found.</returns>
        public Material GetMaterial(ColorType colorType) {
            // Build cache on first access
            if (_materialCache == null) {
                BuildCache();
            }
            
            if (_materialCache.TryGetValue(colorType, out Material material)) {
                return material;
            }
            
            Debug.LogWarning($"No material found for ColorType: {colorType}");
            return null;
        }
        
        private void BuildCache() {
            _materialCache = new Dictionary<ColorType, Material>();
            
            foreach (var mapping in _colorMappings) {
                if (mapping.Material != null) {
                    _materialCache[mapping.ColorType] = mapping.Material;
                }
            }
        }
        
        private void OnValidate() {
            // Clear cache when values change in inspector
            _materialCache = null;
        }
    }
    
    /// <summary>
    /// Serializable mapping between ColorType and Material.
    /// </summary>
    [Serializable]
    public class ColorMaterialMapping {
        [SerializeField] private ColorType _colorType;
        [SerializeField] private Material _material;
        
        public ColorType ColorType => _colorType;
        public Material Material => _material;
    }
}

