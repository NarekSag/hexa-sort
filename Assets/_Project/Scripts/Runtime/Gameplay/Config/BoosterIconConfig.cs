using System;
using System.Collections.Generic;
using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Domain.Boosters;

namespace _Project.Scripts.Runtime.Gameplay.Config
{
    [CreateAssetMenu(fileName = "BoosterIconConfig", menuName = "Hexa Sort/Booster Icon Config")]
    public class BoosterIconConfig : ScriptableObject
    {
        [SerializeField] private List<BoosterIconMapping> _iconMappings = new List<BoosterIconMapping>();
        
        private Dictionary<BoosterType, Sprite> _iconCache;
        public Sprite GetIcon(BoosterType boosterType)
        {
            // Build cache on first access
            if (_iconCache == null)
            {
                BuildCache();
            }
            
            if (_iconCache.TryGetValue(boosterType, out Sprite sprite))
            {
                return sprite;
            }
            
            Debug.LogWarning($"No icon found for BoosterType: {boosterType}");
            return null;
        }
        
        private void BuildCache()
        {
            _iconCache = new Dictionary<BoosterType, Sprite>();
            
            foreach (var mapping in _iconMappings)
            {
                if (mapping.Icon != null)
                {
                    _iconCache[mapping.BoosterType] = mapping.Icon;
                }
            }
        }
        
        private void OnValidate()
        {
            // Clear cache when values change in inspector
            _iconCache = null;
        }
    }
    
    /// <summary>
    /// Serializable mapping between BoosterType and Sprite.
    /// </summary>
    [Serializable]
    public class BoosterIconMapping
    {
        [Tooltip("The type of booster")]
        public BoosterType BoosterType;
        
        [Tooltip("The icon sprite for this booster")]
        public Sprite Icon;
    }
}

