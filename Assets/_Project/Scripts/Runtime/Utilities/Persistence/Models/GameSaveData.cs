using System;

namespace _Project.Scripts.Runtime.Utilities.Persistence.Models
{
    [Serializable]
    public class GameSaveData
    {
        public int CurrentLevel;

        public GameSaveData()
        {
            CurrentLevel = 1;
        }

        public GameSaveData(int currentLevel)
        {
            CurrentLevel = currentLevel;
        }
    }
}

