using System;
using VContainer.Unity;
using _Project.Scripts.Runtime.Utilities.Logging;
using _Project.Scripts.Runtime.Gameplay.Grid.Presentation;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Config;

namespace _Project.Scripts.Runtime.Gameplay.DI
{
    public class GameplayFlow : IStartable
    {
        private readonly HexGridFactory _hexGridFactory;
        private readonly HexGridConfig _hexGridConfig;

        public GameplayFlow(HexGridFactory hexGridFactory, HexGridConfig hexGridConfig)
        {
            _hexGridFactory = hexGridFactory;
            _hexGridConfig = hexGridConfig;
        }

        public void Start()
        {
            try
            {
                _hexGridFactory.Create(_hexGridConfig);
                CustomDebug.Log(LogCategory.Gameplay, "GameplayFlow started");
            }
            catch (Exception e)
            {
                CustomDebug.LogError(LogCategory.Gameplay, $"GameplayFlow Start Failed: {e.Message}");
            }
        }
    }
}

