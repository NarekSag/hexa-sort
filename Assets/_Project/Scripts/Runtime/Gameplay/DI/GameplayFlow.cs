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
        private readonly HexAnimationConfig _hexAnimationConfig;
        
        public GameplayFlow(HexGridFactory hexGridFactory, HexGridConfig hexGridConfig, HexAnimationConfig hexAnimationConfig)
        {
            _hexGridFactory = hexGridFactory;
            _hexGridConfig = hexGridConfig;
            _hexAnimationConfig = hexAnimationConfig;
        }

        public void Start()
        {
            try
            {
                _hexGridFactory.Create(_hexGridConfig, _hexAnimationConfig);
                CustomDebug.Log(LogCategory.Gameplay, "GameplayFlow started");
            }
            catch (Exception e)
            {
                CustomDebug.LogError(LogCategory.Gameplay, $"GameplayFlow Start Failed: {e.Message}");
            }
        }
    }
}

