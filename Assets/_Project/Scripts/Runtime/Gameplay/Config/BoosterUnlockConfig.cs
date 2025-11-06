using System;
using System.Linq;
using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Domain.Boosters;

namespace _Project.Scripts.Runtime.Gameplay.Config
{
    /// <summary>
    /// Configuration for booster unlock requirements based on level progression.
    /// </summary>
    [CreateAssetMenu(fileName = "BoosterUnlockConfig", menuName = "Hexa Sort/Booster Unlock Config")]
    public class BoosterUnlockConfig : ScriptableObject
    {
        [SerializeField] private BoosterUnlockData[] _boosterUnlocks;
        
        /// <summary>
        /// Gets the level required to unlock a specific booster.
        /// </summary>
        /// <param name="boosterType">The type of booster to check.</param>
        /// <returns>The unlock level, or 1 if not configured (unlocked from start).</returns>
        public int GetUnlockLevel(BoosterType boosterType)
        {
            var unlockData = _boosterUnlocks?.FirstOrDefault(data => data.BoosterType == boosterType);
            return unlockData?.UnlockLevel ?? 1;
        }
        
        /// <summary>
        /// Checks if a booster is unlocked at the given level.
        /// </summary>
        /// <param name="boosterType">The type of booster to check.</param>
        /// <param name="currentLevel">The player's current level.</param>
        /// <returns>True if the booster is unlocked, false otherwise.</returns>
        public bool IsUnlocked(BoosterType boosterType, int currentLevel)
        {
            int unlockLevel = GetUnlockLevel(boosterType);
            return currentLevel >= unlockLevel;
        }
    }
    
    /// <summary>
    /// Data structure for booster unlock requirements.
    /// </summary>
    [Serializable]
    public class BoosterUnlockData
    {
        [Tooltip("The type of booster")]
        public BoosterType BoosterType;
        
        [Tooltip("The level at which this booster unlocks")]
        [Min(1)]
        public int UnlockLevel = 1;
    }
}

